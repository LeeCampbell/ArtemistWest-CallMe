using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using ArtemisWest.CallMe.Contract;
using Newtonsoft.Json.Linq;
//For help see https://developers.google.com/accounts/docs/OAuth2InstalledApp
namespace ArtemisWest.CallMe.Google.Authorization
{
    //TODO: Move actual http calls into a seperat service so we can unit test this class. -LC
    public sealed class AuthorizationModel : IAuthorizationModel
    {
        private readonly ILocalStore _localStore;
        private readonly ILogger _logger;
        private const string AppName = "CallMe.Spike";
        private const string ClientId = "410654176090.apps.googleusercontent.com";
        private const string ClientSecret = "bDkwW8Y2RnUt0JsjbAwYA8cb";
        private const string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";

        private readonly BehaviorSubject<AuthorizationStatus> _status = new BehaviorSubject<AuthorizationStatus>(AuthorizationStatus.NotAuthorized);
        private readonly IResourceScope[] _availableServices;
        private readonly ObservableCollection<IResourceScope> _selectedServices;
        private RequestAuthorizationCode _callback;
        private Session _currentSession;

        public AuthorizationModel(IEnumerable<IResourceScope> availableServices, ILocalStore localStore, ILoggerFactory loggerFactory)
        {
            _localStore = localStore;
            _logger = loggerFactory.GetLogger();
            _availableServices = availableServices.ToArray();
            _selectedServices = new ObservableCollection<IResourceScope>(_availableServices);

            _currentSession = LoadSession();
            if (_currentSession != null)
            {
                _status.OnNext(AuthorizationStatus.Authorized);
            }
        }

        public IObservable<AuthorizationStatus> Status
        {
            get { return _status.AsObservable(); }
        }

        public IResourceScope[] AvailableServices
        {
            get { return _availableServices; }
        }

        public ObservableCollection<IResourceScope> SelectedServices
        {
            get { return _selectedServices; }
        }

        public void RegisterAuthorizationCallback(RequestAuthorizationCode callback)
        {
            _callback = callback;
        }

        private Session CurrentSession
        {
            get { return _currentSession; }
            set
            {
                _currentSession = value;
                if (_currentSession == null)
                {
                    _localStore.Remove("Google.AccessToken");
                    _localStore.Remove("Google.AccessTokenExpires");
                    _localStore.Remove("Google.RefreshToken");
                }
                else
                {
                    _localStore.Put("Google.AccessToken", _currentSession.AccessToken);
                    _localStore.Put("Google.AccessTokenExpires", _currentSession.Expires.ToString("o"));
                    _localStore.Put("Google.RefreshToken", _currentSession.RefreshToken);
                    _status.OnNext(AuthorizationStatus.Authorized);
                }
            }
        }

        private string AuthorizationCode
        {
            get { return _localStore.Get("Google.AuthorizationCode"); }
            set { _localStore.Put("Google.AuthorizationCode", value); }
        }

        public IObservable<string> RequestAccessToken()
        {
            var currentSession = Observable.Return(CurrentSession);
            var refreshSession = Observable.Defer(RefreshSession);
            var createSession = Observable.Defer(CreateSession);
            
            var sessionPriorities = ObservableExtensions.LazyConcat(currentSession, refreshSession, createSession);

            var sequence = sessionPriorities
                .Where(session => session != null && !session.HasExpired())
                .Do(session => CurrentSession = session)
                .Take(1)
                .Select(session => session.AccessToken);
            return sequence.Log(_logger, "RequestAccessToken()");
        }

        private IObservable<Session> RefreshSession()
        {
            if (CurrentSession == null)
                return Observable.Empty<Session>().Log(_logger, "refreshSession()");
            return (from authCode in GetAuthorizationCode()
                    from accessToken in RequestRefreshedAccessToken(CurrentSession.RefreshToken)
                    select accessToken).Log(_logger, "refreshSession()");
        }

        private IObservable<Session> CreateSession()
        {
            return (from authCode in GetAuthorizationCode()
                   from accessToken in RequestAccessToken(authCode)
                    select accessToken).Log(_logger, "createSession()");
        }

        private IObservable<string> GetAuthorizationCode()
        {
            return Observable.Create<string>(
                o =>
                {
                    if (AuthorizationCode != null)
                    {
                        return Observable.Return(AuthorizationCode)
                                         .Subscribe(o);
                    }
                    return RequestAuthorizationCode()
                        .Do(newCode => AuthorizationCode = newCode)
                        .Subscribe(o);
                })
                .Log(_logger, "getAuthorizationCode()");
        }

        private IObservable<string> RequestAuthorizationCode()
        {
            return Observable.Create<string>(
                o =>
                {


                    if (_callback == null)
                        throw new InvalidOperationException("No callback has been registered via the RegisterAuthorizationCallback method");
                    var uri = BuildAuthorizationUri();
                    return _callback(uri).Subscribe(o);
                })
                .Log(_logger, "requestAuthorizationCode()");
        }

        private Session LoadSession()
        {
            var accessToken = _localStore.Get("Google.AccessToken");
            var strExpires = _localStore.Get("Google.AccessTokenExpires");
            var refreshToken = _localStore.Get("Google.RefreshToken");
            if (accessToken == null || strExpires == null || refreshToken == null)
                return null;
            var expires = DateTimeOffset.Parse(strExpires);
            return new Session(accessToken, refreshToken, expires);
        }

        private Uri BuildAuthorizationUri()
        {
            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString["response_type"] = "code";
            queryString["client_id"] = ClientId; //Lee.Ryan.Campbell@gmail.com client Id
            queryString["redirect_uri"] = RedirectUri;
            var scopes = _selectedServices.Select(svc => svc.Resource.ToString()).ToArray();
            queryString["scope"] = string.Join("+", scopes);

            var authorizationUri = new UriBuilder(@"https://accounts.google.com/o/oauth2/auth");
            authorizationUri.Query = queryString.ToString(); // Returns "key1=value1&key2=value2", all URL-encoded
            return authorizationUri.Uri;
        }

        private IObservable<Session> RequestAccessToken(string authorizationCode)
        {
            return Observable.Create<Session>(
                o =>
                {
                    try
                    {
                        var request = CreateAccessTokenWebRequest(authorizationCode);
                        var requestedAt = DateTimeOffset.Now;
                        
                        var q = from response in request.GetResponseAsync().ToObservable()
                                let responseStream = response.GetResponseStream()
                                let reader = new StreamReader(responseStream)
                                from x in reader.ReadToEndAsync().ToObservable()
                                select x;
                       
                        return q.Select(payload => JObject.Parse(payload))
                                .Select(json => new Session(
                                    (string)json["access_token"],
                                    (string)json["refresh_token"],
                                    TimeSpan.FromSeconds((int)json["expires_in"]),
                                    requestedAt))
                                .Subscribe(o);

                        //Old code
                        using (var response = request.GetResponse())
                        {
                            var responseStream = response.GetResponseStream();
                            using (var reader = new StreamReader(responseStream))
                            {
                                var result = reader.ReadToEnd();
                                var payload = JObject.Parse(result);
                                var session = new Session(
                                    (string)payload["access_token"],
                                    (string)payload["refresh_token"],
                                    TimeSpan.FromSeconds((int)payload["expires_in"]),
                                    requestedAt);
                                o.OnNext(session);
                                o.OnCompleted();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        o.OnError(e);
                    }
                    return Disposable.Empty;    //TODO: Provide real Cancellation feature.
                })
                .Log(_logger, string.Format("requestAccessToken({0})", authorizationCode));
        }

        private IObservable<Session> RequestRefreshedAccessToken(string refreshToken)
        {
            return Observable.Create<Session>(
                o =>
                {
                    try
                    {
                        var request = CreateRefreshTokenWebRequest(refreshToken);
                        var requestedAt = DateTimeOffset.Now;
                        using (var response = request.GetResponse())
                        {
                            var responseStream = response.GetResponseStream();
                            using (var reader = new StreamReader(responseStream))
                            {
                                var result = reader.ReadToEnd();
                                var payload = JObject.Parse(result);
                                var session = new Session(
                                    (string)payload["access_token"],
                                    refreshToken,
                                    TimeSpan.FromSeconds((int)payload["expires_in"]),
                                    requestedAt);
                                o.OnNext(session);
                                o.OnCompleted();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        o.OnError(e);
                    }
                    return Disposable.Empty;    //TODO: Provide real Cancellation feature.
                })
                .Log(_logger, string.Format("requestRefreshedAccessToken({0})", refreshToken));
        }
        
        private HttpWebRequest CreateAccessTokenWebRequest(string authorizationCode)
        {
            var requestParams = new HttpRequestParameters(@"https://accounts.google.com/o/oauth2/token");
            requestParams.PostParameters.Add("code", authorizationCode);
            requestParams.PostParameters.Add("client_id", ClientId);
            requestParams.PostParameters.Add("client_secret", ClientSecret);
            requestParams.PostParameters.Add("redirect_uri", RedirectUri);
            requestParams.PostParameters.Add("grant_type", "authorization_code");
            return requestParams.CreateRequest(_logger);
        }

        private HttpWebRequest CreateRefreshTokenWebRequest(string refreshToken)
        {
            var requestParams = new HttpRequestParameters(@"https://accounts.google.com/o/oauth2/token");
            requestParams.PostParameters.Add("client_id", ClientId);
            requestParams.PostParameters.Add("client_secret", ClientSecret);
            requestParams.PostParameters.Add("refresh_token", refreshToken);
            requestParams.PostParameters.Add("grant_type", "refresh_token");
            return requestParams.CreateRequest(_logger);
        }
    }

    public sealed class OfflineAuthorizationModel : IAuthorizationModel
    {
        private readonly BehaviorSubject<AuthorizationStatus> _status = new BehaviorSubject<AuthorizationStatus>(AuthorizationStatus.Authorized);
        public IObservable<AuthorizationStatus> Status { get { return _status; } }
        public IResourceScope[] AvailableServices { get; private set; }
        public ObservableCollection<IResourceScope> SelectedServices { get; private set; }
        public void RegisterAuthorizationCallback(RequestAuthorizationCode callback)
        {
        }

        public IObservable<string> RequestAccessToken()
        {
            return Observable.Return("OFFLINE");
        }
    }
}

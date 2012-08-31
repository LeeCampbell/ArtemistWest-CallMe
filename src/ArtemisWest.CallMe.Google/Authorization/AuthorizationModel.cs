using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using ArtemisWest.CallMe.Contract;
using Newtonsoft.Json.Linq;

namespace ArtemisWest.CallMe.Google.Authorization
{
    public sealed class AuthorizationModel : IAuthorizationModel
    {
        private readonly ILocalStore _localStore;
        private const string AppName = "CallMe.Spike";
        private const string ClientId = "410654176090.apps.googleusercontent.com";
        private const string ClientSecret = "bDkwW8Y2RnUt0JsjbAwYA8cb";
        private const string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";

        private readonly BehaviorSubject<AuthorizationStatus> _status = new BehaviorSubject<AuthorizationStatus>(AuthorizationStatus.NotAuthorized);
        private readonly IResourceScope[] _availableServices;
        private readonly ObservableCollection<IResourceScope> _selectedServices;
        private RequestAuthorizationCode _callback;
        private string _authorizationCode;
        private Session _currentSession;

        public AuthorizationModel(IEnumerable<IResourceScope> availableServices, ILocalStore localStore)
        {
            _localStore = localStore;
            _availableServices = availableServices.ToArray();
            _selectedServices = new ObservableCollection<IResourceScope>(_availableServices);
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
                if (_currentSession != null) _status.OnNext(AuthorizationStatus.Authorized);
            }
        }

        private string AuthorizationCode
        {
            get
            {
                //return _authorizationCode;
                return _localStore.Get("AuthorizationCode");
            }
            set
            {
                //_authorizationCode = value;
                _localStore.Put("AuthorizationCode", value);
            }
        }

        public IObservable<string> RequestAccessToken()
        {
            var nextSession = Observable.Defer(() => CreateSession().Do(session => CurrentSession = session));
            return Observable.Return(CurrentSession)
                .Concat(nextSession)
                .Where(session => session != null && !session.HasExpired())
                .Take(1)
                .Select(session => session.AccessToken);
        }

        private IObservable<Session> CreateSession()
        {
            return from authCode in GetAuthorizationCode()
                   from accessToken in RequestAccessToken(authCode)
                   select accessToken;
        }

        private IObservable<string> GetAuthorizationCode()
        {
            return Observable.Create<string>(
                o =>
                {
                    if (AuthorizationCode != null)
                    {
                        o.OnNext(AuthorizationCode);
                        o.OnCompleted();
                        return Disposable.Empty;
                    }
                    return RequestAuthorizationCode()
                        .Do(newCode => AuthorizationCode = newCode)
                        .Subscribe(o);
                });
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
                });
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

        private static IObservable<Session> RequestAccessToken(string authorizationCode)
        {
            //Console.WriteLine("RequestAccessToken({0})", authorizationCode);
            return Observable.Create<Session>(
                o =>
                {
                    try
                    {
                        var request = CreateAccessTokenWebRequest(authorizationCode);
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
                });
        }

        private static HttpWebRequest CreateAccessTokenWebRequest(string authorizationCode)
        {
            var postArguments = System.Web.HttpUtility.ParseQueryString(string.Empty);
            postArguments["code"] = authorizationCode;
            postArguments["client_id"] = ClientId; //Lee.Ryan.Campbell@gmail.com client Id
            postArguments["client_secret"] = ClientSecret; //HACK: Need to hide this some how.
            postArguments["redirect_uri"] = RedirectUri;
            postArguments["grant_type"] = "authorization_code";

            var accessTokenUri = new Uri(@"https://accounts.google.com/o/oauth2/token");
            var request = (HttpWebRequest)WebRequest.Create(accessTokenUri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            using (var requestStream = request.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(postArguments.ToString());
            }
            return request;
        }
    }
}

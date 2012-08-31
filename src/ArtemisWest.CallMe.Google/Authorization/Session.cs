using System;

namespace ArtemisWest.CallMe.Google.Authorization
{
    internal sealed class Session
    {
        private readonly string _accessToken;
        private readonly string _refreshToken;
        private readonly TimeSpan _accessPeriod;
        private readonly DateTimeOffset _expires;

        public Session(string accessToken, string refreshToken, TimeSpan accessPeriod, DateTimeOffset requested)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _accessPeriod = accessPeriod;

            _expires = requested + accessPeriod;
        }

        public string AccessToken { get { return _accessToken; } }

        public string RefreshToken { get { return _refreshToken; } }

        public TimeSpan AccessPeriod { get { return _accessPeriod; } }

        public DateTimeOffset Expires { get { return _expires; } }

        public bool HasExpired()
        {
            return DateTimeOffset.Now > _expires;
        }
    }
}
using System;
using ArtemisWest.CallMe.Contract;

namespace ArtemisWest.CallMe.Google
{
    public sealed class GoogleProviderDescription : IProviderDescription
    {
        private static readonly GoogleProviderDescription _instance = new GoogleProviderDescription();
        private static readonly Uri _image;

        static GoogleProviderDescription()
        {
            if (!UriParser.IsKnownScheme("pack"))
            {
                UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);
            }
            _image = new Uri("pack://application:,,,/ArtemisWest.CallMe.Google;component/Google_64x64.png");
        }

        public static IProviderDescription Instance { get { return _instance; } }

        private GoogleProviderDescription()
        { }

        public string Name
        {
            get { return "Google"; }
        }
        public Uri Image
        {
            get { return _image; }
        }
    }
}
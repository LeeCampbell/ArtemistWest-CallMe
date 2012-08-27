using System;
using ArtemisWest.CallMe.Contract;

namespace ArtemisWest.CallMe.Google.Contacts
{
    public class ContactResourceScope : IResourceScope
    {
        public string Name
        {
            get { return "Google Contacts"; }
        }

        public Uri Resource
        {
            get { return new Uri(@"https://www.google.com/m8/feeds/"); }
        }
    }
}

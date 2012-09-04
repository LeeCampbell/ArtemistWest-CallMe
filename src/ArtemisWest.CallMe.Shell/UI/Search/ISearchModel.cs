using System;

namespace ArtemisWest.CallMe.Shell.UI.Search
{
    public interface ISearchModel
    {
        IObservable<Contract.IProfile> IdentityActivated { get; }
        void Search(string query);
    }
}
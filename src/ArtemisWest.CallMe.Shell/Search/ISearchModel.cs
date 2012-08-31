using System;

namespace ArtemisWest.CallMe.Shell.Search
{
    public interface ISearchModel
    {
        IObservable<Contract.IProfile> IdentityActivated { get; }
        void Search(string query);
    }
}
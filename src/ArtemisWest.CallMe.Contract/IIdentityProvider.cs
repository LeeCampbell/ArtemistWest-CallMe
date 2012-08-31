using System;

namespace ArtemisWest.CallMe.Contract
{
    public interface IIdentityProvider
    {
        IObservable<IProfile> FindProfile(string query);
    }
}

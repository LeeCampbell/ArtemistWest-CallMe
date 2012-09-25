using System;
using System.Collections.Generic;

namespace ArtemisWest.CallMe.Contract
{
    public interface IIdentityProvider
    {
        IObservable<IProfile> FindProfile(IList<string> identityKeys);
    }
}

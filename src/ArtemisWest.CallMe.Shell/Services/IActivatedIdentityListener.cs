using System;
using System.Collections.Generic;

namespace ArtemisWest.CallMe.Shell.Services
{
    public interface IActivatedIdentityListener
    {
        IObservable<IList<string>> IdentitiesActivated { get; }
    }
}
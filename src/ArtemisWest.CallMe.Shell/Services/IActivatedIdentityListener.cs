using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;

namespace ArtemisWest.CallMe.Shell.Services
{
    public interface IActivatedIdentityListener
    {
        IObservable<IList<string>> IdentitiesActivated(IScheduler scheduler);
    }
}
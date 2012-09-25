using System;

namespace ArtemisWest.CallMe.Shell.Services
{
    public interface IActiveProfileService
    {
        IObservable<Contract.IProfile> ProfileActivated { get; }
    }
}
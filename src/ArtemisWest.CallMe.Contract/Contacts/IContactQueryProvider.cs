using System;

namespace ArtemisWest.CallMe.Contract.Contacts
{
    public interface IContactQueryProvider
    {
        IObservable<string> Search(Contract.IProfile activeProfile);
    }
}

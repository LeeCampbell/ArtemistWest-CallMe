using System;

namespace ArtemisWest.CallMe.Contract.Contacts
{
    public interface IContactQueryProvider
    {
        IObservable<IContact> Search(Contract.IProfile activeProfile);
    }
}

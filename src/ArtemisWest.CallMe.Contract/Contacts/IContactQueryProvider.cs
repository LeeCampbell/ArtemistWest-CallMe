using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisWest.CallMe.Contract.Contacts
{
    public interface IContactQueryProvider
    {
        IObservable<string> Search(string query);
    }
}

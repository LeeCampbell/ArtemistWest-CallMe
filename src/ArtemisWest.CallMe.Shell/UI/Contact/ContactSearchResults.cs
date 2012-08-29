using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisWest.CallMe.Shell.UI.Contact
{
    public interface IContactSearchResults
    {

    }

    public class ContactSearchResults
    {
        private readonly Search.ISearchModel _search;
        private readonly IEnumerable<Contract.Contacts.IContactQueryProvider> _queryProviders;

        public ContactSearchResults(Search.ISearchModel search, IEnumerable<Contract.Contacts.IContactQueryProvider> queryProviders)
        {
            _search = search;
            _queryProviders = queryProviders;

            //Subscribe to the searchRequester (switch) and populate the Model with the results.
        }

        //This is where the Model goes. Potentially just 
        //  ObsCol<IProviderDetails, string> Title
        //  ObsCol<IProviderDetails, kvp> Contacts -->ie <Google, <home email, lee@gmail.com>>
        //  ObsCol<IPD, exteded details?> ExtendedDetails -->ie Partner=Anne
    }
}

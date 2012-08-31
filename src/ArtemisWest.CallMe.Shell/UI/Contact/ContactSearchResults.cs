using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace ArtemisWest.CallMe.Shell.UI.Contact
{
    public class ContactSearchResults
    {
        private readonly Search.ISearchModel _search;
        private readonly IEnumerable<Contract.Contacts.IContactQueryProvider> _queryProviders;

        public ContactSearchResults(Search.ISearchModel search, IEnumerable<Contract.Contacts.IContactQueryProvider> queryProviders)
        {
            _search = search;
            _queryProviders = queryProviders;
            
            _search.IdentityActivated
                .Log("_search.IdentityActivated")
                .SelectMany(identity => 
                    _queryProviders.Select(
                        p =>
                        {
                            return p.Search(identity);
                        })
                        .Merge())
                .Subscribe(_contacts.Add);
        }

        //This is where the Model goes. Potentially just 
        //  ObsCol<IProviderDetails, string> Title
        //  ObsCol<IProviderDetails, kvp> Contacts -->ie <Google, <home email, lee@gmail.com>>
        //  ObsCol<IPD, exteded details?> ExtendedDetails -->ie Partner=Anne

        private readonly ObservableCollection<string> _contacts = new ObservableCollection<string>();

        public ObservableCollection<string> Contacts
        {
            get { return _contacts; }
        }
    }
}

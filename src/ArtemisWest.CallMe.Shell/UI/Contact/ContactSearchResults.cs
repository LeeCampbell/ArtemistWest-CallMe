using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using ArtemisWest.CallMe.Contract.Contacts;

namespace ArtemisWest.CallMe.Shell.UI.Contact
{
    public class ContactSearchResults
    {
        private readonly Search.ISearchModel _search;
        private readonly IEnumerable<IContactQueryProvider> _queryProviders;
        private readonly ObservableCollection<IContact> _contacts = new ObservableCollection<IContact>();

        public ContactSearchResults(Search.ISearchModel search, IEnumerable<IContactQueryProvider> queryProviders)
        {
            _search = search;
            _queryProviders = queryProviders;

            _search.IdentityActivated
                .Log("_search.IdentityActivated")
                .SelectMany(identity =>
                            _queryProviders.Select(p => p.Search(identity))
                                .Merge())
                .Select(contact => contact)
                .Subscribe(c =>
                               {
                                   _contacts.Add(c);
                               });
        }

        //This is where the Model goes. Potentially just 
        //  ObsCol<IProviderDetails, string> Title
        //  ObsCol<IProviderDetails, kvp> Contacts -->ie <Google, <home email, lee@gmail.com>>
        //  ObsCol<IPD, exteded details?> ExtendedDetails -->ie Partner=Anne
        
        public ObservableCollection<IContact> Contacts
        {
            get { return _contacts; }
        }
    }
}

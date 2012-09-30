using ArtemisWest.CallMe.Contract.Contacts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace ArtemisWest.CallMe.Shell.UI.Contact
{
    public class ContactSearchResults
    {
        private readonly IEnumerable<IContactQueryProvider> _queryProviders;
        private readonly ILogger _logger;
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly ObservableCollection<IContact> _contacts = new ObservableCollection<IContact>();

        public ContactSearchResults(Services.IActiveProfileService activeProfileService, 
            IEnumerable<IContactQueryProvider> queryProviders, 
            ILoggerFactory loggerFactory, 
            ISchedulerProvider schedulerProvider)
        {
            _queryProviders = queryProviders.ToArray();
            _logger = loggerFactory.GetLogger();
            _schedulerProvider = schedulerProvider;

            activeProfileService.ProfileActivated
                .Log(_logger, "activeProfileService.ProfileActivated")
                .SelectMany(identity =>
                            _queryProviders.Select(p => p.Search(identity))
                                           .Merge())
                .Select(contact => contact)
                .ObserveOn(_schedulerProvider.Dispatcher)
                .Subscribe(c => _contacts.Add(c));
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Practices.Prism.Logging;

namespace ArtemisWest.CallMe.Shell.Search
{
    public class SearchModel : ISearchModel
    {
        private readonly Subject<string> _identityQueries = new Subject<string>();
        private readonly Contract.IIdentityProvider[] _identityProviders;
        private readonly IObservable<Contract.IProfile> _identityActivated;

        public SearchModel(IEnumerable<Contract.IIdentityProvider> identityProviders, ILoggerFacade logger)
        {
            logger.Log("SearchModel ctor", Category.Debug, Priority.Low);
            _identityProviders = identityProviders.ToArray();

            var profiles = from query in _identityQueries
                                select _identityProviders.Select(ip => ip.FindProfile(query));

            //Merge the results from each of the identityProviders into a single sequence. 
            //Cancel any running queries if another request comes in (using the Switch Statement)
            _identityActivated = profiles
                .Select(queryResult => queryResult.Merge().ToList())
                .Switch()
                .Select(MergeProfiles)
                .Log("IdentityActivated");


            var msg = string.Format("SearchModel constructed with {0} identityProviders ({1})",
                                    _identityProviders.Length,
                                    string.Join(",", _identityProviders.Select(i => i.GetType().Name)));
            logger.Log(msg, Category.Debug, Priority.Medium);
        }

        public IObservable<Contract.IProfile> IdentityActivated
        {
            get { return _identityActivated; }
        }

        public void Search(string query)
        {
            //First get the Contact's identifiers.
            //e.g. if 0212543824 is recieved, Twitter want know what to do. 1st, push that to the ContactProviders/IdentityProviders?
            // With these results (wait for all to complete?) we then broadcast those identifiers out to the the providers
            _identityQueries.OnNext(query);
        }

        private static Contract.IProfile MergeProfiles(IEnumerable<Contract.IProfile> profiles)
        {
            return new Contract.Profile(profiles.SelectMany(p=>p.Identifiers));
        }
    }
}
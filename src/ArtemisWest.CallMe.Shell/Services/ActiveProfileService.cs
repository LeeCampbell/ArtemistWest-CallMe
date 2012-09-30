using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ArtemisWest.CallMe.Contract;

namespace ArtemisWest.CallMe.Shell.Services
{
    public class ActiveProfileService : IActiveProfileService
    {
        private readonly IEnumerable<IActivatedIdentityListener> _activatedIdentityListeners;
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly IObservable<IProfile> _profileActivated;
        private readonly IIdentityProvider[] _identityProviders;
        private readonly ILogger _logger;

        public ActiveProfileService(IEnumerable<IActivatedIdentityListener> activatedIdentityListeners,
            IEnumerable<Contract.IIdentityProvider> identityProviders, 
            ISchedulerProvider schedulerProvider,
            ILoggerFactory loggerFactory)
        {
            _activatedIdentityListeners = activatedIdentityListeners;
            _schedulerProvider = schedulerProvider;
            _logger = loggerFactory.GetLogger();
            
            _identityProviders = identityProviders.ToArray();

            var profiles = from listener in _activatedIdentityListeners.ToObservable()
                           from activation in listener.IdentitiesActivated(_schedulerProvider.ThreadPool)
                           select _identityProviders.Select(ip => ip.FindProfile(activation));

            //Merge the results from each of the identityProviders into a single sequence. 
            //Cancel any running queries if another request comes in (using the Switch Statement)
            _profileActivated = profiles
                .Select(queryResult => queryResult.Merge().ToList())
                .Switch()
                .Select(MergeProfiles)
                .Log(_logger, "IdentityActivated");
        }


        public IObservable<IProfile> ProfileActivated
        {
            get { return _profileActivated; }
        }

        private static Contract.IProfile MergeProfiles(IEnumerable<Contract.IProfile> profiles)
        {
            return new Contract.Profile(profiles.SelectMany(p => p.Identifiers));
        }
    }
}

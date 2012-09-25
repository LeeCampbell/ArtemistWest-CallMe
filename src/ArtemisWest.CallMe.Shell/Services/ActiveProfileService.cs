using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ArtemisWest.CallMe.Contract;
using Microsoft.Practices.Prism.Logging;

namespace ArtemisWest.CallMe.Shell.Services
{
    public class ActiveProfileService : IActiveProfileService
    {
        private readonly IEnumerable<IActivatedIdentityListener> _activatedIdentityListeners;
        private readonly IObservable<IProfile> _profileActivated;
        private readonly IIdentityProvider[] _identityProviders;

        public ActiveProfileService(IEnumerable<IActivatedIdentityListener> activatedIdentityListeners,
            IEnumerable<Contract.IIdentityProvider> identityProviders, 
            ILoggerFacade logger)
        {
            _activatedIdentityListeners = activatedIdentityListeners;

            
            _identityProviders = identityProviders.ToArray();

            var profiles = from listener in _activatedIdentityListeners.ToObservable()
                           from activation in listener.IdentitiesActivated
                           select _identityProviders.Select(ip => ip.FindProfile(activation));

            //Merge the results from each of the identityProviders into a single sequence. 
            //Cancel any running queries if another request comes in (using the Switch Statement)
            _profileActivated = profiles
                .Select(queryResult => queryResult.Merge().ToList())
                .Switch()
                .Select(MergeProfiles)
                .Log(logger, "IdentityActivated");
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

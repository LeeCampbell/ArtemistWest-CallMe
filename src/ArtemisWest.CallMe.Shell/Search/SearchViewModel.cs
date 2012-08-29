using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;

namespace ArtemisWest.CallMe.Shell.Search
{
    public sealed class SearchViewModel //: INotifyPropertyChanged
    {
        public SearchViewModel(ISearchModel model)
        {
            SearchCommand = new DelegateCommand(() => model.Search(SearchText));
        }
        
        public string SearchText { get; set; }
        public DelegateCommand SearchCommand { get; private set; }
    }

    public interface ISearchModel
    {
        IObservable<Contract.IProfile> IdentityActivated { get; }
        void Search(string query);
    }

    public class SearchModel : ISearchModel
    {
        private readonly Subject<string> _identityQueries = new Subject<string>();

        private readonly IEnumerable<Contract.IIdentityProvider> _identityProviders;
        private readonly IObservable<Contract.IProfile> _identityActivated;

        public SearchModel(IEnumerable<Contract.IIdentityProvider> identityProviders)
        {
            _identityProviders = identityProviders;

            //I want all results of the _identityProvider searches to be on one sequence (merged), I then want to stich on that.
            /*
             * 
             * idQuery  ------o
             * idP1     --------x
             * idp2
             * 
             * 
             * */

            

            var identityQuery = from query in _identityQueries
                                select  _identityProviders.Select(ip=>ip.FindRelatedIdentities(query));
            var currentIdentitySequence = identityQuery
                .Select(queryResult => queryResult.Merge().ToList())   //Merge the results from each of the identityProviders into a single sequence. **I assume that the Merge sequencce only complets once all have completd?**
                .Switch();//Cancel any runnig queries if another request comes in...
                

            //                    from ids in idProvider.Search(query)
            //                    select ids;
            //_identityActivated = iden
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
    }
}

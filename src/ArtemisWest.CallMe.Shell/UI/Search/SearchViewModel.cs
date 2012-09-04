using Microsoft.Practices.Prism.Commands;

namespace ArtemisWest.CallMe.Shell.UI.Search
{
    public sealed class SearchViewModel
    {
        public SearchViewModel(ISearchModel model)
        {
            SearchCommand = new DelegateCommand(() => model.Search(SearchText));
        }
        
        public string SearchText { get; set; }
        public DelegateCommand SearchCommand { get; private set; }
    }
}

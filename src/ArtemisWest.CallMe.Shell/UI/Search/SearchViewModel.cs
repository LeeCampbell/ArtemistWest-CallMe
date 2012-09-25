using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;

namespace ArtemisWest.CallMe.Shell.UI.Search
{
    public sealed class SearchViewModel : INotifyPropertyChanged
    {
        private string _searchText;

        public SearchViewModel(ISearchModel model)
        {
            SearchCommand = new DelegateCommand(() => model.Search(SearchText));
        }
        
        public string SearchText
        {
            get { return _searchText; }
            set 
            { 
                _searchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        public DelegateCommand SearchCommand { get; private set; }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}

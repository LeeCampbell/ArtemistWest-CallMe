using System.Windows.Controls;

namespace ArtemisWest.CallMe.Shell.UI.Search
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        private readonly SearchViewModel _viewModel;

        public SearchView()
        {
            InitializeComponent();
        }
        
        public SearchView(SearchViewModel viewModel)
        {
            viewModel.SearchText = "Dan Rowe";  //HACK:
            DataContext = _viewModel = viewModel;
            InitializeComponent();
        }
    }
}

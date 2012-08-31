using System.Windows.Controls;

namespace ArtemisWest.CallMe.Shell.UI.Contact
{
    /// <summary>
    /// Interaction logic for ContactSearchResultsView.xaml
    /// </summary>
    public partial class ContactSearchResultsView : UserControl
    {
        public ContactSearchResultsView()
        {
            InitializeComponent();
        }

        public ContactSearchResultsView(ContactSearchResults viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}

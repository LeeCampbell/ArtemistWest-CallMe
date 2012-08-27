using System.Windows.Controls;

namespace ArtemisWest.CallMe.Google.Authorization
{
    /// <summary>
    /// Interaction logic for GoogleLoginView.xaml
    /// </summary>
    public partial class GoogleLoginView : IGoogleLoginView
    {
        private readonly GoogleLoginViewModel _viewModel;

        public GoogleLoginView()
        {
            InitializeComponent();
        }

        public GoogleLoginView(GoogleLoginViewModel viewModel)
        {
            DataContext = _viewModel = viewModel;
            InitializeComponent();
        }

        public GoogleLoginViewModel ViewModel
        {
            get { return _viewModel; }
        }
    }
}

namespace ArtemisWest.CallMe.Shell.ProviderSettings
{
    public partial class ProviderSettingsView
    {
        public ProviderSettingsView()
        {
            InitializeComponent();
        }

        public ProviderSettingsView(ProviderSettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();  
        }
    }
}

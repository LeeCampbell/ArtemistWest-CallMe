namespace ArtemisWest.CallMe.Google.Authorization
{
    public interface IGoogleLoginView : Microsoft.Practices.Prism.IActiveAware
    {
        GoogleLoginViewModel ViewModel { get; }
    }
}
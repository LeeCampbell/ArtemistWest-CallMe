namespace ArtemisWest.CallMe.Contract
{
    public interface IPersonalIdentifier
    {
        IProviderDescription Provider { get; }
        string IdentifierType { get; }
        string Value { get; }
    }
}
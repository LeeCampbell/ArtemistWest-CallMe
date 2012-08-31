namespace ArtemisWest.CallMe.Contract
{
    public class PersonalIdentifier : IPersonalIdentifier
    {
        private readonly IProviderDescription _provider;
        private readonly string _identifierType;
        private readonly string _value;

        public PersonalIdentifier(IProviderDescription provider, string identifierType, string value)
        {
            _provider = provider;
            _identifierType = identifierType;
            _value = value;
        }

        public IProviderDescription Provider
        {
            get { return _provider; }
        }

        public string IdentifierType
        {
            get { return _identifierType; }
        }

        public string Value
        {
            get { return _value; }
        }
    }
}
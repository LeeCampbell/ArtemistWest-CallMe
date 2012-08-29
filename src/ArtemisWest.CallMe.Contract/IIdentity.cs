using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisWest.CallMe.Contract
{
    public interface IProfile
    {
        IList<IPersonalIdentifier> Identifiers { get; }
    }
    
    public interface IPersonalIdentifier
    {
        IProvider Provider { get; }
        string IdentifierType { get; }
        string Value { get; }
    }
    public class PersonalIdentifier : IPersonalIdentifier
    {
        private readonly IProvider _provider;
        private readonly string _identifierType;
        private readonly string _value;

        public PersonalIdentifier(IProvider provider, string identifierType, string value)
        {
            _provider = provider;
            _identifierType = identifierType;
            _value = value;
        }

        public IProvider Provider
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

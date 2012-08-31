using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ArtemisWest.CallMe.Contract
{
    public class Profile : IProfile
    {
        private readonly IList<IPersonalIdentifier> _identifiers;

        public Profile(IEnumerable<IPersonalIdentifier> identifiers)
        {
            _identifiers = new ReadOnlyCollection<IPersonalIdentifier>(identifiers.ToList());
        }

        public IList<IPersonalIdentifier> Identifiers
        {
            get { return _identifiers; }
        }

        public override string ToString()
        {
            return string.Join(";", _identifiers.Select(i => i.Value));
        }
    }
}
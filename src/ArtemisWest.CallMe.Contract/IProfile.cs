using System.Collections.Generic;

namespace ArtemisWest.CallMe.Contract
{
    public interface IProfile
    {
        IList<IPersonalIdentifier> Identifiers { get; }
    }
}
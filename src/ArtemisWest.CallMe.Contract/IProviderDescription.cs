using System;

namespace ArtemisWest.CallMe.Contract
{
    public interface IProviderDescription
    {
        string Name { get; }
        Uri Image { get; }
    }
}
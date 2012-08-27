using System;

namespace ArtemisWest.CallMe.Contract
{
    public interface IResourceScope
    {
        string Name { get; }
        Uri Resource { get; }
    }
}
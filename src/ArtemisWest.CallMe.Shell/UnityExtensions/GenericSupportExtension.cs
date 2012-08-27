using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

//Taken from http://pwlodek.blogspot.co.uk/2010/05/lazy-and-ienumerable-support-comes-to.html
namespace ArtemisWest.CallMe.Shell.UnityExtensions
{
    /// <summary>
    /// Adds <see cref="Lazy{T}"/> and <see cref="IEnumerable{T}"/> support for Unity.
    /// </summary>
    public class GenericSupportExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // IEnumerable<T> strategy
            Context.Strategies.AddNew<EnumerableResolutionStrategy>(
                UnityBuildStage.TypeMapping);

            // Lazy<T> policy
            Context.Policies.Set<IBuildPlanPolicy>(
                new LazyResolveBuildPlanPolicy(), typeof(Lazy<>));
        }
    }
}
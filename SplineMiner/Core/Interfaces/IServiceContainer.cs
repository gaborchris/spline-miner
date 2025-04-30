using System;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for a service container that manages dependency injection.
    /// </summary>
    public interface IServiceContainer
    {
        /// <summary>
        /// Registers a service implementation.
        /// </summary>
        /// <typeparam name="TService">The type of the service interface.</typeparam>
        /// <param name="implementation">The service implementation instance.</param>
        void RegisterService<TService>(TService implementation) where TService : class;

        /// <summary>
        /// Registers a singleton service implementation.
        /// </summary>
        /// <typeparam name="TService">The type of the service interface.</typeparam>
        /// <param name="implementation">The service implementation instance.</param>
        void RegisterSingleton<TService>(TService implementation) where TService : class;

        /// <summary>
        /// Gets a service implementation.
        /// </summary>
        /// <typeparam name="TService">The type of the service interface.</typeparam>
        /// <returns>The service implementation instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the service is not registered.</exception>
        TService GetService<TService>() where TService : class;
    }
} 
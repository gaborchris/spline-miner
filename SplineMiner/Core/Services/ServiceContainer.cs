using System;
using System.Collections.Generic;
using SplineMiner.Core.Interfaces;

namespace SplineMiner.Core.Services
{
    /// <summary>
    /// Implements a service container for dependency injection.
    /// </summary>
    public class ServiceContainer : IServiceContainer
    {
        private readonly Dictionary<Type, object> _services = [];
        private readonly Dictionary<Type, object> _singletons = [];

        /// <summary>
        /// Registers a service implementation.
        /// </summary>
        /// <typeparam name="TService">The type of the service interface.</typeparam>
        /// <param name="implementation">The service implementation instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when implementation is null.</exception>
        public void RegisterService<TService>(TService implementation) where TService : class
        {
            ArgumentNullException.ThrowIfNull(implementation);

            _services[typeof(TService)] = implementation;
        }

        /// <summary>
        /// Registers a singleton service implementation.
        /// </summary>
        /// <typeparam name="TService">The type of the service interface.</typeparam>
        /// <param name="implementation">The service implementation instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when implementation is null.</exception>
        public void RegisterSingleton<TService>(TService implementation) where TService : class
        {
            ArgumentNullException.ThrowIfNull(implementation);

            _singletons[typeof(TService)] = implementation;
        }

        /// <summary>
        /// Gets a service implementation.
        /// </summary>
        /// <typeparam name="TService">The type of the service interface.</typeparam>
        /// <returns>The service implementation instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the service is not registered.</exception>
        public TService GetService<TService>() where TService : class
        {
            if (_singletons.TryGetValue(typeof(TService), out var singleton))
                return (TService)singleton;

            if (_services.TryGetValue(typeof(TService), out var service))
                return (TService)service;

            throw new InvalidOperationException($"Service of type {typeof(TService)} not registered.");
        }
    }
}
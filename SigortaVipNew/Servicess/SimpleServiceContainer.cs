using System;
using System.Collections.Generic;
using SigortaVipNew.Helpers;

namespace SigortaVipNew.Servicess
{
    public class SimpleServiceContainer : IServiceContainer
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services;
        private readonly Dictionary<Type, object> _singletonInstances;

        public SimpleServiceContainer()
        {
            _services = new Dictionary<Type, ServiceDescriptor>();
            _singletonInstances = new Dictionary<Type, object>();
        }

        public void RegisterSingleton<TInterface, TImplementation>()
            where TImplementation : class, TInterface
        {
            _services[typeof(TInterface)] = new ServiceDescriptor
            {
                ServiceType = typeof(TInterface),
                ImplementationType = typeof(TImplementation),
                Lifetime = ServiceLifetime.Singleton
            };

            ErrorLogger.LogError($"Singleton registered: {typeof(TInterface).Name} -> {typeof(TImplementation).Name}");
        }

        public void RegisterTransient<TInterface, TImplementation>()
            where TImplementation : class, TInterface
        {
            _services[typeof(TInterface)] = new ServiceDescriptor
            {
                ServiceType = typeof(TInterface),
                ImplementationType = typeof(TImplementation),
                Lifetime = ServiceLifetime.Transient
            };

            ErrorLogger.LogError($"Transient registered: {typeof(TInterface).Name} -> {typeof(TImplementation).Name}");
        }

        public void RegisterInstance<TInterface>(TInterface instance)
        {
            _services[typeof(TInterface)] = new ServiceDescriptor
            {
                ServiceType = typeof(TInterface),
                Instance = instance,
                Lifetime = ServiceLifetime.Singleton
            };

            _singletonInstances[typeof(TInterface)] = instance;
            ErrorLogger.LogError($"Instance registered: {typeof(TInterface).Name}");
        }

        public TInterface Resolve<TInterface>()
        {
            return (TInterface)Resolve(typeof(TInterface));
        }

        public object Resolve(Type serviceType)
        {
            try
            {
                if (!_services.TryGetValue(serviceType, out ServiceDescriptor descriptor))
                {
                    throw new InvalidOperationException($"Service {serviceType.Name} is not registered");
                }

                if (descriptor.Instance != null)
                {
                    return descriptor.Instance;
                }

                if (descriptor.Lifetime == ServiceLifetime.Singleton)
                {
                    if (_singletonInstances.TryGetValue(serviceType, out object singletonInstance))
                    {
                        return singletonInstance;
                    }

                    var newInstance = CreateInstance(descriptor.ImplementationType);
                    _singletonInstances[serviceType] = newInstance;
                    return newInstance;
                }

                return CreateInstance(descriptor.ImplementationType);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, $"Service resolve hatası: {serviceType.Name}");
                throw;
            }
        }

        public bool IsRegistered<TInterface>()
        {
            return _services.ContainsKey(typeof(TInterface));
        }

        public void Clear()
        {
            _services.Clear();
            _singletonInstances.Clear();
            ErrorLogger.LogError("Service container cleared");
        }

        private object CreateInstance(Type implementationType)
        {
            try
            {
                return Activator.CreateInstance(implementationType);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, $"Instance oluşturma hatası: {implementationType.Name}");
                throw new InvalidOperationException($"Cannot create instance of {implementationType.Name}", ex);
            }
        }

        private class ServiceDescriptor
        {
            public Type ServiceType { get; set; }
            public Type ImplementationType { get; set; }
            public object Instance { get; set; }
            public ServiceLifetime Lifetime { get; set; }
        }

        private enum ServiceLifetime
        {
            Transient,
            Singleton
        }
    }
}
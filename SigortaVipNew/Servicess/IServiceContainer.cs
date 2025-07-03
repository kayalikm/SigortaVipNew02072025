using System;

namespace SigortaVipNew.Servicess  // Servicess (çift s) olarak değiştirin
{
    public interface IServiceContainer
    {
        void RegisterSingleton<TInterface, TImplementation>()
            where TImplementation : class, TInterface;

        void RegisterTransient<TInterface, TImplementation>()
            where TImplementation : class, TInterface;

        void RegisterInstance<TInterface>(TInterface instance);

        TInterface Resolve<TInterface>();

        object Resolve(Type serviceType);

        bool IsRegistered<TInterface>();

        void Clear();
    }
}
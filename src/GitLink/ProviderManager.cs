﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProviderManager.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitLink.Providers
{
    using Catel.IoC;
    using Catel.Reflection;

    public class ProviderManager : IProviderManager
    {
        public ProviderBase GetProvider(string url)
        {
            var providerTypes = TypeCache.GetTypes(x => typeof(ProviderBase).IsAssignableFromEx(x) && !x.IsAbstract && x != typeof(CustomRawUrlProvider) && x != typeof(CustomUrlProvider));

            var typeFactory = TypeFactory.Default;

            var customUrlProvider = typeFactory.CreateInstance<CustomUrlProvider>();
            if (customUrlProvider.Initialize(url))
            {
                return customUrlProvider;
            }

            foreach (var providerType in providerTypes)
            {
                var provider = (ProviderBase) typeFactory.CreateInstance(providerType);
                if (provider.Initialize(url))
                {
                    return provider;
                }
            }            

            var customProvider = typeFactory.CreateInstance<CustomRawUrlProvider>();
            if (customProvider.Initialize(url))
            {
                return customProvider;
            }

           return null;
        }
    }
}
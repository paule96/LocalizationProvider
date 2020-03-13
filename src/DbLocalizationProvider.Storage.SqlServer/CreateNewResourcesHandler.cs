// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Commands;

namespace DbLocalizationProvider.Storage.SqlServer
{
    /// <summary>
    /// Implementation of the command to create new resources
    /// </summary>
    public class CreateNewResourcesHandler : ICommandHandler<CreateNewResources.Command>
    {
        /// <summary>
        /// Handles the command. Actual instance of the command being executed is passed-in as argument
        /// </summary>
        /// <param name="command">Actual command instance being executed</param>
        /// <exception cref="InvalidOperationException">Resource with key `{resource.ResourceKey}` already exists</exception>
        public void Execute(CreateNewResources.Command command)
        {
            if (command.LocalizationResources == null || !command.LocalizationResources.Any()) return;

            var repo = new ResourceRepository();

            foreach (var resource in command.LocalizationResources)
            {
                var existingResource = repo.GetByKey(resource.ResourceKey);

                if (existingResource != null)
                {
                    throw new InvalidOperationException($"Resource with key `{resource.ResourceKey}` already exists");
                }

                resource.ModificationDate = DateTime.UtcNow;
                repo.InsertResource(resource);

                ConfigurationContext.Current.BaseCacheManager.StoreKnownKey(resource.ResourceKey);
            }
        }
    }
}

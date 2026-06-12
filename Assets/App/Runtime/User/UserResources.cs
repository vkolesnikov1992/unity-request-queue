using System;
using UnityEngine.Scripting;

namespace UnityRequestQueue.Runtime.User
{
    public sealed class UserResources
    {
        [Preserve]
        public UserResources(UserResourcesConfig config)
        {
            if (!config)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Currency = new UserResource(config.InitialCurrency);
            Energy = new UserResource(config.InitialEnergy, config.MaxEnergy);
        }

        public UserResource Currency { get; }

        public UserResource Energy { get; }
    }
}

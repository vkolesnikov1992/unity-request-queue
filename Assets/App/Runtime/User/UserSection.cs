using System;
using UnityEngine.Scripting;
using UnityRequestQueue.Runtime.Sections;

namespace UnityRequestQueue.Runtime.User
{
    public sealed class UserSection : IAppSection
    {
        [Preserve]
        public UserSection(UserResources resources)
        {
            Resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public UserResources Resources { get; }
    }
}

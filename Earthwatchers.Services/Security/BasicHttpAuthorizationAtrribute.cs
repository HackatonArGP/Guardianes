using System;
using Earthwatchers.Models;

namespace Earthwatchers.Services.Security
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BasicHttpAuthorizationAttribute : Attribute
    {
        public Role Role { get; set;}

        public BasicHttpAuthorizationAttribute(Role role)
        {
            this.Role = role;
        }

    }
}
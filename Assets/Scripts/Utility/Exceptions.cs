using System;

namespace Utility
{
    public class NetworkConfigurationError : Exception
    {
        public NetworkConfigurationError(string message) : base(message) {}
    }
}
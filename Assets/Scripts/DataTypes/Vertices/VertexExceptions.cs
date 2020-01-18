using System;

namespace DataTypes
{
    class WrongAnchorCount : Exception
    {
        public WrongAnchorCount() {}
        public WrongAnchorCount(string message) : base(message) {}
        public WrongAnchorCount(string message, Exception innerException) : base (message, innerException) {}
    }
}
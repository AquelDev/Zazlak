using System;

namespace Zazlak.Storage
{
    [Serializable()]
    public class DatabaseException : Exception 
    {
        internal DatabaseException(string sMessage) : base(sMessage) { }
    }
}

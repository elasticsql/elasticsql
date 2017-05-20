using System;
using System.Runtime.Serialization;

namespace ESQLWebSericeCSharpAPI.classes.exceptions
{
    [Serializable]
    public class ExceptionDuringDisconnect : Exception
    {
        public ExceptionDuringDisconnect(string message) : base(message)
        { }

        protected ExceptionDuringDisconnect(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
        { }
    }
}

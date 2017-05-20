using System;
using System.Runtime.Serialization;

namespace ESQLWebSericeCSharpAPI.classes.exceptions
{
    [Serializable]
    public class ExceptionDuringConnect : Exception
    {
        public ExceptionDuringConnect(string message) : base(message)
        { }

        protected ExceptionDuringConnect(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
        { }
    }
}

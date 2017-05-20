using System;
using System.Runtime.Serialization;

namespace ESQLWebSericeCSharpAPI.classes.exceptions
{
    [Serializable]
    public class ExceptionServiceTimeout : Exception
    {
        public ExceptionServiceTimeout(string message) : base(message)
        { }

        protected ExceptionServiceTimeout(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
        { }
    }
}

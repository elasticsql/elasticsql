using System;
using System.Runtime.Serialization;
namespace ESQLWebSericeCSharpAPI.classes.exceptions
{
    [Serializable]
    public class ExceptionDuringReset : Exception
    {
        public ExceptionDuringReset(string message) : base(message)
        { }

        protected ExceptionDuringReset(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
        { }
    }
}

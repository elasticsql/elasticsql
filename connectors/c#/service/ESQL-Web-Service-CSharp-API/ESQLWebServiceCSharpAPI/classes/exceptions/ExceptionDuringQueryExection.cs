using System;
using System.Runtime.Serialization;

namespace ESQLWebSericeCSharpAPI.classes.exceptions
{
    /// <summary>
    /// Common exception for all types of errors during code execution. 
    /// The message of exception will be generated with concatanation with message and code from JSON Service.
    /// </summary>
    [Serializable]
    public class ExceptionDuringQueryExection : Exception
    {
        public ExceptionDuringQueryExection(string message) : base(message)
        { }

        protected ExceptionDuringQueryExection(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
        { }
    }
}

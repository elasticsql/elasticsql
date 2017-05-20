using System;
using System.Runtime.Serialization;

namespace ESQLWebSericeCSharpAPI.classes.exceptions
{
    /// <summary>
    /// Exception that is thrown when ESQL Web Service is not avaliable.
    /// </summary>
    [Serializable]
    public class ExceptionDuringTokenRenewal : Exception
    {
        public ExceptionDuringTokenRenewal(string message) : base(message)
        { }

        protected ExceptionDuringTokenRenewal(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
        { }
    }
}

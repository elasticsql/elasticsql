using System;
using System.Runtime.Serialization;

namespace ESQLWebSericeCSharpAPI.classes.exceptions
{
    /// <summary>
    /// Exception that is thrown when ESQL Web Service is not avaliable.
    /// </summary>
    [Serializable]
    public class ExceptionServiceNotAvaliable : Exception
    {
        public ExceptionServiceNotAvaliable(string message) : base(message)
        { }

        protected ExceptionServiceNotAvaliable(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
        { }
    }
}

using System;
using System.Runtime.Serialization;

namespace ESQLWebSericeCSharpAPI.classes.exceptions
{
    /// <summary>
    /// Wrong credentials exception, will be thrown in case of bad user and password combination.
    /// </summary>
    [Serializable]
    public class ExceptionWrongCredentials : Exception
    {
        public ExceptionWrongCredentials(string message) : base(message)
        { }

        protected ExceptionWrongCredentials(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
        { }
    }
}

namespace InsightArchitectures.Utilities;

/// <summary>
/// An exception class for <see cref="SigV4MessageHandler"/>.
/// </summary>
[Serializable]
public class SigV4MessageHandlerException : Exception
{
    /// <summary>
    /// An exception class for <see cref="SigV4MessageHandler"/>.
    /// </summary>
    /// <param name="message"></param>
    public SigV4MessageHandlerException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// An exception class for <see cref="SigV4MessageHandler"/>.
    /// </summary>
    public SigV4MessageHandlerException()
    {
    }

    /// <summary>
    /// An exception class for <see cref="SigV4MessageHandler"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public SigV4MessageHandlerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SigV4MessageHandlerException"/> class.
    /// </summary>
    /// <param name="serializationInfo"></param>
    /// <param name="streamingContext"></param>
    protected SigV4MessageHandlerException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
    {
        throw new NotImplementedException();
    }
}

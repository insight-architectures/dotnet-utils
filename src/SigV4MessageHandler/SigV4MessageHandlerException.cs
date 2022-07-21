namespace InsightArchitectures.Utilities;

/// <summary>
/// An exception class for <see cref="SigV4MessageHandler"/>.
/// </summary>
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
}

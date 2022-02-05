namespace ISO22900.II
{
    public class Iso22900IIException : Iso22900IIExceptionBase
    {
        internal Iso22900IIException(string message, PduError error)
            : base(message + $" [{error}]")
        {
            PduError = error;
        }
    }
}

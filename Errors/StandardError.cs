namespace fidelizPlus_back.Errors
{
    public class StandardError : Error
    {
        private LogContext logContext;

        public StandardError(LogContext logContext)
        {
            this.logContext = logContext;
        }

        public void Throw(object content, int status = 500)
        {
            throw new AppException(content, status, logContext);
        }

        public T TypedThrow<T>(object content, int status = 500)
        {
            throw new AppException(content, status, logContext);
        }
    }
}

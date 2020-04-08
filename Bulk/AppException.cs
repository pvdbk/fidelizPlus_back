using System;

namespace fidelizPlus_back
{
    public class AppException : Exception
    {
        public int Status { get; }
        public object Content { get; }

        public AppException(object content, int status = 500) : base()
        {
            Content = content;
            Status = status;
        }

        public T Cast<T>()
        {
            throw this;
        }
    }
}

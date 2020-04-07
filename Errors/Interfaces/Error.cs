namespace fidelizPlus_back.Errors
{
    public interface Error
    {
        public void Throw(object content, int status = 500);

        public T TypedThrow<T>(object content, int status = 500);
    }
}

namespace SF.Core.Message
{
    public class MessageArgs<T>
    {
        public T Item { get; set; }
        public MessageArgs(T item)
        {
            Item = item;
        }
    }
}
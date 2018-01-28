namespace Composite.Cs.Interfaces
{
    public class CompositeValue<T, V> : CompositeCs<T, V>
    {
        public CompositeValue(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}

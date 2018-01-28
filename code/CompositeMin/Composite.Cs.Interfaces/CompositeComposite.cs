namespace Composite.Cs.Interfaces
{
    public class CompositeComposite<T, V> : CompositeCs<T, V>
    {
        public CompositeComposite(V value)
        {
            Value = value;
        }

        public V Value { get; }
    }
}

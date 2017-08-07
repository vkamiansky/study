namespace ImageEditor.ViewModel.model
{
    public static class ArrayUtils
    {
        public static T[] CloneArray<T>(this T[] t)
        {
            T[] clone = new T[t.Length];
            for (var i = 0; i < t.Length; i++)
            {
                clone[i] = t[i];
            }
            return clone;
        }

        public static void Fill<T>(this T[] t, T value)
        {
            for (var i = 0; i < t.Length; i++)
            {
                t[i] = value;
            }
        }
    }
}
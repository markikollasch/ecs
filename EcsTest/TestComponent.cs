namespace EcsTest
{
    public struct Component<T>
    {
        public T Value { get; }

        public Component(T value)
        {
            Value = value;
        }

        public static implicit operator T(Component<T> component)
        {
            return component.Value;
        }

        public static implicit operator Component<T>(T value)
        {
            return new Component<T>(value);
        }
    }

    public static class Component
    {
        public static Component<T> Of<T>(T value)
        {
            return new Component<T>(value);
        }
    }
}

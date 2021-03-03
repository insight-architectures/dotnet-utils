namespace Tests
{
    public class TypeWithProperty<TProperty>
    {
        public TProperty Property { get; set; }
    }

    public class TypeWithMultipleProperties<TProperty1, TProperty2>
    {
        public TProperty1 FirstProperty { get; set; }

        public TProperty2 SecondProperty { get; set; }
    }
}

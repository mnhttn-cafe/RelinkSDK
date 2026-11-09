namespace RelinkSDK.Attributes;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class ArrayLengthAttribute : Attribute
{
    private readonly int _length;

    public ArrayLengthAttribute(int length)
    {
        _length = length;
    }
}

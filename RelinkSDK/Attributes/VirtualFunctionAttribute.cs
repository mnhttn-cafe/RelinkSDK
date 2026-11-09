namespace RelinkSDK.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal sealed class VirtualFunctionAttribute : Attribute
{
    private readonly int _slot;

    public VirtualFunctionAttribute(int slot)
    {
        _slot = slot;
    }
}

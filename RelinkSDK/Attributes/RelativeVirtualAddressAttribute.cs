namespace RelinkSDK.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
internal sealed class RelativeVirtualAddressAttribute : Attribute
{
    private uint _address;
    
    public RelativeVirtualAddressAttribute(uint address)
    {
        _address = address;
    }
}
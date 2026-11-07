using System;
using System.Diagnostics;

namespace RelinkSDK.Attributes;

[AttributeUsage(AttributeTargets.Struct, AllowMultiple = true)]
[Conditional("RELINKSDK_GENERATOR")]
internal sealed class BaseAttribute : Attribute
{
    private readonly Type _type;
    private readonly uint _offset;

    public BaseAttribute(Type type, uint offset = 0x0)
    {
        _type = type;
        _offset = offset;
    }
}
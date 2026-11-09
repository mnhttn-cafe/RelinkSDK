using System.Runtime.InteropServices;

namespace RelinkSDK.Bindings.Entities;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EntityHandle
{
    public uint Index;
    public Entity* Entity;
    public ulong Stamp;

    public bool IsEmpty => Index == 0;
}

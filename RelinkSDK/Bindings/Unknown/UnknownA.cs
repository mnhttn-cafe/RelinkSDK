using System.Runtime.InteropServices;

namespace RelinkSDK.Bindings.Unknown;

/// <summary>
/// No idea what this is, but it honestly looks like a struct lmao
///
/// [mov rax, [rdi+0x28]]
/// [mov [rax+0x18], rbx]
///
/// ...the real question is, why is the entity's handle almost 3 classes deep?
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe struct UnknownA
{
    [FieldOffset(0x18)] public Entities.Entity* Entity;
}
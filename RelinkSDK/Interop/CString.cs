using System.Runtime.InteropServices;
using System.Text;

namespace RelinkSDK.Interop;

/// <summary>
/// Unmanaged representation of a C string (const char*)
/// </summary>
public unsafe struct CString
{
    private readonly byte* _string;
    
    public override string ToString()
    {
        return Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(_string));
    }
}
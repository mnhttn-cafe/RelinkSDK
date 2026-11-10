using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RelinkSDK.Standard;

/// <summary>
/// Unmanaged representation of C++'s std::vector class in C#.
/// </summary>
/// <typeparam name="T">The type this vector can take.</typeparam>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Vector<T> where T : unmanaged
{
    private T* _first;
    private T* _last;
    private T* _end;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nuint Size()
    {
        if (_first == null)
        {
            return 0;
        }

        return (nuint)(_last - _first);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan()
    {
        if (_first == null)
        {
            return Span<T>.Empty;
        }
        ;

        return new Span<T>(_first, (int)Size());
    }
    
    public Span<T>.Enumerator GetEnumerator() => AsSpan().GetEnumerator();
}
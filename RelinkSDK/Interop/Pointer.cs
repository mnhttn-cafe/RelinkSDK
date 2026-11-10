namespace RelinkSDK.Interop;

public unsafe struct Pointer<T> where T : unmanaged
{
    public T* Value;

    public static implicit operator T*(Pointer<T> pointer) => pointer.Value;
}
using System.Text;

namespace RelinkSDK.Interop;

public static unsafe class Runtime
{
    public static T* DynamicCast<T>(void* pointer) where T : unmanaged
    {
        if (pointer == null)
            return null;

        byte* image = (byte*)Game.Instance.BaseAddress;
        int* locator = *(int**)(*(byte**)pointer - 8);
        int* hierarchy = (int*)(image + locator[4]);
        int* bases = (int*)(image + hierarchy[3]);
        byte* root = (byte*)pointer - locator[1];

        byte[] target = TypeName<T>.Prefix;

        for (int i = 0; i < hierarchy[2]; i++)
        {
            int* descriptor = (int*)(image + bases[i]);
            var name = new ReadOnlySpan<byte>(image + descriptor[0] + 0x10, target.Length);

            if (name.SequenceEqual(target))
            {
                return (T*)(root + descriptor[2]);
            }
        }

        return null;
    }

    private static class TypeName<T>
    {
        public static readonly byte[] Prefix = Encoding.ASCII.GetBytes($".?AV{typeof(T).Name}@");
    }
}

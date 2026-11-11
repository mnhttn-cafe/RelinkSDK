using System.Diagnostics;
using System.Security.Cryptography;

namespace RelinkSDK;

public class Game
{
    private const string CURRENT_SUPPORTED_SHA = "4b3ace0fd03df5d3d08c6453e12df2279c6259abf7d2fb3622921fcbea775145";

    private static Game? _instance;

    /// <summary>
    /// The shared <see cref="Game" /> instance.
    /// </summary>
    public static Game Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Game();
            }

            return _instance;
        }
    }

    private readonly Dictionary<string, nint> _vtables = new();
    private ProcessModule _module;
    private nint _baseAddress;

    /// <summary>
    /// Whether this version of the SDK supports the current version of the game.
    /// </summary>
    public Boolean IsSupported { get; private set; } = true;

    /// <summary>
    /// The base address of the game.
    /// </summary>
    public nint BaseAddress => _baseAddress;

    private Game()
    {
        _module = Process.GetCurrentProcess().MainModule!;
        _baseAddress = _module.BaseAddress;

        using (FileStream stream = File.OpenRead(_module.FileName))
        {
            string moduleHash = Convert.ToHexString(SHA256.HashData(stream));

            if (!moduleHash.Equals(CURRENT_SUPPORTED_SHA, StringComparison.OrdinalIgnoreCase))
            {
                // this version of the sdk doesn't support
                // this version of the game
                IsSupported = false;
            }
        }

    }

    public nint AddressFor(nint rva) => _baseAddress + rva;
    public unsafe nint VirtualFunction(string className, int slot) => ((nint*)VirtualTable(className))[slot];

    public unsafe nint VirtualTable(string className)
    {
        if (_vtables.TryGetValue(className, out nint cached))
        {
            return cached;
        }

        var image = new ReadOnlySpan<byte>((void*)_baseAddress, _module.ModuleMemorySize);

        int name = image.IndexOf(System.Text.Encoding.ASCII.GetBytes($".?AV{className}@@\0"));
        if (name < 0)
        {
            name = FindScopedTypeName(image, className);
        }

        uint descriptor = (uint)(name - 0x10);
        int locator = -1;

        for (int at = 0, found; (found = image[at..].IndexOf(BitConverter.GetBytes(descriptor))) >= 0; at += found + 4)
        {
            int candidate = at + found - 0xC;

            if (candidate >= 0
                && BitConverter.ToUInt32(image[candidate..]) == 1
                && BitConverter.ToUInt32(image[(candidate + 0x4)..]) == 0
                && BitConverter.ToUInt32(image[(candidate + 0x14)..]) == (uint)candidate)
            {
                locator = candidate;
                break;
            }
        }

        if (locator < 0)
        {
            throw new KeyNotFoundException($"No complete object locator for {className}");
        }

        int pointer = image.IndexOf(BitConverter.GetBytes((long)(_baseAddress + locator)));
        if (pointer < 0)
        {
            throw new KeyNotFoundException($"No virtual table for {className}");
        }

        nint vtable = _baseAddress + pointer + 8;
        _vtables[className] = vtable;
        return vtable;
    }

    private static int FindScopedTypeName(ReadOnlySpan<byte> image, string className)
    {
        byte[] prefix = System.Text.Encoding.ASCII.GetBytes($".?AV{className}@");
        var names = new List<string>();
        int match = -1;

        for (int at = 0, found; (found = image[at..].IndexOf(prefix)) >= 0; at += found + prefix.Length)
        {
            int start = at + found;
            int end = start + prefix.Length;

            while (end < image.Length && IsNameChar(image[end]))
            {
                end++;
            }

            if (end >= image.Length || image[end] != 0 || image[end - 1] != '@' || image[end - 2] != '@')
            {
                continue;
            }

            match = start;
            names.Add(System.Text.Encoding.ASCII.GetString(image[(start + 4)..(end - 2)]));
        }

        if (names.Count == 0)
        {
            throw new KeyNotFoundException($"No type descriptor for {className}");
        }

        if (names.Count > 1)
        {
            throw new KeyNotFoundException($"{className} is ambiguous, use one of: {string.Join(", ", names)}");
        }

        return match;
    }

    private static bool IsNameChar(byte c) => char.IsAsciiLetterOrDigit((char)c) || c == '_' || c == '@';

    /// <summary>
    /// Whether <paramref name="address" /> is a valid address inside the game.
    /// </summary>
    public bool Contains(nint address) => address >= _baseAddress && address < _baseAddress + _module.ModuleMemorySize;
}
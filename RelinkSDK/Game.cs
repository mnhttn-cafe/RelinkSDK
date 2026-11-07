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

    /// <summary>
    /// Whether <paramref name="address" /> is a valid address inside the game.
    /// </summary>
    public bool Contains(nint address) => address >= _baseAddress && address < _baseAddress + _module.ModuleMemorySize;
}
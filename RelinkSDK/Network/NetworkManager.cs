using RelinkSDK.Bindings.Network;

namespace RelinkSDK.Network;

public unsafe class NetworkManager
{
    internal NetworkManager()
    {
    }

    public bool IsOnline
    {
        get
        {
            var session = NetworkSession.Instance;
            return session != null && session->State == 3;
        }
    }

    public int LocalSlot => IsOnline ? NetworkSession.Instance->LocalSlot() : 0;

    public bool IsLocalAuthority(Bindings.Entities.Entity* entity) => entity->IsLocalAuthority();
}

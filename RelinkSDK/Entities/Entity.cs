using RelinkSDK.Bindings.Entities;

namespace RelinkSDK.Entities;

public abstract unsafe class Entity
{
    private readonly EntityHandle _handle;

    public Bindings.Entities.Entity* Self => EntityHandleManager.Instance->Resolve(_handle);
    public bool IsStale => Self == null;

    protected Entity(EntityHandle handle)
    {
        _handle = handle;
    }

    public bool TryResolve(out Bindings.Entities.Entity* entity)
    {
        entity = EntityHandleManager.Instance->Resolve(_handle);
        return entity != null;
    }
}

public unsafe class Entity<T> : Entity where T : unmanaged
{
    public T* Behavior => !IsStale ? (T*)Self->Behavior : null;

    public Entity(EntityHandle handle) : base(handle)
    {
    }
}

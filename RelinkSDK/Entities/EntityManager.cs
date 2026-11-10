using Reloaded.Hooks.Definitions;
using RelinkSDK.Bindings.Entities;
using RelinkSDK.Interop;

namespace RelinkSDK.Entities;

public unsafe class EntityManager
{
    private readonly IHook<Bindings.Entities.EntityManager.CreateFn> _createEntity;
    private readonly IHook<Bindings.Entities.EntityManager.DestroyFn> _destroyEntity;

    private readonly object _lock = new();
    private readonly Dictionary<string, Func<EntityHandle, Entity>> _factories = new();
    private readonly Dictionary<(uint, ulong), Entity> _entities = new();

    internal EntityManager(IReloadedHooks hooks)
    {
        _createEntity = Bindings.Entities.EntityManager.HookCreate(hooks, CreateEntity);
        _destroyEntity = Bindings.Entities.EntityManager.HookDestroy(hooks, DestroyEntity);
    }

    public void Register<TEntity>(string className, Func<EntityHandle, TEntity> create) where TEntity : Entity
    {
        lock (_lock)
        {
            _factories[className] = create;
        }
    }

    public bool TryGet<TEntity>(EntityHandle handle, out TEntity entity) where TEntity : Entity
    {
        lock (_lock)
        {
            if (_entities.TryGetValue((handle.Index, handle.Stamp), out var found) && found is TEntity typed)
            {
                entity = typed;
                return true;
            }
        }

        entity = null!;
        return false;
    }

    public List<TEntity> All<TEntity>() where TEntity : Entity
    {
        lock (_lock)
        {
            return _entities.Values.OfType<TEntity>().ToList();
        }
    }

    private Bindings.Entities.Entity* CreateEntity(Bindings.Entities.EntityManager* self, void* name, void* description, void* unknown)
    {
        var entity = _createEntity.OriginalFunction(self, name, description, unknown);

        if (entity != null && entity->Behavior != null)
        {
            Track(entity);
        }

        return entity;
    }

    private void DestroyEntity(Bindings.Entities.Entity* entity)
    {
        if (entity != null)
        {
            Untrack(entity);
        }

        _destroyEntity.OriginalFunction(entity);
    }

    private void Track(Bindings.Entities.Entity* entity)
    {
        CString name;
        entity->Behavior->GetTypeName(&name);
        string className = name.ToString();

        Entity created;

        lock (_lock)
        {
            if (!_factories.TryGetValue(className, out var create))
            {
                return;
            }

            created = create(entity->Handle);
            _entities[(entity->Handle.Index, entity->Handle.Stamp)] = created;
        }
    }

    private void Untrack(Bindings.Entities.Entity* entity)
    {
        // TODO: find sth to do with this lol
        Entity? removed;

        lock (_lock)
        {
            if (!_entities.Remove((entity->Handle.Index, entity->Handle.Stamp), out removed))
            {
                return;
            }
        }
    }
}

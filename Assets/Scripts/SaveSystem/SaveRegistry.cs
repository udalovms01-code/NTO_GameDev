using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    public class SaveRegistry
    {
        private readonly Dictionary<string, SaveableEntity> _entities = new Dictionary<string, SaveableEntity>();

        public IEnumerable<SaveableEntity> RegisteredEntities => _entities.Values;

        public bool Register(SaveableEntity entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                Debug.LogWarning($"Cannot register entity without id on {entity.gameObject.name}");
                return false;
            }

            if (_entities.ContainsKey(entity.Id))
            {
                Debug.LogWarning($"Duplicate saveable id '{entity.Id}' detected on {entity.gameObject.name}");
                return false;
            }

            _entities[entity.Id] = entity;
            return true;
        }

        public void Unregister(SaveableEntity entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                return;
            }

            if (_entities.TryGetValue(entity.Id, out var registered) && registered == entity)
            {
                _entities.Remove(entity.Id);
            }
        }

        public SaveableEntity Find(string id)
        {
            _entities.TryGetValue(id, out var entity);
            return entity;
        }
    }
}

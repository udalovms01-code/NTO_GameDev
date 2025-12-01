using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SaveSystem
{
    public interface ISaveDataSource
    {
        void Capture(SaveDataContainer container);
        void Restore(SaveDataContainer container);
    }

    public class SaveManager : IInitializable, IDisposable
    {
        public const string CurrentVersion = "1.0.0";
        public const string DefaultSlot = "autosave";

        private readonly SaveRegistry _registry;
        private readonly SaveFileStorage _storage;
        private readonly List<ISaveDataSource> _dataSources;

        public SaveManager(SaveRegistry registry, SaveFileStorage storage, List<ISaveDataSource> dataSources)
        {
            _registry = registry;
            _storage = storage;
            _dataSources = dataSources ?? new List<ISaveDataSource>();
        }

        public void Initialize()
        {
            Debug.Log("SaveManager initialized. Ready to capture game state.");
        }

        public void Dispose()
        {
            Debug.Log("SaveManager disposed.");
        }

        public async Task SaveAsync(string slotName = DefaultSlot)
        {
            var container = new SaveDataContainer
            {
                Version = CurrentVersion,
                SavedAtTicks = DateTime.UtcNow.Ticks,
                ActiveScene = SceneManager.GetActiveScene().name
            };

            foreach (var dataSource in _dataSources)
            {
                try
                {
                    dataSource.Capture(container);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"Save data source {dataSource.GetType().Name} failed: {exception.Message}");
                }
            }

            foreach (var entity in _registry.RegisteredEntities.ToList())
            {
                container.Entities.Add(entity.CaptureState(container.ActiveScene));
            }

            await _storage.WriteAsync(slotName, container);
            Debug.Log($"Game saved to slot '{slotName}'.");
        }

        public async Task<bool> LoadAsync(string slotName = DefaultSlot)
        {
            var container = await _storage.ReadAsync(slotName);
            if (container == null)
            {
                return false;
            }

            if (!string.Equals(container.Version, CurrentVersion, StringComparison.Ordinal))
            {
                Debug.LogWarning($"Save version mismatch. Expected {CurrentVersion} but found {container.Version}");
            }

            foreach (var dataSource in _dataSources)
            {
                try
                {
                    dataSource.Restore(container);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"Load data source {dataSource.GetType().Name} failed: {exception.Message}");
                }
            }

            foreach (var entityState in container.Entities)
            {
                var entity = _registry.Find(entityState.Id);
                if (entity == null)
                {
                    Debug.LogWarning($"No entity found with id {entityState.Id}. It might need to be spawned from prefab {entityState.PrefabId}.");
                    continue;
                }

                entity.Restore(entityState);
            }

            Debug.Log($"Game loaded from slot '{slotName}'.");
            return true;
        }

        public bool Delete(string slotName)
        {
            return _storage.Delete(slotName);
        }
    }
}

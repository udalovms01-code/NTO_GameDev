using System;
using UnityEngine;
using Zenject;

namespace SaveSystem
{
    public interface ISavePayloadProvider
    {
        void Capture(EntityStateData data);
        void Restore(EntityStateData data);
    }

    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField] private string _prefabId;
        [SerializeField] private bool _persistAcrossScenes;

        public string Id => _id;
        public string PrefabId => _prefabId;
        public bool PersistAcrossScenes => _persistAcrossScenes;

        private SaveRegistry _registry;

        [Inject]
        private void Construct(SaveRegistry registry)
        {
            _registry = registry;
        }

        private void OnEnable()
        {
            _registry?.Register(this);
        }

        private void OnDisable()
        {
            _registry?.Unregister(this);
        }

        public EntityStateData CaptureState(string sceneName)
        {
            var data = new EntityStateData
            {
                Id = _id,
                PrefabId = _prefabId,
                SceneName = sceneName,
                Position = transform.position
            };

            foreach (var provider in GetComponents<ISavePayloadProvider>())
            {
                try
                {
                    provider.Capture(data);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"Capture failed on provider {provider.GetType().Name} for entity {_id}: {exception.Message}");
                }
            }

            return data;
        }

        public void Restore(EntityStateData data)
        {
            if (data == null)
            {
                Debug.LogWarning($"No data provided for saveable entity {_id}");
                return;
            }

            if (!string.IsNullOrEmpty(data.SceneName) && !_persistAcrossScenes)
            {
                var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                if (!string.Equals(currentScene, data.SceneName, StringComparison.Ordinal))
                {
                    Debug.LogWarning($"Entity {_id} belongs to scene {data.SceneName} but current scene is {currentScene}");
                }
            }

            transform.position = data.Position;

            foreach (var provider in GetComponents<ISavePayloadProvider>())
            {
                try
                {
                    provider.Restore(data);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"Restore failed on provider {provider.GetType().Name} for entity {_id}: {exception.Message}");
                }
            }
        }

        private void Reset()
        {
            GenerateId();
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_id))
            {
                GenerateId();
            }
        }

        private void GenerateId()
        {
            _id = Guid.NewGuid().ToString();
        }
    }
}

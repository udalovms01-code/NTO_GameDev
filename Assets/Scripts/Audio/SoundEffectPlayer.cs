using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class SoundCollection
    {
        [SerializeField] private AudioClip[] clips;
        [Range(0f, 2f)] [SerializeField] private float volume = 1f;
        [Range(0f, 0.5f)] [SerializeField] private float pitchVariation = 0.05f;

        public bool IsValid => clips != null && clips.Length > 0;

        public AudioClip GetRandomClip()
        {
            if (!IsValid)
            {
                return null;
            }

            int index = UnityEngine.Random.Range(0, clips.Length);
            return clips[index];
        }

        public float Volume => volume;
        public float PitchVariation => pitchVariation;
    }

    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private void Awake()
        {
            audioSource ??= GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        public void Play(SoundCollection collection)
        {
            if (collection == null || !collection.IsValid)
            {
                return;
            }

            PlayClip(collection.GetRandomClip(), collection.Volume, collection.PitchVariation);
        }

        public void PlayClip(AudioClip clip, float volume = 1f, float pitchVariation = 0f)
        {
            if (clip == null)
            {
                return;
            }

            float originalPitch = audioSource.pitch;
            audioSource.pitch = originalPitch + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
            audioSource.PlayOneShot(clip, volume);
            audioSource.pitch = originalPitch;
        }
    }
}

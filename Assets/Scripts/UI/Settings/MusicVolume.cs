using UnityEngine;
using UnityEngine.Audio;

namespace UI.Settings
{
    public class MusicVolume : MonoBehaviour
    {

        [SerializeField] private AudioMixerGroup _master;
        [SerializeField] private AudioMixerGroup _music; 
        [SerializeField] private AudioMixerGroup _sfx;
        
        public static MusicVolume Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }
        
        public void SetMusicVolume(float volume)
        {
            _music.audioMixer.SetFloat("Music", volume);
        }
        
        public void SetSFXVolume(float volume)
        {
            _sfx.audioMixer.SetFloat("Sfx", volume);
        }
        
        public void SetMasterVolume(float volume)
        {
            _master.audioMixer.SetFloat("Master", volume);
        }
    }
}
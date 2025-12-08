using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Audio
{
    public class GameStateMusicController : MonoBehaviour, IInitializable, System.IDisposable
    {
        private const string MusicMixerResourcePath = "Music/AudioMixer";

        [SerializeField] private float _fadeDuration = 2f;
        [SerializeField] private string _waitingForSlicedFishClip = "Music/walking";
        [SerializeField] private string _slicedFishClip = "Music/shipTheme";
        [SerializeField] private string _waitingForDialogClip = "Music/lastDayWalking";
        [SerializeField] private string _dialogClip = "Music/monster";
        [SerializeField] private string _waitingForSleepClip = "Music/lastDayWalking";
        [SerializeField] private string _sleepClip = "Music/lastDayWalking";

        private readonly Dictionary<GameState, AudioClip> _clips = new();
        private AudioSource[] _sources;
        private int _activeSourceIndex;
        private Coroutine _fadeCoroutine;
        private AudioMixerGroup _musicGroup;

        private GameStateService _gameStateService;

        [Inject]
        public void Construct(GameStateService gameStateService)
        {
            _gameStateService = gameStateService;
        }

        public void Initialize()
        {
            LoadClips();
            SetupAudioSources();

            _gameStateService.OnStateChanged += HandleStateChanged;
            HandleStateChanged(_gameStateService.CurrentState);
        }

        public void Dispose()
        {
            _gameStateService.OnStateChanged -= HandleStateChanged;

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
        }

        private void LoadClips()
        {
            _musicGroup = LoadMusicGroup();

            _clips[GameState.WaitingForSlicedFish] = Resources.Load<AudioClip>(_waitingForSlicedFishClip);
            _clips[GameState.SlicedFish] = Resources.Load<AudioClip>(_slicedFishClip);
            _clips[GameState.WaitingForDialog] = Resources.Load<AudioClip>(_waitingForDialogClip);
            _clips[GameState.Dialog] = Resources.Load<AudioClip>(_dialogClip);
            _clips[GameState.WaitingForSleep] = Resources.Load<AudioClip>(_waitingForSleepClip);
            _clips[GameState.Sleep] = Resources.Load<AudioClip>(_sleepClip);
        }

        private AudioMixerGroup LoadMusicGroup()
        {
            AudioMixer mixer = Resources.Load<AudioMixer>(MusicMixerResourcePath);
            if (mixer == null)
            {
                return null;
            }

            AudioMixerGroup[] groups = mixer.FindMatchingGroups("Music");
            return groups.Length > 0 ? groups[0] : null;
        }

        private void SetupAudioSources()
        {
            _sources = new AudioSource[2];

            for (int i = 0; i < _sources.Length; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.loop = true;
                source.playOnAwake = false;
                source.outputAudioMixerGroup = _musicGroup;
                _sources[i] = source;
            }
        }

        private void HandleStateChanged(GameState state)
        {
            if (!_clips.TryGetValue(state, out AudioClip nextClip) || nextClip == null)
            {
                return;
            }

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _fadeCoroutine = StartCoroutine(FadeToClip(nextClip));
        }

        private IEnumerator FadeToClip(AudioClip clip)
        {
            int nextSourceIndex = 1 - _activeSourceIndex;
            AudioSource currentSource = _sources[_activeSourceIndex];
            AudioSource nextSource = _sources[nextSourceIndex];

            if (currentSource.clip == clip)
            {
                if (!currentSource.isPlaying)
                {
                    currentSource.volume = 1f;
                    currentSource.Play();
                }

                yield break;
            }

            nextSource.clip = clip;
            nextSource.volume = 0f;
            nextSource.Play();

            float time = 0f;
            while (time < _fadeDuration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / _fadeDuration);

                currentSource.volume = Mathf.Lerp(1f, 0f, t);
                nextSource.volume = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }

            currentSource.Stop();
            currentSource.volume = 1f;

            _activeSourceIndex = nextSourceIndex;
            _fadeCoroutine = null;
        }
    }
}

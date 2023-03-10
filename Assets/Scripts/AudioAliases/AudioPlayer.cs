
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace AudioAliase
{
    public enum CurrentlyPlaying
    {
        /// <summary>
        /// The start sound is defined
        /// </summary>
        Start,
        Base,
        End,
    }
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        public Queue<Aliase> _clips = new();
        [SerializeField] private Aliase _lastAliasePlayed;
        [SerializeField] private bool forceStop;

        private bool _isStopping;
        private StopLoopBehavior _stopLoopBehavior;
        [SerializeField] [Aliase]  private string OnStartAliaseToPlay = "";
        private float _delayLoop = 0;
        #region Private Variable
        
        private bool _startWasPlayed;
        // TODO : Not good to use aliase in properties because it will be copied (serialize shit), we need to use simply string
        private Aliase _nextSound;
        private Transform _transformToFollow;
        private float _timePlayed = 0;
        private CurrentlyPlaying _state = CurrentlyPlaying.Start;
        #endregion
       

        public AudioSource Source { get; private set; }
        /// <summary>
        /// AudioPlayer is available ? 
        /// </summary>
        public bool IsUsable => _clips.Count == 0 && !Source.isPlaying && !gameObject.activeSelf;
        public bool IsFollowingTransform => _transformToFollow != null;
        
        public void SetTransformToFollow(Transform transformTarget)
        {
            _transformToFollow = transformTarget;
        }
        
        #region Event function
        
            private void Awake()
            {
                Source = transform.GetComponent<AudioSource>();
            }
            private void Start()
            {
                if (OnStartAliaseToPlay != "")
                {
                    Play(OnStartAliaseToPlay);
                }
            }
            // Update is called once per frame
            private void Update()
            {
                if(_lastAliasePlayed == null)  gameObject.SetActive(false);
                FollowTransform();
                // Audio play have finish the play
                if (_timePlayed >= (Source.clip.length* Source.pitch ) + _delayLoop)
                {
                    if (_isStopping)
                    {
                        gameObject.SetActive(false);
                        Reset();
                        return;
                    }
                        
                    if (_lastAliasePlayed.isLooping)
                    {
                        SetupAudioSource(_lastAliasePlayed);
                        Source.Play();
                        _timePlayed = 0;
                    }
                    else // End of the sound
                    {
                        switch(_state)
                        {
                            case CurrentlyPlaying.Start:
                                //_state = CurrentlyPlaying.Base;
                                SetupAudioSource(_nextSound);
                                Source.clip = _nextSound.Audio; 
                                Source.Play();
                                break;
                            case CurrentlyPlaying.Base:
                                StopSound(_stopLoopBehavior);
                                break;
                            case CurrentlyPlaying.End:
                            default:
                                gameObject.SetActive(false);
                                Reset();
                                break;
                        }

                        _timePlayed = 0;
                        _state++;
                    }
                }
                else
                {
                    _timePlayed += Time.unscaledDeltaTime;
                }
    
            }
            private void Reset()
            {
                Source.Stop();
                _lastAliasePlayed = null;
                _transformToFollow = null;
                _state = CurrentlyPlaying.Start;
                _timePlayed = 0;
                _nextSound = null;
            }


        #endregion
        
        private void Play(Aliase aliaseToPlay)
        {
            // If a start aliase is available, we need to play it before the base aliase
            if (_state == CurrentlyPlaying.Start && AudioManager.GetSoundByAliase(aliaseToPlay.startAliase, out Aliase startLoop))
            {
                SetupAudioSource(startLoop);
                Source.clip = startLoop.Audio;
                Source.Play();
                _nextSound = aliaseToPlay;
                return;
            }
            _state = CurrentlyPlaying.Base; // Sinon ca fait le bug du next sound pas def
            //Setup the base aliase
            SetupAudioSource(aliaseToPlay);
            Source.clip = aliaseToPlay.Audio; 
            Source.Play();
        }
        private void Play(string onStartAliasToPlay)
        {
            AudioManager.GetSoundByAliase(onStartAliasToPlay, out var aliase);
            Play(aliase);
        }
        public void Setup(Aliase aliaseToPlay)
        {
            Reset();
            Play(aliaseToPlay);
        }
        private void FollowTransform()
        {
            if (IsFollowingTransform)
            {
                transform.position = _transformToFollow.position;
            } 
        }
        public void StopSound(StopLoopBehavior stopLoopBehavior)
        {
            _stopLoopBehavior = stopLoopBehavior;
            if (_state == CurrentlyPlaying.Start)
            {
                switch (stopLoopBehavior)
                {
                    case StopLoopBehavior.Direct:
                        gameObject.SetActive(false);
                        break;
                    case StopLoopBehavior.FinishCurrentPlay:
                        _isStopping = true;
                        break;
                    default:
                        gameObject.SetActive(false);
                        break;
                }
               
                return;
            }
            
            if (_state == CurrentlyPlaying.Base
                && !string.IsNullOrEmpty(_lastAliasePlayed.endAliase)
                && AudioManager.GetSoundByAliase(_lastAliasePlayed.endAliase, out Aliase stopLoop) )
            {
                SetupAudioSource(stopLoop);
                Source.clip = stopLoop.Audio;
                Source.Play();
            }
            else
            {
                switch (stopLoopBehavior)
                {
                    case StopLoopBehavior.Direct:
                        gameObject.SetActive(false);
                        break;
                    case StopLoopBehavior.FinishCurrentPlay:
                        _isStopping = true;
                        break;
                    default:
                        gameObject.SetActive(false);
                        break;
                }
            }

            _state++;
        }

        public void SetupAudioSource(Aliase aliase)
        {
            if (aliase == null)
            {
                if(AudioManager.ShowDebugText) Debug.LogError("What the fuck ?");
            }
            _timePlayed = 0;
            _isStopping = false;
           
            _lastAliasePlayed = aliase;
            var audiosource = Source;
            audiosource.volume = Random.Range(aliase.minVolume, aliase.maxVolume);
           // audiosource.loop = aliase.isLooping;
            if ( aliase.isLooping)
            {
                _delayLoop = aliase.DelayLoop;
            }
            else
            {
                _delayLoop = 0;
            }
              
            audiosource.pitch = Random.Range(aliase.minPitch, aliase.maxPitch);
            audiosource.spatialBlend = aliase.spatialBlend;
            if (aliase.MixerGroup != null)
                audiosource.outputAudioMixerGroup = aliase.MixerGroup;

            switch (aliase.CurveType)
            {
                case AudioRolloffMode.Logarithmic:
                case AudioRolloffMode.Linear:
                    audiosource.rolloffMode = aliase.CurveType;
                    break;
                case AudioRolloffMode.Custom:
                    audiosource.rolloffMode = aliase.CurveType;
                    audiosource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, aliase.distanceCurve);
                    break;

            }
        }
    }
}
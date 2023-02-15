
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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
        public Queue<Alias> _clips = new();
        [FormerlySerializedAs("_lastAliasePlayed")] [SerializeReference] private Alias lastAliasPlayed;
        [SerializeField] private bool forceStop;
        
        private bool _isStopping;
        private StopLoopBehavior _stopLoopBehavior;
        [SerializeField] [Aliase]  private string OnStartAliaseToPlay = "";
        private float _delayLoop = 0;
        #region Private Variable
        
        private bool _startWasPlayed;
        // TODO : Not good to use aliase in properties because it will be copied (serialize shit), we need to use simply string
        private Alias _nextSound;
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

        [Tooltip("Specify if the audioplayer is reserved by another object, the pool will not use it")]
        [SerializeField] private bool isReserved;
        /// <summary>
        /// Specify if the audioplayer is reserved by another object, the pool will not use it
        /// </summary>
        public bool IsReserved
        {
            get => isReserved;
            set => isReserved = value;
        }

        public void SetTransformToFollow(Transform transformTarget)
        {
            _transformToFollow = transformTarget;
        }
        
        #region MonoBehaviour
        
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
                if(lastAliasPlayed == null)  gameObject.SetActive(false);
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
                        
                    if (lastAliasPlayed.isLooping)
                    {
                        SetupAudioSource(lastAliasPlayed);
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
                lastAliasPlayed = null;
                _transformToFollow = null;
                _state = CurrentlyPlaying.Start;
                _timePlayed = 0;
                _nextSound = null;
            }


        #endregion
        
        private void Play(Alias aliasToPlay)
        {
            // If a start aliase is available, we need to play it before the base aliase
            if (_state == CurrentlyPlaying.Start && AudioManager.GetSoundByAliase(aliasToPlay.startAliase, out Alias startLoop))
            {
                SetupAudioSource(startLoop);
                Source.clip = startLoop.Audio;
                Source.Play();
                _nextSound = aliasToPlay;
                return;
            }
            _state = CurrentlyPlaying.Base; // Sinon ca fait le bug du next sound pas def
            //Setup the base aliase
            SetupAudioSource(aliasToPlay);
            Source.clip = aliasToPlay.Audio; 
            Source.Play();
        }
        private void Play(string onStartAliasToPlay)
        {
            AudioManager.GetSoundByAliase(onStartAliasToPlay, out var aliase);
            Play(aliase);
        }
        public void Setup(Alias aliasToPlay , Vector3 position )
        {
            transform.position = position;
            gameObject.SetActive(true);
            Reset();
            Play(aliasToPlay);
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
            if(lastAliasPlayed != null && lastAliasPlayed.audioPlayers.Contains(this))
                lastAliasPlayed.audioPlayers.Remove(this);
            
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
                && !string.IsNullOrEmpty(lastAliasPlayed.endAliase)
                && AudioManager.GetSoundByAliase(lastAliasPlayed.endAliase, out Alias stopLoop) )
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
        public void SetupAudioSource(Alias alias)
        {
            if (alias == null)
            {
                if(AudioManager.ShowDebugText) Debug.LogError("What the fuck ?");
            }
            if(lastAliasPlayed != null && lastAliasPlayed.audioPlayers.Contains(this))
                lastAliasPlayed.audioPlayers.Remove(this);
            
            if(!alias.audioPlayers.Contains(this))
                alias.audioPlayers.Add(this);
            
            _timePlayed = 0;
            _isStopping = false;
           
            lastAliasPlayed = alias;
            var audiosource = Source;
            audiosource.volume = Random.Range(alias.minVolume, alias.maxVolume);
            if ( alias.isLooping)
            {
                _delayLoop = alias.DelayLoop;
            }
            else
            {
                _delayLoop = 0;
            }
              
            audiosource.pitch = Random.Range(alias.minPitch, alias.maxPitch);
            audiosource.spatialBlend = alias.spatialBlend;
            if (alias.MixerGroup != null)
                audiosource.outputAudioMixerGroup = alias.MixerGroup;

            switch (alias.CurveType)
            {
                case AudioRolloffMode.Logarithmic:
                case AudioRolloffMode.Linear:
                    audiosource.rolloffMode = alias.CurveType;
                    break;
                case AudioRolloffMode.Custom:
                    audiosource.rolloffMode = alias.CurveType;
                    audiosource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, alias.distanceCurve);
                    break;

            }
        }
    }
}
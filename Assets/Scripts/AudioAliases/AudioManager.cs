using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;


namespace AudioAliase
{
    public enum SoundType
    {
        Start,
        Root,
        End,
    }

    public static class AudioManagerExtension 
    {
        public static void PlayLoopSound(this Transform transform, string aliaseName, ref AudioPlayer audioPlayerLoop)
        {
            AudioManager.PlayLoopSound(aliaseName, transform, ref audioPlayerLoop);
        }
        public static void PlayLoopSound(this GameObject gameObject, string aliaseName, ref AudioPlayer audioPlayerLoop)
        {
            AudioManager.PlayLoopSound(aliaseName, gameObject.transform, ref audioPlayerLoop);
        }
        public static Aliase PlaySoundAtPosition(this GameObject gameObject, string aliaseName)
        {
            return AudioManager.PlaySoundAtPosition(aliaseName, gameObject.transform.position);
        }
        public static Aliase PlaySoundAtPosition(this Transform transform, string aliaseName)
        {
            return AudioManager.PlaySoundAtPosition(aliaseName, transform.position);
        }
        
    }
    
    public class AudioManager : MonoBehaviour
    {
        #region Singleton

        private static AudioManager _instance;
        private static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                    if(_instance == null)
                        _instance = new GameObject("AudioManager").AddComponent<AudioManager>();
                }

                return _instance;
            }
            set => _instance = value;
        }
        

        #endregion

        public bool debugText = false;
        public static bool ShowDebugText => Instance.debugText;
        public static Aliases[] aliasesArray
        {
            get { return Instance._audioManagerData.aliases; }
            //set { Instance.TableAliasesLoaded = value; }
        } 
        [SerializeField] private List<AudioPlayer> _audioSource;
        public const string AliasNameNull = "None";
        [SerializeField] int audioSourcePoolSize = 128; // 32 is a good start
        private static Vector3 positionDefault = Vector3.zero;
        [Header("Debug")]
        //[SerializeField] Aliases[] TableAliasesLoaded = new Aliases[0];

        bool _isPaused;
        
        [HideInInspector] [SerializeReference] private AudioManagerData _audioManagerData;
        public bool IsPaused
        {
            set { _isPaused = value; }
        }
        
       

        void InitAudioSources()
        {
            _audioSource = new List<AudioPlayer>();
            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                GameObject newAudioSource = new GameObject("Audio Source " + i);
                newAudioSource.transform.SetParent(transform);
                AudioPlayer audioS = newAudioSource.AddComponent<AudioPlayer>();
                Instance._audioSource.Add(audioS);
                newAudioSource.SetActive(false);
            }
        }
        void Awake()
        {
            Instance = this;
            _audioManagerData = (AudioManagerData) Resources.Load("AudioManager Data") ;
         
            InitAudioSources();
        }
        // Update is called once per frame
        void Update()
        {
            if (_isPaused)
            {
                PauseAllAudio();
            }
            else
            {
                 UnPauseAllAudio();
                //DisableInusedAudioSource();
            }
        }
        void DisableInusedAudioSource()
        {
            foreach (AudioPlayer aS in _audioSource)
            {
                if (!aS.IsUsable)
                {
                    aS.gameObject.SetActive(false);
                }
            }
        }

        public static bool GetSoundByAliase(string name, out Aliase alias)
        {
            alias = null;
            if (name == AliasNameNull) return false;
            
            for (int i = 0; i < aliasesArray.Length; i++)
            {
                foreach (Aliase tempalias in aliasesArray[i].aliases)
                {
                    if (tempalias.name == name)
                    {
                        alias = tempalias;

                    }
                }
            }

            if (alias != null && alias.audio.Length == 0)
            {
                if(ShowDebugText)
                Debug.LogError("[AudioManager] : Aliase: " + name + " contains no sounds.");
                return false;
            }

            if (alias == null)
            {
                if(ShowDebugText)Debug.LogWarning("[AudioManager] : Aliase: " + name + " not found.");
                return false;
            }

            return true;
            //return alias;
        }

        static AudioSource GetAudioSource()
        {
            foreach (AudioPlayer aS in Instance._audioSource)
            {
                if (!aS.Source.isPlaying)
                {
                    return aS.Source;
                }
            }
            return null;
        }

        internal static bool GetAudioPlayer(out AudioPlayer audioPlayer)
        {
            audioPlayer = null;
           
            foreach (AudioPlayer aS in Instance._audioSource)
            {
                if (aS.IsUsable)
                {
                    audioPlayer = aS;
                    return true;
                }
            }
            if(ShowDebugText)Debug.LogWarning($"AudioManager : Limits exceded for _audioSource, maybe you need to increase your audioSourcePoolSize (Size = {Instance.audioSourcePoolSize})");
            return false;
        }
        
        public static void PauseAllAudio()
        {
            foreach (AudioPlayer aS in Instance._audioSource)
            {
                if (aS.Source.isPlaying)
                {
                    aS.Source.Pause();
                }
            }
        }
        public static void UnPauseAllAudio()
        {
            foreach (AudioPlayer aS in Instance._audioSource)
            {
                //if(audio.UnPause)
                {
                    aS.Source.UnPause();
                }
            }
        }

        static bool IsValidAliase()
        {

            return false;
        }

        void AliaseIsValid()
        {
            
        }
        public static Aliase PlaySoundAtPosition(string aliaseName, Vector3 position = default)
        {
            if (string.IsNullOrEmpty(aliaseName))
            {
                if(ShowDebugText)  Debug.LogError("AudioManager : Un son a voulu ??tre jouer sans d'aliaseName, il faut en assign?? un dans le script qui a ex??cut?? la function");
                return null;
            }
            if(GetSoundByAliase(aliaseName, out Aliase clip) && GetAudioPlayer(out AudioPlayer audioPlayer))
            {
                audioPlayer.gameObject.transform.position = position;
                audioPlayer.gameObject.SetActive(true);
                audioPlayer.Setup(clip);

                if (clip.isPlaceholder)
                {
                    if(ShowDebugText) Debug.LogWarning("Un son placeholder a ??t?? jouer, il faut le changer , nom de l'aliase " + aliaseName);
                }

                if (clip.Secondary != System.String.Empty)
                {
                    PlaySoundAtPosition(clip.Secondary, position);
                }

                return clip;
            }

            return null;
        }
        /// <summary>
        /// Play a loop sound on the desired transform
        /// </summary>
        /// <param name="aliaseName"></param>
        /// <param name="transformToTarget">transform to follow</param>
        /// <param name="audioPlayerLoop"> A ref to AudioPlayer, it can be used with the method StopLoopSound</param>
        public static void PlayLoopSound(string aliaseName, Transform transformToTarget, ref AudioPlayer audioPlayerLoop)
        {
            PlayLoopSound(aliaseName, transformToTarget.position, ref audioPlayerLoop);
            if (audioPlayerLoop != null)
            {
                audioPlayerLoop.SetTransformToFollow(transformToTarget);
            }
           
        }
        /// <summary>
        /// Play a loop sound at the desired position
        /// </summary>
        /// <param name="aliaseName"></param>
        /// <param name="position"> The position of the loop sound</param>
        /// <param name="audioPlayerLoop"> A ref to <see cref="AudioPlayer"/>, it can be used with the method StopLoopSound</param>
        public static void PlayLoopSound(string aliaseName, Vector3 position, ref AudioPlayer audioPlayerLoop )
        {
            if (audioPlayerLoop != null && !audioPlayerLoop.IsUsable)
            {
                if (ShowDebugText)
                {
                    Debug.Log($"[AudioManager] PlayLoop {aliaseName} already played");
                }

                return;
            }
            if (string.IsNullOrEmpty(aliaseName))
            {
               // throw new InvalidAliasesException("AudioManager : No specified aliases");
               return;
            }
            if (!GetSoundByAliase(aliaseName, out Aliase clip))
            {
                return ;
            }
            if (!GetAudioPlayer(out AudioPlayer audioPlayer))
            {
                if(ShowDebugText) Debug.LogWarning($"AudioManager :green; ??? Limits exceded for _audioSource, maybe you need to increase your audioSourcePoolSize (Size = {Instance.audioSourcePoolSize})");
                return ;
            }
            audioPlayer.gameObject.transform.position = position;
            audioPlayer.gameObject.SetActive(true);
            audioPlayer.Setup(clip);
            if (clip.isPlaceholder)
            {
                if(ShowDebugText) Debug.LogWarning("[AudioManager] Placeholder sound was played, name " + aliaseName);
            }
            if (!String.IsNullOrEmpty(clip.Secondary))
            {
                PlaySoundAtPosition(clip.Secondary, position);
            }
            audioPlayerLoop = audioPlayer;
        }
        public static void StopLoopSound(ref AudioPlayer audioPlayer, StopLoopBehavior stopLoopBehavior = StopLoopBehavior.FinishCurrentPlay)
        {
            if (audioPlayer != null)
            {
                audioPlayer.StopSound(stopLoopBehavior);
            }

            audioPlayer = null;
        }
    }

    public enum StopLoopBehavior
    {
        Direct,
        FinishCurrentPlay
    }
    
    public class InvalidAliasesException : Exception {
        public InvalidAliasesException() : base() { }

        public InvalidAliasesException(string message) : base(message) { }

        public InvalidAliasesException(string message, Exception innerException) : base(message, innerException) { }
    }
}
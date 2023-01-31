using AudioAliase;
using UnityEngine;

namespace AudioAliase
{
    public partial class AudioManager : MonoBehaviour
    {
        public static AudioPlayer _audioPlayerAnnouncer;

        public static bool PlayAnnouncer(string aliaseName)
        {
            if (_audioPlayerAnnouncer == null)
            {
                AudioManager.GetAudioPlayer(out var audioPlayer);
                _audioPlayerAnnouncer = audioPlayer;
                _audioPlayerAnnouncer.IsReserved = true;
            }

            if (_audioPlayerAnnouncer.Source.isPlaying)
            {
                return false;
            }

            return AudioManager.PlaySoundAtPosition(aliaseName, _audioPlayerAnnouncer) != null;
        }

    }
}

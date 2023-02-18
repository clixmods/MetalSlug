using UnityEngine;
namespace Powerup
{
    public class PowerupPoint : PowerupInstance
    {
        public delegate void EventPointGrab(int amount);
        public static event EventPointGrab OnGrabPoint;
        [SerializeField] private int pointAmount;
        private void Awake()
        {
            LevelManager.CallbackOnRoundChange += LevelManagerOnCallbackOnRoundChange;
        }

        private void LevelManagerOnCallbackOnRoundChange(int newround)
        {
            gameObject.SetActive(true);
        }

        public override void OnGrab(PlayerInstance playerInstance)
        {
            base.OnGrab(playerInstance);
            gameObject.SetActive(false);
            OnGrabPoint?.Invoke(pointAmount);
        }
    }
}
using System;
using UnityEngine;
namespace Powerup
{
    public class PowerupPoint : PowerupInstance
    {
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
            UIPointsPlusPanel.CreateUIPointsPlus(FindObjectOfType<Canvas>().gameObject, transform.position, pointAmount);
            base.OnGrab(playerInstance);
            gameObject.SetActive(false);
        }
    }
}
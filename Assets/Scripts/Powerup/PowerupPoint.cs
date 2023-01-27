using UnityEngine;
namespace Powerup
{
    public class PowerupPoint : PowerupInstance
    {
        [SerializeField] private int pointAmount;
        public override void OnGrab(PlayerInstance playerInstance)
        {
            UIPointsPlusPanel.CreateUIPointsPlus(FindObjectOfType<Canvas>().gameObject, transform.position, pointAmount);
            base.OnGrab(playerInstance);
        }
    }
}
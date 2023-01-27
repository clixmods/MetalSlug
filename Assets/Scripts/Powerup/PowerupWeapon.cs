using UnityEngine;
namespace Powerup
{
    public class PowerupWeapon : PowerupInstance
    {
        [SerializeField] private WeaponScriptableObject weaponGift;
        public override void OnGrab(PlayerInstance playerInstance)
        {
            playerInstance.GiveWeapon(weaponGift);
            base.OnGrab(playerInstance);
        }
    }
}
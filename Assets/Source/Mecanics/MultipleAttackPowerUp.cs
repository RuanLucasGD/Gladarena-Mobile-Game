using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class MultipleAttackPowerUp : PowerUp
    {
        [Header("Multiplier Velocity")]
        public float CharacterInterval;
        public float WeaponInterval;

        public override Character Owner
        {
            get => base.Owner;
            set
            {
                // reset attack length multiplier
                if (Owner && Owner.HasWeapon)
                {
                    Owner.Weapon.WeaponObject.AttackLengthMultiplier = 1;
                }

                // reset attack interval multiplier and attack anim type
                if (Owner)
                {
                    Owner.Weapon.DelatyToAttackMultiplier = 1;
                    Owner.IsSuperAttack = false;
                }

                if (value && value.HasWeapon)
                {
                    value.Weapon.WeaponObject.AttackLengthMultiplier = WeaponInterval;
                }

                if (value)
                {
                    value.Weapon.DelatyToAttackMultiplier = CharacterInterval;
                    value.IsSuperAttack = true;
                }

                base.Owner = value;

                UsePowerUp();
            }
        }

        public override void UsePowerUp()
        {
            if (!Owner || !Owner.enabled || !Owner.HasWeapon)
            {
                return;
            }

            base.UsePowerUp();
        }
    }
}
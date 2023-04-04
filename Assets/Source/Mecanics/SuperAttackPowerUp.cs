using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class SuperAttackPowerUp : PowerUp
    {
        public float AttackForceMultiplier;
        public float AttackDamageMultiplier;

        public override Character Owner
        {
            get => base.Owner;
            set
            {
                // reset attack force/damage multiplier
                if (Owner && Owner.HasWeapon)
                {
                    Owner.Weapon.WeaponObject.IsSuperAttack = false;
                    Owner.Weapon.WeaponObject.AttackDamageMultiplier = 1;
                    Owner.Weapon.WeaponObject.AttackForceMultiplier = 1;
                }

                if (value && value.HasWeapon)
                {
                    value.Weapon.WeaponObject.IsSuperAttack = true;
                    value.Weapon.WeaponObject.AttackDamageMultiplier = AttackDamageMultiplier;
                    value.Weapon.WeaponObject.AttackForceMultiplier = AttackForceMultiplier;
                }

                base.Owner = value;
            }
        }

        public SuperAttackPowerUp()
        {
            AttackForceMultiplier = 2;
            AttackDamageMultiplier = 2;
        }
    }
}
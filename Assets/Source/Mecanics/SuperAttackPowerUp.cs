using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class SuperAttackPowerUp : PowerUp
    {
        public float AttackLengthMultiplier;
        public float AttackForceMultiplier;
        public float AttackDamageMultiplier;

        public override PlayerCharacter Owner
        {
            get => base.Owner;
            set
            {
                if (value.HasWeapon)
                {
                    value.Weapon.WeaponObject.IsSuperAttack = true;
                    value.Weapon.WeaponObject.AttackDamageMultiplier = AttackDamageMultiplier;
                    value.Weapon.WeaponObject.AttackForceMultiplier = AttackForceMultiplier;
                    value.Weapon.WeaponObject.AttackLengthMultiplier = AttackLengthMultiplier;
                }

                base.Owner = value;
            }
        }

        public SuperAttackPowerUp()
        {
            AttackForceMultiplier = 2;
            AttackDamageMultiplier = 2;
            AttackLengthMultiplier = 2;
        }

        public override void OnRemove()
        {
            base.OnRemove();

            // reset attack multiplier
            if (Owner && Owner.HasWeapon)
            {
                Owner.Weapon.WeaponObject.IsSuperAttack = false;
                Owner.Weapon.WeaponObject.AttackDamageMultiplier = 1;
                Owner.Weapon.WeaponObject.AttackForceMultiplier = 1;
                Owner.Weapon.WeaponObject.AttackLengthMultiplier = 1;
            }
        }
    }
}
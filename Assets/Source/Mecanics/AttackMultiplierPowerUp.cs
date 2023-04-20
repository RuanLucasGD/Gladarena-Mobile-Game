using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class AttackMultiplierPowerUp : PowerUp
    {
        public float AttackRageMultiplier;
        public float AttackLengthMultiplier;
        public float AttackDamageMultiplier;

        public override PlayerCharacter Owner
        {
            get => base.Owner;
            set
            {
                base.Owner = value;
                Owner.Weapon.AttackRate *= AttackRageMultiplier;
                Owner.Weapon.AttackDamageMultiplier *= AttackDamageMultiplier;
                Owner.Weapon.AttackLengthMultiplier *= AttackLengthMultiplier;
            }
        }

        public AttackMultiplierPowerUp()
        {
            AttackDamageMultiplier = 1;
            AttackLengthMultiplier = 1;
            AttackRageMultiplier = 1;
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }
    }
}
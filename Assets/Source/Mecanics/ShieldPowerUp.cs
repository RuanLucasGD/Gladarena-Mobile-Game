using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class ShieldPowerUp : PowerUp
    {
        public float Damage;
        public float Force;

        public override Character Owner
        {
            get => base.Owner;
            set
            {
                base.Owner = value;
                gameObject.SetActive(true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Character>(out var c))
            {
                if (c != Owner)
                {
                    c.AddDamage(Damage, (c.transform.position - transform.position).normalized * Force);
                }
            }
        }
    }
}
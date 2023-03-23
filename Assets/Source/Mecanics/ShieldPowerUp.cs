using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class ShieldPowerUp : PowerUp
    {
        public override Character Owner
        {
            get => base.Owner;
            set
            {
                // remove the power up effect of the character when change/remove your power up
                if (Owner)
                {
                    Owner.IsInvencible = false;
                }

                // add power up effect on character
                if (value)
                {
                    value.IsInvencible = true;
                }

                base.Owner = value;

                gameObject.SetActive(Owner != null);
            }
        }
    }
}



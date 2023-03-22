using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class RestoreLifePowerUp : PowerUp
    {
        public override Character Owner
        {
            get => base.Owner;
            set
            {
                if (value)
                {
                    if (ArenaManager.Instance)
                    {
                        ArenaManager.Instance.OnStartLevel.AddListener(UseWhenStartLevel);
                    }
                }
                else
                {
                    if (ArenaManager.Instance)
                    {
                        ArenaManager.Instance.OnStartLevel.RemoveListener(UseWhenStartLevel);
                    }
                }

                base.Owner = value;
            }
        }

        public override void UsePowerUp()
        {
            if (!Owner)
            {
                return;
            }

            // dont use power up if game is finished
            if (ArenaManager.Instance && ArenaManager.Instance.GameWin)
            {
                return;
            }

            base.UsePowerUp();
            Owner.ResetLife();
            Owner.SetPowerUp(null, true);   // use this power up only once
        }

        private void UseWhenStartLevel(int currentHorder)
        {
            UsePowerUp();
        }
    }
}

using UnityEngine;

namespace Game.Mecanics
{
    public class RestoreLifePowerUp : PowerUp
    {
        public override PlayerCharacter Owner
        {
            get => base.Owner;
            set
            {
                SetupPowerUp();
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


            Debug.Log("used");
        }

        public override void OnRemove()
        {
            base.OnRemove();
            Debug.Log("removed");
        }

        private void UseWhenStartLevel(int currentHorder)
        {
            UsePowerUp();

            // dont use this powerup anymore
            Owner.RemovePowerUp(this);
        }

        private void SetupPowerUp()
        {

            if (ArenaManager.Instance)
            {
                ArenaManager.Instance.OnStartLevel.AddListener(UseWhenStartLevel);
                ArenaManager.Instance.OnCompleteLevel.RemoveListener(UseWhenStartLevel);
            }
        }
    }
}

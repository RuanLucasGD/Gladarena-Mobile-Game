using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class AresArmyItem : MonoBehaviour
    {
        private float _regenerateTime;
        public AresArmyPowerUp powerUp;

        private void Update()
        {
            if (GameManager.Instance.Player.IsDeath)
            {
                return;
            }

            if (powerUp.Levels[powerUp.CurrentLevelIndex].GenerateInterval == 0)
            {
                return;
            }

            if (powerUp.CurrentClonesAmount > 0)
            {
                _regenerateTime = 0;
                return;
            }

            _regenerateTime += Time.deltaTime;
            if (_regenerateTime > powerUp.Levels[powerUp.CurrentLevelIndex].GenerateInterval)
            {
                _regenerateTime = 0f;
                powerUp.RecreateAllClones();
            }
        }
    }
}
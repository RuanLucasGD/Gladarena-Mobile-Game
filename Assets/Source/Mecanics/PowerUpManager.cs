using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class PowerUpManager : MonoBehaviour
    {
        public PowerUp[] AllPowerUps;
        public List<PowerUp> CurrentPowerUps;

        public PowerUp[] GeneratePowerUpsOptionsList(int amount)
        {
            var _powerUps = new PowerUp[amount];

            for (int i = 0; i < amount; i++)
            {
                if (i <= AllPowerUps.Length - 1)
                {
                    _powerUps[i] = AllPowerUps[i];
                }
            }

            return _powerUps;
        }

        public void AddNewPowerUp(PowerUp powerUp)
        {
            var hasPowerUp = ((Func<bool>)(() =>
            {
                foreach (var p in CurrentPowerUps)
                {
                    if (powerUp == p)
                    {
                        return true;
                    }
                }

                return false;
            }))();

            if (!hasPowerUp)
            {
                CurrentPowerUps.Add(powerUp);
                powerUp.Upgrade();
            }

            powerUp.Use();
        }
    }
}



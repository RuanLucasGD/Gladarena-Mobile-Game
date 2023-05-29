using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Game.Mecanics
{
    public class PowerUpManager : MonoBehaviour
    {
        public List<PowerUp> AllPowerUps;
        public List<PowerUp> CurrentPowerUps;

        public PowerUp[] GenerateRandomPowerUpsList(int amount)
        {
            var _addedAmount = 0;
            var _allPowerUpsToGetAmount = AllPowerUps.Count;
            var _selectedPowerUps = new List<PowerUp>();

            PowerUp _randomPowerUp() => AllPowerUps[Random.Range(0, AllPowerUps.Count)];

            // gets a list of various random powerups
            while (_addedAmount < amount && _allPowerUpsToGetAmount > 0)
            {
                var randomPowerUp = _randomPowerUp();

                foreach (var p in _selectedPowerUps)
                {
                    while (randomPowerUp == p)
                    {
                        randomPowerUp = _randomPowerUp();
                    }
                }

                // remove powerups that cannot be obtained because its
                // already at a full level
                if (randomPowerUp.IsFullUpgrade())
                {
                    randomPowerUp = null;
                    _allPowerUpsToGetAmount--;
                }

                if (randomPowerUp != null)
                {
                    _addedAmount++;
                    _selectedPowerUps.Add(randomPowerUp);
                }
            }

            return _selectedPowerUps.ToArray();
        }

        public void AddNewPowerUp(PowerUp powerUp)
        {
            if (CurrentPowerUps.Contains(powerUp))
            {
                powerUp.Upgrade();
            }

            powerUp.Use();

            // don't add existent powerup
            for (int i = 0; i < CurrentPowerUps.Count; i++)
            {
                if (CurrentPowerUps[i] == powerUp)
                {
                    return;
                }
            }

            CurrentPowerUps.Add(powerUp);
        }

        public bool HasPowerUp(PowerUp powerUp)
        {
            foreach (var p in CurrentPowerUps)
            {
                if (powerUp == p)
                {
                    return true;
                }
            }

            return false;
        }
    }
}



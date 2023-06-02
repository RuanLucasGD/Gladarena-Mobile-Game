using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class PowerUpManager : MonoBehaviour
    {
        public List<PowerUp> AllPowerUps;
        public List<PowerUp> CurrentPowerUps;

        public PowerUp[] GenerateRandomPowerUpsList(int amount)
        {
            amount = Mathf.Min(amount, AllPowerUps.Count);

            var _addedAmount = 0;
            var _allPowerUpsToGetAmount = AllPowerUps.Count;
            var _selectedPowerUps = new List<PowerUp>();

            PowerUp _RandomPowerUp() => AllPowerUps[Random.Range(0, AllPowerUps.Count)];

            // gets a list of various random powerups
            while (_addedAmount < amount && _allPowerUpsToGetAmount > 0)
            {
                var randomPowerUp = _RandomPowerUp();

                foreach (var p in _selectedPowerUps)
                {
                    while (randomPowerUp == p)
                    {
                        randomPowerUp = _RandomPowerUp();
                    }
                }

                var _hasPowerUp = CurrentPowerUps.Contains(randomPowerUp);

                // remove powerups that cannot be obtained because its
                // already at a full level
                if (randomPowerUp.IsFullUpgrade() && !_hasPowerUp)
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
            if (HasPowerUp(powerUp))
            {
                powerUp.Upgrade();
             
                if (!powerUp.IsFullUpgrade())
                {
                    powerUp.OnUpgrated.Invoke();
                    if (powerUp.IsFullUpgrade())
                    {
                        powerUp.OnFullUpgrated.Invoke();
                    }
                }
            }
            else
            {
                powerUp.OnSetupPowerUp.Invoke();
            }

            powerUp.Use();
            powerUp.OnUsePowerUp.Invoke();

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
            return CurrentPowerUps.Contains(powerUp);
        }
    }
}



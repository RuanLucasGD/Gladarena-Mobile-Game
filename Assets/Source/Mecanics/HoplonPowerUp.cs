using Assets.Source.Mecanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    [CreateAssetMenu(fileName = "HoplonPowerUp", menuName = "PowerUps/HoplonPowerUp", order = 1)]
    public class HoplonPowerUp : PowerUp
    {
        [System.Serializable]
        public class Level
        {
            public HoplonPowerUpShield Model;

            public string[] CustomUpgradeMessages;
        }

        public Level[] Levels;

        public HoplonPowerUpShield CurrentHoplonShield { get; private set; }

        public Level CurrentLevel => Levels[CurrentLevelIndex];

        protected override void OnEnable()
        {
            base.OnEnable();
            CurrentHoplonShield = null;
        }

        public override bool IsFullUpgrade()
        {
            return CurrentLevelIndex > Levels.Length - 1;
        }

        public override void Use()
        {
            // set new hoplon shield 
            if (CurrentLevel.Model != null)
            {
                if (CurrentHoplonShield)
                {
                    Destroy(CurrentHoplonShield.gameObject);
                }

                var _player = GameManager.Instance.Player;
                CurrentHoplonShield = Instantiate(CurrentLevel.Model, _player.transform);
            }
        }

        public override void Upgrade()
        {
            base.Upgrade();
            CurrentLevelIndex = Mathf.Clamp(CurrentLevelIndex, 0, Levels.Length - 1);
        }

        public override string UpgradeInfo()
        {
            var _info = "";

            foreach (var m in CurrentLevel.CustomUpgradeMessages)
            {
                _info += m + "\n";
            }

            return _info;
        }
    }
}



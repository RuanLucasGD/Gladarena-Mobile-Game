using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    [CreateAssetMenu(fileName = "AttackPowerUp", menuName = "PowerUps/AttackPowerUp", order = 1)]
    public class AttackPowerUp : PowerUp
    {
        [System.Serializable]
        public struct Level
        {
            [Min(0)] public int AddAttacks;
            [Min(0)] public float RangeMultiplier;
            [Min(0)] public float DamageMultiplier;

            [Range(0, 100)]
            public float CooldownMultiplier;
        }

        public Level[] Levels;
        public Level CurrentLevel => Levels[CurrentLevelIndex];

        private void OnEnable()
        {
            CurrentLevelIndex = 0;
        }

        public override bool IsFullUpgrade()
        {
            return CurrentLevelIndex >= Levels.Length - 1;
        }

        public override void Upgrade()
        {
            if (IsFullUpgrade())
            {
                return;
            }

            base.Upgrade();
            CurrentLevelIndex = Mathf.Clamp(CurrentLevelIndex, 0, Levels.Length - 1);
        }

        public override void Use()
        {
            var _player = GameManager.Instance.Player;
            _player.Weapon.SequencialAttacks += CurrentLevel.AddAttacks;
            _player.Weapon.AttackDamageMultiplier *= 1 + (CurrentLevel.DamageMultiplier / 100f);
            _player.Weapon.AttackDistanceMultiplier *= 1 + (CurrentLevel.RangeMultiplier / 100f);
            _player.Weapon.AttackRate *= Mathf.Max(1 - (CurrentLevel.CooldownMultiplier / 100f), 0);
        }

        public override string UpgradeInfo()
        {
            if (IsFullUpgrade())
            {
                return "full upgrade";
            }

            var _level = Levels[CurrentLevelIndex];
            var _formatedInfo = "";

            if (_level.AddAttacks > 0) _formatedInfo += $"+{_level.AddAttacks} attack\n";
            if (_level.RangeMultiplier > 0) _formatedInfo += $"+{_level.RangeMultiplier * 100}% attack range\n";
            if (_level.DamageMultiplier > 0) _formatedInfo += $"+{_level.DamageMultiplier * 100}% damage\n";
            if (_level.CooldownMultiplier > 0) _formatedInfo += $"-{_level.CooldownMultiplier * 100}% cooldown\n";

            return _formatedInfo;
        }
    }
}
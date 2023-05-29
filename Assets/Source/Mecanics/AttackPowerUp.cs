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

            public GameObject WeaponModel;
        }

        public Level[] Levels;
        public Level CurrentLevel => Levels[CurrentLevelIndex];

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
            var _playerWeapon = _player.Weapon.WeaponObject;

            _player.Weapon.SequencialAttacks += CurrentLevel.AddAttacks;
            _player.Weapon.AttackDamageMultiplier *= 1 + (CurrentLevel.DamageMultiplier / 100f);
            _player.Weapon.AttackDistanceMultiplier *= 1 + (CurrentLevel.RangeMultiplier / 100f);
            _player.Weapon.AttackRate *= Mathf.Max(1 - (CurrentLevel.CooldownMultiplier / 100f), 0);
           
            if (_playerWeapon && CurrentLevel.WeaponModel)
            {
                var _oldWeaponModel = _playerWeapon.Mesh.gameObject;
                var _newWeaponModel = Instantiate(CurrentLevel.WeaponModel, _oldWeaponModel.transform.parent);
                Destroy(_oldWeaponModel);
            }
        }

        public override string UpgradeInfo()
        {
            if (IsFullUpgrade())
            {
                return "full upgrade";
            }

            var _level = Levels[CurrentLevelIndex];
            var _formatedInfo = "";

            if (_level.AddAttacks > 0) _formatedInfo += $"+ {_level.AddAttacks} attack \n";
            if (_level.RangeMultiplier > 0) _formatedInfo += $"+{_level.RangeMultiplier}% attack area\n";
            if (_level.DamageMultiplier > 0) _formatedInfo += $"+{_level.DamageMultiplier}% damage\n";
            if (_level.CooldownMultiplier > 0) _formatedInfo += $"-{_level.CooldownMultiplier}% cooldown\n";

            return _formatedInfo;
        }
    }
}
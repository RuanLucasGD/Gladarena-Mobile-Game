using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    [CreateAssetMenu(fileName = "AresArmy", menuName = "PowerUps/AresArmy", order = 1)]
    public class AresArmyPowerUp : PowerUp
    {
        [System.Serializable]
        public class PlayerCloneBehaviour
        {
            public float AttackDistance;
            public float FindEnemiesDistance;
        }

        [System.Serializable]
        public class Level
        {
            public int ClonesAmount;
            public PlayerCharacter CustomPlayer;
        }

        public string PlayerTag;
        public PlayerCloneBehaviour CloneBehaviour;
        public Level[] Levels;

        private List<PlayerCharacter> _currentPlayerClones;

        protected override void OnEnable()
        {
            base.OnEnable();

            var _isPlaying = true;
#if UNITY_EDITOR
            _isPlaying = EditorApplication.isPlaying;
#else
            _isPlaying = Application.isPlaying;
#endif
            if (_isPlaying)
            {
                return;
            }

            OnSetupPowerUp.AddListener(SetupPowerUp);
            _currentPlayerClones = new List<PlayerCharacter>();
        }

        public override bool IsFullUpgrade()
        {
            return CurrentLevelIndex >= Levels.Length - 1;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            CurrentLevelIndex = Mathf.Clamp(CurrentLevelIndex, 0, Levels.Length - 1);
        }

        public override string UpgradeInfo()
        {
            return "";
        }

        public override void Use()
        {
            RecreateAllClones();
        }

        private void SetupPowerUp()
        {
            if (GameProgressionManager.Instance)
            {
                GameProgressionManager.Instance.OnStartLevel.AddListener(l => RecreateAllClones());
            }
        }

        private void CreateClone()
        {
            var _player = GameObject.FindWithTag(PlayerTag).GetComponent<PlayerCharacter>();

            var _randomSpawnDirection = new Vector3(Random.Range(-1, 1),0, (Random.Range(-1, 1)));
            _randomSpawnDirection /= _randomSpawnDirection.magnitude;
            _randomSpawnDirection *= 3;

            var _randomSpawnPos = _player.transform.position + _randomSpawnDirection;
            var _playerClone = Instantiate(_player, _randomSpawnPos, Quaternion.identity);

            _playerClone.tag = "Untagged";
            _playerClone.Weapon.AttackLengthMultiplier = 1;
            _playerClone.Weapon.AttackRateMultiplier = 1;
            _playerClone.Weapon.AttackDamageMultiplier = 1;
            _playerClone.Weapon.AttackDistanceMultiplier = 1;
            _playerClone.Weapon.SequencialAttacks = 1;

            _playerClone.OnDamaged.RemoveAllListeners();
            _playerClone.OnDeath.RemoveAllListeners();
            _playerClone.OnRevive.RemoveAllListeners();
            _playerClone.OnSetWeapon.RemoveAllListeners();

            _playerClone.ResetLife();

            var _playerClonesPowerUp = _playerClone.GetComponentsInChildren<PowerUpItem>();

            if (_playerClonesPowerUp.Length > 0)
            {
                for (int i = 0; i < _playerClonesPowerUp.Length; i++)
                {
                    Destroy(_playerClonesPowerUp[i].gameObject);
                }
            }

            var _playerCloneAi = _playerClone.gameObject.AddComponent<PlayerCloneAI>();

            _playerCloneAi.Clone = _playerClone;
            _playerCloneAi.PowerUpController = this;
            _playerClone.OnDeath.AddListener(() => _currentPlayerClones.Remove(_playerClone));

            _currentPlayerClones.Add(_playerClone);
        }

        public void RecreateAllClones()
        {
            while (_currentPlayerClones.Count < Levels[CurrentLevelIndex].ClonesAmount)
            {
                CreateClone();
            }
        }

    }
}



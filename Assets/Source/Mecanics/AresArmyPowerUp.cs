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
            var _player = GameManager.Instance.Player;

            // spawn clone
            var _randomSpawnDirection = new Vector3(Random.Range(-1f, 1f),0, (Random.Range(-1f, 1f)));
            _randomSpawnDirection /= _randomSpawnDirection.magnitude;
            _randomSpawnDirection *= 3;

            var _randomSpawnPos = _player.transform.position + _randomSpawnDirection;
            var _playerClone = Instantiate(_player, _randomSpawnPos, Quaternion.identity);

            // reset clone
            _playerClone.tag = "Untagged";
            _playerClone.Weapon.AttackLengthMultiplier = 1;
            _playerClone.Weapon.AttackRateMultiplier = 1;
            _playerClone.Weapon.AttackDamageMultiplier = 1;
            _playerClone.Weapon.AttackDistanceMultiplier = 1;
            _playerClone.Weapon.SequencialAttacks = 1;
            _playerClone.ResetLife();

            _playerClone.OnDamaged.RemoveAllListeners();
            _playerClone.OnDeath.RemoveAllListeners();
            _playerClone.OnRevive.RemoveAllListeners();
            _playerClone.OnSetWeapon.RemoveAllListeners();
            _playerClone.OnDeath.AddListener(() => _currentPlayerClones.Remove(_playerClone));

            var _playerClonesPowerUp = _playerClone.GetComponentsInChildren<PowerUpItem>();

            // removing all powerups
            if (_playerClonesPowerUp.Length > 0)
            {
                for (int i = 0; i < _playerClonesPowerUp.Length; i++)
                {
                    Destroy(_playerClonesPowerUp[i].gameObject);
                }
            }

            // adding AI to clone
            var _playerCloneAi = _playerClone.gameObject.AddComponent<PlayerCloneAI>();
            _playerCloneAi.Clone = _playerClone;
            _playerCloneAi.PowerUpController = this;

            // finally
            _currentPlayerClones.Add(_playerClone);
        }

        public void RecreateAllClones()
        {
            for (int i = 0; i < _currentPlayerClones.Count; i++)
            {
                _currentPlayerClones[i].ResetLife();
            }

            while (_currentPlayerClones.Count < Levels[CurrentLevelIndex].ClonesAmount)
            {
                CreateClone();
            }
        }
    }
}



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

            [Header("Attack")]
            public float AttackDamageMultiplier;
            public float AttackDistanceMultiplier;
            public float AttackSpeedMultiplier;
            public float AttackRateMultiplier;
            public int SequencialAttacks;

            [Header("Clone")]
            public float LifeMultiplier;
            public float MoveSpeedMultiplier;
        }

        public string PlayerTag;
        public PlayerCloneBehaviour CloneBehaviour;
        public Level[] Levels;

        private List<PlayerCloneAI> _currentPlayerClones;

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
            _currentPlayerClones = new List<PlayerCloneAI>();
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
            var _randomSpawnDirection = new Vector3(Random.Range(-1f, 1f), 0, (Random.Range(-1f, 1f)));
            _randomSpawnDirection /= _randomSpawnDirection.magnitude;
            _randomSpawnDirection *= 3;

            var _randomSpawnPos = _player.transform.position + _randomSpawnDirection;
            var _playerClone = Instantiate(_player, _randomSpawnPos, Quaternion.identity);

            // setting default behaviour;
            _playerClone.tag = "Untagged";
            
            _playerClone.OnDamaged.RemoveAllListeners();
            _playerClone.OnDeath.RemoveAllListeners();
            _playerClone.OnRevive.RemoveAllListeners();
            _playerClone.OnSetWeapon.RemoveAllListeners();

            // removing all powerups from clone
            var _playerClonesPowerUp = _playerClone.GetComponentsInChildren<PowerUpItem>();

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
            _playerClone.OnDeath.AddListener(() => _currentPlayerClones.Remove(_playerCloneAi));

            ResetClone(_playerCloneAi);

            // finally
            _currentPlayerClones.Add(_playerCloneAi);
        }

        private void ResetClone(PlayerCloneAI cloneAi)
        {
            var _playerClone = cloneAi.Clone;
            var _currentLevel = Levels[CurrentLevelIndex];
            
            _playerClone.Weapon.AttackLengthMultiplier = _currentLevel.AttackSpeedMultiplier;
            _playerClone.Weapon.SequencialAttacks = _currentLevel.SequencialAttacks;
            _playerClone.Weapon.AttackDamageMultiplier = _currentLevel.AttackDamageMultiplier;
            _playerClone.Weapon.AttackDistanceMultiplier = _currentLevel.AttackDistanceMultiplier;
            _playerClone.Weapon.AttackRate = _currentLevel.AttackRateMultiplier;

            _playerClone.Life.LifeMultiplier = _currentLevel.LifeMultiplier;
            _playerClone.Movimentation.MoveSpeedMultiplier = _currentLevel.MoveSpeedMultiplier;

            _playerClone.ResetLife();
        }

        public void RecreateAllClones()
        {
            while (_currentPlayerClones.Count < Levels[CurrentLevelIndex].ClonesAmount)
            {
                CreateClone();
            }

            for (int i = 0; i < _currentPlayerClones.Count; i++)
            {
                ResetClone(_currentPlayerClones[i]);
            }
        }
    }
}



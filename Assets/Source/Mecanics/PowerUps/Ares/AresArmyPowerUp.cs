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
            public string ClonesLayer;
        }

        [System.Serializable]
        public class Level
        {
            public int ClonesAmount;
            public PlayerCharacter CustomPlayer;

            [Space]
            public bool FollowPlayer;

            [Header("On Start Level")]
            public bool DeleteAllClones;

            [Header("On Update")]
            public float GenerateInterval;

            [Header("Attack")]
            public float AttackDamageMultiplier;
            public float AttackDistanceMultiplier;
            public float AttackLengthMultiplier;
            public float AttackRateMultiplier;
            public int SequencialAttacks;

            [Header("Clone")]
            public float LifeMultiplier;
            public float MoveSpeedMultiplier;

            [Header("Explosion")]
            public bool ExplodeOnDeath;
            public float ExplosionRange;
            public float ExplosionDamage;
            public GameObject ExplosionPrefab;
            public float ExplosionPrefabLifeTime;

            [Header("Dash Mode")]
            public bool UseDashModeOnDeath;
            public float DashTime;
        }

        public string PlayerTag;
        public PlayerCloneBehaviour CloneBehaviour;
        public Level[] Levels;

        private List<AresArmyPlayerCloneAI> _currentPlayerClones;
        private AresArmyItem _powerUpItem;

        private bool _destroyClonesOnStartLevel;

        public int CurrentClonesAmount => _currentPlayerClones.Count;

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
            _currentPlayerClones = new List<AresArmyPlayerCloneAI>();
        }

        public override bool IsFullUpgrade()
        {
            return CurrentLevelIndex >= Levels.Length - 1 && PowerUpManager.Instance.HasPowerUp(this);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            _destroyClonesOnStartLevel = true;
            CurrentLevelIndex = Mathf.Clamp(CurrentLevelIndex, 0, Levels.Length - 1);
        }

        public override string UpgradeInfo()
        {
            var _isFirstLevel = !PowerUpManager.Instance.HasPowerUp(this);

            if (_isFirstLevel)
            {
                return $"{Levels[CurrentLevelIndex].ClonesAmount} player clone";
            }

            var _lastLevel = Levels[CurrentLevelIndex];
            var _upgradedLevel = Levels[Mathf.Min(CurrentLevelIndex + 1, Levels.Length - 1)];

            var _message = "";

            var _clonesAmountUpgrade = _upgradedLevel.ClonesAmount - _lastLevel.ClonesAmount;
            var _attackDistanceUpgrade = _upgradedLevel.AttackDistanceMultiplier - _lastLevel.AttackDistanceMultiplier;
            var _attackDamageUpgrade = _upgradedLevel.AttackDamageMultiplier - _lastLevel.AttackDamageMultiplier;
            var _attackRateUpgrade = _upgradedLevel.AttackRateMultiplier - _lastLevel.AttackRateMultiplier;
            var _attackLengthUpgrade = _upgradedLevel.AttackLengthMultiplier - _lastLevel.AttackLengthMultiplier;
            var _sequencialAttacksUpgrade = _upgradedLevel.SequencialAttacks - _lastLevel.SequencialAttacks;

            var _moveSpeedUpgrade = _upgradedLevel.MoveSpeedMultiplier - _lastLevel.MoveSpeedMultiplier;
            var _lifeUpgrade = _upgradedLevel.LifeMultiplier - _lastLevel.LifeMultiplier;

            var _addedExplosion = !_lastLevel.ExplodeOnDeath && _upgradedLevel.ExplodeOnDeath;
            var _addedDashOnDeath = !_lastLevel.UseDashModeOnDeath && _upgradedLevel.UseDashModeOnDeath;

            var _explosionDistance = (_upgradedLevel.ExplosionRange - _lastLevel.ExplosionRange) / _lastLevel.ExplosionRange;
            var _explosionDamage = (_upgradedLevel.ExplosionDamage - _lastLevel.ExplosionDamage) / _lastLevel.ExplosionDamage;

            if (_clonesAmountUpgrade != 0) _message += $"{(_clonesAmountUpgrade > 0f ? "+" : "-")}{Mathf.Abs(_clonesAmountUpgrade)} clone\n";
            if (_sequencialAttacksUpgrade != 0) _message += $"{(_sequencialAttacksUpgrade > 0 ? "+" : "-")}{Mathf.Abs(_sequencialAttacksUpgrade)} sequencial attacks\n";
            if (_attackDistanceUpgrade != 0) _message += $"{(_attackDistanceUpgrade > 0f ? "+" : "-")}{Mathf.Abs(_attackDistanceUpgrade * 100)}% attack distance\n";
            if (_attackDamageUpgrade != 0) _message += $"{(_attackDamageUpgrade > 0f ? "+" : "-")}{Mathf.Abs(_attackDamageUpgrade * 100)}% attack damage\n";
            if (_attackRateUpgrade != 0) _message += $"{(_attackRateUpgrade > 0f ? "+" : "-")}{Mathf.Abs(_attackRateUpgrade * 100)}% attack rate\n";
            if (_attackLengthUpgrade != 0) _message += $"{(_attackLengthUpgrade > 0f ? "+" : "-")}{Mathf.Abs(_attackLengthUpgrade * 100)}% attack length\n";

            if (_addedExplosion)
            {
                _message += "explode on clone death";
            }
            else
            {
                if (_explosionDistance != 0) _message += $"{(_explosionDistance > 0f ? "+" : "-")}{Mathf.Abs(_explosionDistance * 100)}% explosion distance\n";
                if (_explosionDamage != 0) _message += $"{(_explosionDamage > 0f ? "+" : "-")}{Mathf.Abs(_explosionDamage * 100)}% explosion damage\n";
            }

            if (_addedDashOnDeath)
            {
                _message += "add dash";
            }

            if (_moveSpeedUpgrade != 0) _message += $"{(_moveSpeedUpgrade > 0f ? "+" : "-")}{Mathf.Abs(_moveSpeedUpgrade * 100)}% move speed\n";
            if (_lifeUpgrade != 0) _message += $"{(_lifeUpgrade > 0f ? "+" : "-")}{Mathf.Abs(Mathf.RoundToInt(_lifeUpgrade * 100))}% max life\n";

            return _message;
        }

        public override void Use()
        {
            if (!_powerUpItem)
            {
                _powerUpItem = new GameObject(name + " Item").AddComponent<AresArmyItem>();
                _powerUpItem.powerUp = this;
            }

            if (_destroyClonesOnStartLevel)
            {
                _destroyClonesOnStartLevel = false;
                DestroyAndCreateClones();
            }
            else
            {
                RecreateAllClones();
            }
        }

        private void SetupPowerUp()
        {
            if (GameProgressionManager.Instance)
            {
                GameProgressionManager.Instance.OnStartLevel.AddListener(l => RecreateAllClones());
            }
        }

        private AresArmyPlayerCloneAI CreateClone()
        {
            var _originalPlayer = GameManager.Instance.Player;

            var _randomSpawnDirection = new Vector3(Random.Range(-1f, 1f), 0, (Random.Range(-1f, 1f)));
            _randomSpawnDirection /= _randomSpawnDirection.magnitude;
            _randomSpawnDirection *= 3;

            var _randomSpawnPos = _originalPlayer.transform.position + _randomSpawnDirection;
            return CreateClone(_randomSpawnPos);
        }

        private AresArmyPlayerCloneAI CreateClone(Vector3 position)
        {
            var _originalPlayer = GameManager.Instance.Player;
            var _hasCustomPlayerModel = Levels[CurrentLevelIndex].CustomPlayer != null;
            var _playerModel = _hasCustomPlayerModel ? Levels[CurrentLevelIndex].CustomPlayer : _originalPlayer;

            // spawn clone
            var _playerClone = Instantiate(_playerModel, position, Quaternion.identity);

            _playerClone.tag = "Untagged";
            _playerClone.Life.AutoDestroyOnDeathDelay = 10f;

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
            var _playerCloneAi = _playerClone.GetComponent<AresArmyPlayerCloneAI>();

            if (!_playerCloneAi)
            {
                _playerCloneAi = _playerClone.gameObject.AddComponent<AresArmyPlayerCloneAI>();
            }

            _playerCloneAi.Clone = _playerClone;
            _playerCloneAi.PowerUpController = this;

            ResetClone(_playerCloneAi);
            SetupCloneEvents(_playerCloneAi);

            // finally
            _currentPlayerClones.Add(_playerCloneAi);
            return _playerCloneAi;
        }

        private void ResetClone(AresArmyPlayerCloneAI cloneAi)
        {
            var _playerClone = cloneAi.Clone;
            var _currentLevel = Levels[CurrentLevelIndex];

            cloneAi.ExplodeOnDeath = _currentLevel.ExplodeOnDeath;
            cloneAi.UseDashModeOnDeath = _currentLevel.UseDashModeOnDeath;
            cloneAi.DashTimeOnDeath = _currentLevel.DashTime;

            _playerClone.enabled = true;
            _playerClone.gameObject.layer = LayerMask.NameToLayer(CloneBehaviour.ClonesLayer);

            _playerClone.Weapon.AttackLengthMultiplier = _currentLevel.AttackLengthMultiplier;
            _playerClone.Weapon.SequencialAttacks = _currentLevel.SequencialAttacks;
            _playerClone.Weapon.AttackDamageMultiplier = _currentLevel.AttackDamageMultiplier;
            _playerClone.Weapon.AttackDistanceMultiplier = _currentLevel.AttackDistanceMultiplier;
            _playerClone.Weapon.AttackRate = _currentLevel.AttackRateMultiplier;

            _playerClone.Life.LifeMultiplier = _currentLevel.LifeMultiplier;
            _playerClone.Movimentation.MoveSpeedMultiplier = _currentLevel.MoveSpeedMultiplier;

            _playerClone.ResetLife();
        }

        private void SetupCloneEvents(AresArmyPlayerCloneAI playerCloneAI)
        {
            playerCloneAI.Clone.OnDamaged.RemoveAllListeners();
            playerCloneAI.Clone.OnDeath.RemoveAllListeners();
            playerCloneAI.Clone.OnRevive.RemoveAllListeners();
            playerCloneAI.Clone.OnSetWeapon.RemoveAllListeners();

            playerCloneAI.Clone.OnDeath.AddListener(() =>
            {
                // wait finish all clone logic
                // because the clone can be revived
                // when is killed.
                IEnumerator _WaitFinishAllCloneLogic()
                {
                    yield return new WaitForEndOfFrame();

                    if (!playerCloneAI.DashModeEnabled)
                    {
                        _currentPlayerClones.Remove(playerCloneAI);
                    }
                }

                playerCloneAI.StartCoroutine(_WaitFinishAllCloneLogic());
            });
        }

        public void AddExplosion(Vector3 position)
        {
            var _level = Levels[CurrentLevelIndex];

            if (!_level.ExplosionPrefab)
            {
                return;
            }

            // getting all near colliders and spawn explosion particle
            var _explosionEffect = Instantiate(Levels[CurrentLevelIndex].ExplosionPrefab, position, Quaternion.identity);
            var _colliders = Physics.OverlapSphere(position, _level.ExplosionRange);
            var _enemies = new List<EnemyBase>();

            // getting enemies
            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i].TryGetComponent<EnemyBase>(out var _enemy))
                {
                    _enemies.Add(_enemy);
                }
            }

            // apply damage on enemies
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].AddDamage(_level.ExplosionDamage);
            }

            Destroy(_explosionEffect, _level.ExplosionPrefabLifeTime);
        }

        public void DestroyAndCreateClones()
        {
            for (int i = 0; i < _currentPlayerClones.Count; i++)
            {
                Destroy(_currentPlayerClones[i].gameObject);
            }

            _currentPlayerClones.Clear();

            RecreateAllClones();
        }

        public void RecreateAllClones()
        {
            var _positionAngle = 0f;
            var _currentClonesAmount = _currentPlayerClones.Count;
            var _maxClonesAmount = Levels[CurrentLevelIndex].ClonesAmount;

            for (int i = _currentClonesAmount; i < _maxClonesAmount; i++)
            {
                AresArmyPlayerCloneAI _newClone = null;

                if (!Levels[CurrentLevelIndex].FollowPlayer)
                {
                    _newClone = CreateClone();
                    continue;
                }

                // spawn clone around the player
                var _direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * _positionAngle), 0, Mathf.Cos(Mathf.Deg2Rad * _positionAngle)) * 2;
                var _position = GameManager.Instance.Player.transform.position + _direction;
                _positionAngle += (360 / _maxClonesAmount);

                _newClone = CreateClone(_position);

                _newClone.FollowPlayerOffset = _direction;
                _newClone.FollowPlayer = Levels[CurrentLevelIndex].FollowPlayer;
            }

            for (int i = 0; i < _maxClonesAmount; i++)
            {
                ResetClone(_currentPlayerClones[i]);
            }
        }
    }
}
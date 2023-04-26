using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [System.Serializable]
        public class LevelInfo
        {
            public bool IsUpgraded;
            public int LevelIndex;

            public float LifeBase;
            public float DamageBase;
            public float VelocityBase;

            public float LifeUpgrade;
            public float DamageUpgrade;
            public float VelocityUpgrade;
        }

        [System.Serializable]
        public class LevelProgression
        {
            [Header("Horder 1")]
            public float LifeBase;
            public float DamageBase;
            public float VelocityBase;


            [Header("Horder 2")]
            public float LifeUpgrade;
            public float DamageUpgrade;
            public float VelocityUpgrade;
        }

        [System.Serializable]
        public class EnemyType
        {
            public Enemy Prefab;
            public int StartSpawnLevel;
            public int EndSpawnLevel;
        }

        [System.Serializable]
        public class SpawnSettings
        {
            public float SpawnRate;
            public Transform[] SpawnPoints;
            public LayerMask ObstaclesLayer;

            [Space]

            public int MaxAsyncSpawns;

            [Space]

            public int BossLevelInterval;
        }

        public bool CanSpawn;
        public Transform Follow;
        public SpawnSettings Spawn;
        public EnemyType[] Enemies;
        public Enemy[] Boses;
        public LevelProgression FirstLevel;
        public LevelProgression Progression;

        [Space]
        public UnityEvent<Enemy> OnEnemySpawned;
        public UnityEvent<Enemy> OnEnemyKilled;
        public UnityEvent<int> OnChangeLevel;

        public List<LevelInfo> CurrentLevels;

        public int CurrentLevelIndex;

        public bool IsOnBossLevel { get; private set; }
        public bool BossSpawned { get; private set; }

        public bool HasEnemiesOnScene => _enemiesSpawnCount > 0;

        private int _bossLevelInterval;
        private int _currentBossIndex;

        private int _enemiesSpawnCount;

        void Start()
        {
            InvokeRepeating(nameof(SpawnEnemy), Spawn.SpawnRate, Spawn.SpawnRate);

            SetLevel(0);
        }

        void Update()
        {
            transform.position = Follow.position;
        }

        private void SpawnEnemy()
        {
            if (!CanSpawn)
            {
                return;
            }

            if (IsOnBossLevel)
            {
                // spawn only once enemy
                // spawn boss onfy if doesn't have enemies on scene
                if (BossSpawned || HasEnemiesOnScene)
                {
                    return;
                }

                var _boss = SpawnEnemy(Boses[_currentBossIndex]);
                BossSpawned = true;

                _currentBossIndex++;
                if (_currentBossIndex >= Boses.Length - 1)
                {
                    _currentBossIndex = 0;
                }
            }
            else
            {
                foreach (var e in Enemies)
                {
                    if (CurrentLevelIndex >= e.StartSpawnLevel && CurrentLevelIndex <= e.EndSpawnLevel)
                    {
                        SpawnEnemyOnLevel(e);
                    }
                }
            }
        }

        private Enemy SpawnEnemy(Enemy enemy)
        {
            var _randomPos = Spawn.SpawnPoints[Random.Range(0, Spawn.SpawnPoints.Length - 1)].position;
            var _newEnemy = Instantiate(enemy, _randomPos, Quaternion.identity);
            _newEnemy.OnSpawned.AddListener(() => OnEnemySpawned.Invoke(_newEnemy));
            _newEnemy.OnKilled.AddListener(() => OnEnemyKilled.Invoke(_newEnemy));
            return _newEnemy;
        }

        private void SpawnEnemyOnLevel(EnemyType enemy)
        {
            var _newEnemy = SpawnEnemy(enemy.Prefab);

            var _level = CurrentLevels[CurrentLevels.Count - 1];
            var _damage = _level.IsUpgraded ? _level.DamageUpgrade : _level.DamageBase;
            var _velocity = _level.IsUpgraded ? _level.VelocityUpgrade : _level.VelocityBase;
            var _life = _level.IsUpgraded ? _level.LifeUpgrade : _level.LifeBase;

            _newEnemy.AttackDamage *= _damage;
            _newEnemy.MaxLife *= _life;
            _newEnemy.MoveSpeed *= _velocity;

            _newEnemy.OnSpawned.AddListener(() => _enemiesSpawnCount++);
            _newEnemy.OnKilled.AddListener(() => _enemiesSpawnCount--);
        }

        private void SetLevel(int level)
        {
            var _newLevel = new LevelInfo();

            if (CurrentLevels.Count >= 1)
            {
                var _lastLevel = CurrentLevels[CurrentLevels.Count - 1];
                _newLevel.DamageBase = _lastLevel.DamageBase + Progression.DamageBase;
                _newLevel.LifeBase = _lastLevel.LifeBase + Progression.LifeBase;
                _newLevel.VelocityBase = _lastLevel.DamageBase + Progression.VelocityBase;

                _newLevel.DamageUpgrade = _lastLevel.DamageUpgrade + Progression.DamageUpgrade;
                _newLevel.LifeUpgrade = _lastLevel.LifeUpgrade + Progression.LifeUpgrade;
                _newLevel.VelocityUpgrade = _lastLevel.VelocityUpgrade + Progression.VelocityUpgrade;
            }
            else
            {
                _newLevel.DamageBase = FirstLevel.DamageBase;
                _newLevel.LifeBase = FirstLevel.LifeBase;
                _newLevel.VelocityBase = FirstLevel.VelocityBase;

                _newLevel.DamageUpgrade = FirstLevel.DamageUpgrade;
                _newLevel.LifeUpgrade = FirstLevel.LifeUpgrade;
                _newLevel.VelocityUpgrade = FirstLevel.VelocityUpgrade;
            }

            _bossLevelInterval = level;

            CurrentLevels.Add(_newLevel);
            CurrentLevelIndex = level;

            while (CurrentLevels.Count > Spawn.MaxAsyncSpawns)
            {
                CurrentLevels.Remove(CurrentLevels[0]);
            }
        }

        public void SetNextLevel()
        {
            BossSpawned = false;
            CurrentLevelIndex++;
            _bossLevelInterval++;

            SetLevel(CurrentLevelIndex);
            OnChangeLevel.Invoke(CurrentLevelIndex);

            if (IsOnBossLevel)
            {
                IsOnBossLevel = false;
                _bossLevelInterval = 0;
            }
            else if (_bossLevelInterval >= Spawn.BossLevelInterval)
            {
                IsOnBossLevel = true;
            }
        }
    }
}
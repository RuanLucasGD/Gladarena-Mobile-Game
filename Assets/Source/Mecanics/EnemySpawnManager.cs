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
        }

        public bool CanSpawn;
        public Transform Follow;
        public SpawnSettings Spawn;
        public EnemyType[] Enemies;
        public LevelProgression FirstLevel;
        public LevelProgression Progression;

        [Space]
        public UnityEvent<Enemy> OnEnemySpawned;
        public UnityEvent<Enemy> OnEnemyKilled;

        public List<LevelInfo> CurrentLevels;

        public int CurrentLevelIndex;

        void Start()
        {
            InvokeRepeating(nameof(SpawnEnemy), Spawn.SpawnRate, Spawn.SpawnRate);

            SetLevel(0);
            SetNextLevel();
            SetNextLevel();
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

            foreach (var e in Enemies)
            {
                if (e.StartSpawnLevel <= CurrentLevelIndex && e.EndSpawnLevel <= CurrentLevelIndex)
                {
                    SpawnEnemy(e);
                }
            }
        }

        public void SpawnEnemy(EnemyType enemy)
        {
            var _randomPos = Spawn.SpawnPoints[Random.Range(0, Spawn.SpawnPoints.Length - 1)].position;
            var _newEnemy = Instantiate(enemy.Prefab, _randomPos, Quaternion.identity);

            var _level = CurrentLevels[CurrentLevelIndex];
            var _damage = _level.IsUpgraded ? _level.DamageUpgrade : _level.DamageBase;
            var _velocity = _level.IsUpgraded ? _level.VelocityUpgrade : _level.VelocityBase;
            var _life = _level.IsUpgraded ? _level.LifeUpgrade : _level.LifeBase;

            _newEnemy.AttackDamage *= _damage;
            _newEnemy.MaxLife *= _life;
            _newEnemy.MoveSpeed *= _velocity;

            _newEnemy.OnSpawned.AddListener(() => OnEnemySpawned.Invoke(_newEnemy));
            _newEnemy.OnKilled.AddListener(() => OnEnemyKilled.Invoke(_newEnemy));
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

            CurrentLevels.Add(_newLevel);
            CurrentLevelIndex = level;

            while (CurrentLevels.Count > Spawn.MaxAsyncSpawns)
            {
                CurrentLevels.Remove(CurrentLevels[0]);
            }
        }

        public void SetNextLevel()
        {
            SetLevel(CurrentLevelIndex);
            CurrentLevelIndex++;
        }
    }
}
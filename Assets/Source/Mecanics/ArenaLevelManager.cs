using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class ArenaLevelManager : MonoBehaviour
    {
        [System.Serializable]
        public class EnemySpawn
        {
            public Character EnemyType;
            [Min(0)] public int MaxEnemiesInScene;
            [Min(0)] public int MaxEnemiesSpawned;
            [Min(1)] public int CheckInterval;

            [HideInInspector] public float SpawnTimer;
            [HideInInspector] public int EnemiesOnScene;
            [HideInInspector] public int SpawnsAmount;

            public bool SpawnCompleted => SpawnsAmount >= MaxEnemiesSpawned;
            public bool IsMissingEnemies => (EnemiesOnScene < MaxEnemiesInScene) && !SpawnCompleted;
        }

        [System.Serializable]
        public class Level
        {
            public string name;

            [Space]

            public EnemySpawn[] Enemies;
        }

        [System.Serializable]
        public class Arena
        {
            public Transform Center;
            public float ArenaSize;
        }

        public Arena ArenaCaracteristics;
        public Level[] Levels;

        public UnityEvent OnCompleteLevel;
        public UnityEvent OnCompleteGame;

        public int CurrentLevel { get; private set; }
        public bool GameWin { get; private set; }

        private void Start()
        {
        }

        private void Update()
        {
            UpdateEnemiesSpawn();
        }

        private void UpdateEnemiesSpawn()
        {
            if (Levels.Length == 0 && !GameWin)
            {
                return;
            }

            for (int i = 0; i < Levels[CurrentLevel].Enemies.Length; i++)
            {
                var _enemySpawn = Levels[CurrentLevel].Enemies[i];

                if (_enemySpawn.IsMissingEnemies)
                {
                    SpawnEnemy(_enemySpawn);
                }
            }
        }

        private void SpawnEnemy(EnemySpawn enemySpawn)
        {
            var _player = GameManager.Instance.Player;
            var _spawnPosition = GenerateRandomPointInArena();
            var _lookAtPlayer = Quaternion.LookRotation(_spawnPosition - _player.transform.position);
            var _enemyCharacter = Instantiate(enemySpawn.EnemyType.gameObject, _spawnPosition, _lookAtPlayer).GetComponent<Character>();

            enemySpawn.EnemiesOnScene++;
            enemySpawn.SpawnsAmount++;

            _enemyCharacter.OnDeath.AddListener(() => enemySpawn.EnemiesOnScene--);
            _enemyCharacter.OnDeath.AddListener(CheckLevelCompleted);
        }

        private Vector3 GenerateRandomPointInArena()
        {
            var _randomX = Random.Range(-ArenaCaracteristics.ArenaSize, ArenaCaracteristics.ArenaSize);
            var _randomZ = Random.Range(-ArenaCaracteristics.ArenaSize, ArenaCaracteristics.ArenaSize);
            var _rawPosition = Vector3.ClampMagnitude(new Vector3(_randomX, 0, _randomZ), ArenaCaracteristics.ArenaSize) + ArenaCaracteristics.Center.position;
            return _rawPosition;
        }

        private void CheckLevelCompleted()
        {
            var _isCompleted = true;

            foreach (var enemySpawn in Levels[CurrentLevel].Enemies)
            {
                if (!enemySpawn.SpawnCompleted)
                {
                    _isCompleted = false;
                    break;
                }
            }

            if (_isCompleted)
            {
                OnCompleteLevel.Invoke();


                Debug.Log("Level Completed");
                CheckGameCompleted();

                if (CurrentLevel < Levels.Length - 1)
                    CurrentLevel++;
            }
        }

        private void CheckGameCompleted()
        {
            if (CurrentLevel >= Levels.Length - 1)
            {
                Debug.Log("Game Completed");
                GameWin = true;
                OnCompleteGame.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            if (!ArenaCaracteristics.Center)
            {
                return;
            }

            Gizmos.DrawWireSphere(ArenaCaracteristics.Center.position, ArenaCaracteristics.ArenaSize);
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class ArenaLevelBasedManager : ArenaManager
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();

            UpdateEnemiesSpawn();
        }

        private void UpdateEnemiesSpawn()
        {
            if (!CanSpawnEnemies)
            {
                return;
            }

            foreach (var e in CurrentHorder.EnemiesSpawn)
            {
                if (e.IsMissingEnemies)
                {
                    SpawnEnemy(e);
                }
            }
        }

        private void SpawnEnemy(EnemySpawn enemySpawn)
        {
            var _player = GameManager.Instance.Player;
            var _spawnPosition = GenerateSpawnPoint();
            var _lookAtPlayer = Quaternion.LookRotation(_spawnPosition - _player.transform.position);
            var _enemyCharacter = Instantiate(enemySpawn.EnemyType.gameObject, _spawnPosition, _lookAtPlayer).GetComponent<Character>();

            enemySpawn.EnemiesOnScene++;
            enemySpawn.SpawnsAmount++;

            _enemyCharacter.OnDeath.AddListener(() => enemySpawn.EnemiesOnScene--);
            _enemyCharacter.OnDeath.AddListener(CheckGameProgression);
        }
    }
}
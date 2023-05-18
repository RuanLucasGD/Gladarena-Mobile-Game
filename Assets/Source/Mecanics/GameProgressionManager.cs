using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class GameProgressionManager : MonoBehaviour
    {
        [System.Serializable]
        public class EnemiesProgression
        {
            public EnemySpawnManager EnemySpawnManager;
            public int KillsToChangeLevel;
        }

        [System.Serializable]
        public class PowerUpsProgression
        {
            public PowerUpManager PowerUpManager;
            public int KillsToChangeLevel;
        }

        public EnemiesProgression EnemyLevelProgression;
        public PowerUpsProgression PowerUpsLevelProgression;
        public UnityEvent<int> OnChangeLevel;

        private int _enemyKilledAmount;

        public int CurrentLevelIndex;

        private void Start()
        {
            EnemyLevelProgression.EnemySpawnManager.OnEnemyKilled.AddListener(OnEnemyDeath);
            EnemyLevelProgression.EnemySpawnManager.OnEnemyKilled.AddListener(e => CheckEnemiesLevel());
            EnemyLevelProgression.EnemySpawnManager.OnChangeLevel.AddListener((l)  => OnChangeLevel.Invoke(l));
        }


        private void OnEnemyDeath(EnemyBase e)
        {
            if (e.Type == Enemy.EnemyType.Boss)
            {
                EnemyLevelProgression.EnemySpawnManager.SetNextLevel();
            }
        }

        private void CheckEnemiesLevel()
        {
            // don't count kills to not change level.
            // when is on boss level, change level only when kill the boss
            if (EnemyLevelProgression.EnemySpawnManager.IsOnBossLevel)
            {
                _enemyKilledAmount = 0;
                return;
            }

            _enemyKilledAmount++;

            if (_enemyKilledAmount > EnemyLevelProgression.KillsToChangeLevel)
            {
                CurrentLevelIndex++;
                _enemyKilledAmount = 0;
                EnemyLevelProgression.EnemySpawnManager.SetNextLevel();
            }
        }
    }
}

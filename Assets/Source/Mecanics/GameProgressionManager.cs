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

        public EnemiesProgression EnemyLevelProgression;
        public UnityEvent<int> OnChangeLevel;

        private int _enemiesKilledAmount;

        public int CurrentLevelIndex;

        private void Start()
        {
            EnemyLevelProgression.EnemySpawnManager.OnEnemyKilled.AddListener(e => CheckEnemiesLevel());
        }

        private void CheckEnemiesLevel()
        {
            _enemiesKilledAmount++;

            if (_enemiesKilledAmount > EnemyLevelProgression.KillsToChangeLevel)
            {
                CurrentLevelIndex++;
                _enemiesKilledAmount = 0;
                OnChangeLevel.Invoke(CurrentLevelIndex);
            }
        }
    }
}



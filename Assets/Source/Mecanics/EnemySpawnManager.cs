using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [System.Serializable]
        public class SpawnSettings
        {
            public float SpawnRate;
            public Transform[] SpawnPoints;
            public LayerMask ObstaclesLayer;
        }

        [System.Serializable]
        public class EnemyLevel
        {
            public Enemy EnemyPrefab;

            [Header("Level 1")]
            public float LifeBase;
            public float DamageBase;
            public float VelocityBase;


            [Header("Level 2")]
            public float LifeUpgrade;
            public float DamageUpgrade;
            public float VelocityUpgrade;

            public bool Upgraded { get; set; }
        }

        public Transform Follow;

        public SpawnSettings Spawn;
        public EnemyLevel[] Enemylevels;

        public int CurrentLevel { get; set; }

        void Start()
        {
            InvokeRepeating(nameof(SpawnEnemy), Spawn.SpawnRate, Spawn.SpawnRate);
        }

        void Update()
        {
            transform.position = Follow.position;
        }

        private void SpawnEnemy()
        {
            SpawnEnemy(Enemylevels[CurrentLevel]);
        }

        public void SpawnEnemy(EnemyLevel level)
        {
            var _randomPos = Spawn.SpawnPoints[Random.Range(0, Spawn.SpawnPoints.Length - 1)].position;
            var _newEnemy = Instantiate(level.EnemyPrefab, _randomPos, Quaternion.identity);
        }
    }
}
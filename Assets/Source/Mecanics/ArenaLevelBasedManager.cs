using UnityEngine;

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
            CheckGameProgression();
        }

        private void UpdateEnemiesSpawn()
        {
            if (!CanSpawnEnemies)
            {
                return;
            }

            foreach (var s in CurrentHorder.EnemiesSpawn)
            {
                if (s.IsMissingEnemies)
                {
                    SpawnEnemy(s);
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Game.Utils;

namespace Game.Mecanics
{
    public class ArenaManager : MonoBehaviour
    {
        [System.Serializable]
        public class EnemySpawn
        {
            [SerializeField] private string Name;

            public Enemy EnemyType;
            [Min(0)] public int MaxEnemiesInScene;
            [Min(0)] public int MaxEnemiesSpawned;

            [HideInInspector] public float SpawnTimer;
            [HideInInspector] public int EnemiesOnScene;
            [HideInInspector] public int SpawnsAmount;
            [HideInInspector] public int KillsAmount;

            public bool SpawnCompleted => SpawnsAmount >= MaxEnemiesSpawned;
            public bool KilledAllEnemies => KillsAmount >= MaxEnemiesSpawned;
            public bool IsMissingEnemies => (EnemiesOnScene < MaxEnemiesInScene) && !SpawnCompleted;

        }

        [System.Serializable]
        public class Horder
        {
            [SerializeField] private string name;

            [Space]
            public EnemySpawn[] EnemiesSpawn;
        }

        [System.Serializable]
        public class Level
        {
            [SerializeField] private string name;

            [Space]

            public Horder[] Horders;
        }

        [System.Serializable]
        public class Arena
        {
            public string EnemySpawnsTag;
            public Transform ArenaCenter;
        }

        [System.Serializable]
        public class Interval
        {
            public float HordeInterval;
            public float LevelInterval;
        }

        [SerializeField] protected bool DebugLog;

        [Space]

        public Arena ArenaCaracteristics;
        public Interval Intervals;
        public Level[] Levels;

        public UnityEvent OnStartGame;
        public UnityEvent<int> OnStartHorder;
        public UnityEvent<int> OnStartLevel;
        public UnityEvent<int> OnCompleteHorder;
        public UnityEvent<int> OnCompleteLevel;
        public UnityEvent OnGameWin;

        private int _currentHorderIndex;
        private int _currentLevelIndex;

        private bool _startHorderCalled;    // avoid to start horder multiple times
        private bool _startLevelCalled;     // avoid to start level multiple times

        public bool LevelStarted { get; private set; }
        public bool HorderStarted { get; private set; }

        public int CurrentLevelIndex { get => _currentLevelIndex; private set => _currentLevelIndex = Mathf.Clamp(value, 0, Levels.Length - 1); }
        public int CurrentHorderIndex { get => _currentHorderIndex; private set => _currentHorderIndex = Mathf.Clamp(value, 0, CurrentLevel.Horders.Length - 1); }

        public bool SpawnPaused { get; set; }
        public bool GameStarted { get; private set; }
        public bool GameWin { get; private set; }

        public bool CanSpawnEnemies => GameStarted &&
                                        GameManager.Instance.Player &&
                                        !GameManager.Instance.Player.IsDeath &&
                                        !GameWin &&
                                        !SpawnPaused &&
                                        LevelStarted &&
                                        HorderStarted;

        public Level CurrentLevel => Levels[CurrentLevelIndex];
        public Horder CurrentHorder => CurrentLevel.Horders[CurrentHorderIndex];

        /// <summary>
        /// Return true if current horder is lasted horder of the current level
        /// </summary>
        public bool IsLastedHorder => CurrentHorderIndex >= CurrentLevel.Horders.Length - 1;

        /// <summary>
        /// Return true if current level is lasted level of the levels list
        /// </summary>
        public bool IsLastedLevel => CurrentLevelIndex >= Levels.Length - 1;

        private static ArenaManager _arenaManager;

        public static ArenaManager Instance
        {
            get
            {
                if (!_arenaManager)
                {
                    _arenaManager = FindObjectOfType<ArenaManager>();

                    if (!_arenaManager)
                    {
                        Debug.LogWarning($"Does not exist any {nameof(ArenaManager)} on this scene");
                    }
                }

                return _arenaManager;
            }
        }

        protected virtual void Awake() { }

        protected virtual void Start() { }

        protected virtual void Update()
        {
            UpdateEnemiesSpawn();
            CheckGameProgression();
        }

        protected virtual void LateUpdate() { }

        /// <summary>
        /// Return all spawn point transforms active on scene 
        /// </summary>
        /// <returns></returns>
        protected GameObject[] FindAllEnemySpawnPoints()
        {
            return GameObject.FindGameObjectsWithTag(ArenaCaracteristics.EnemySpawnsTag);
        }

        /// <summary>
        /// General random position inside arena area.
        /// The random position needs to be out player view
        /// and inside NavMesh area
        /// </summary>
        /// <returns></returns>
        protected Vector3 GenerateSpawnPoint()
        {
            var _points = FindAllEnemySpawnPoints();

            if (_points.Length == 0)
            {
                Debug.LogError("No spawn point finded.");
                return Vector3.zero;
            }

            var _randomPoint = _points[Random.Range(0, _points.Length - 1)].transform.position;

            // check if is out player view
            while (CameraUtils.IsPointOnView(_randomPoint, Camera.main))
            {
                _randomPoint = _points[Random.Range(0, _points.Length)].transform.position;
            }

            return _randomPoint;
        }

        protected void SpawnEnemy(EnemySpawn enemySpawn)
        {
            var _player = GameManager.Instance.Player;
            var _spawnPosition = GenerateSpawnPoint();
            var _lookAtPlayer = Quaternion.LookRotation(_spawnPosition - _player.transform.position);
            var _enemyCharacter = Instantiate(enemySpawn.EnemyType.gameObject, _spawnPosition, _lookAtPlayer).GetComponent<PlayerCharacter>();

            enemySpawn.EnemiesOnScene++;
            enemySpawn.SpawnsAmount++;

            _enemyCharacter.OnDeath.AddListener(() => enemySpawn.EnemiesOnScene--);
            _enemyCharacter.OnDeath.AddListener(() => enemySpawn.KillsAmount++);
        }

        protected void CheckGameProgression()
        {
            if (!LevelStarted || !HorderStarted)
            {
                return;
            }

            var _horderCompleted = CheckHorderCompleted();
            var _levelCompleted = CheckLevelCompleted(CurrentHorderIndex);
            var _gameCompleted = CheckGameCompleted(CurrentLevelIndex);

            // horder progression
            if (!_horderCompleted) return;
            FinalizeCurrentHorder();

            if ((_gameCompleted || _levelCompleted) == false)
            {
                SetNextHorder();
                StartHorder();
            }

            // level progression
            if (!_levelCompleted) return;
            FinalizeCurrentLevel();

            if (!IsLastedLevel)
            {
                SetNextLevel();
            }

            // game progression
            if (!_gameCompleted) return;
            FinalizeGame();
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

        private bool CheckHorderCompleted()
        {
            // to complete the horder, the player has to kill all enemies on scene and
            // the spawn needs to be has marked as completed

            foreach (var enemySpawn in CurrentHorder.EnemiesSpawn)
            {
                if (!enemySpawn.KilledAllEnemies)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckLevelCompleted(int currentHorderIndex)
        {
            return currentHorderIndex >= CurrentLevel.Horders.Length - 1;
        }

        private bool CheckGameCompleted(int levelIndex)
        {
            return CurrentLevelIndex >= Levels.Length - 1;
        }

        private void FinalizeCurrentHorder()
        {
            if (!HorderStarted)
            {
                Debug.LogError("Current horder is already finalized");
                return;
            }

            HorderStarted = false;
            OnCompleteHorder.Invoke(CurrentHorderIndex);

            if (DebugLog) Debug.Log($"Horder finalized {CurrentHorderIndex}");
        }

        private void FinalizeCurrentLevel()
        {
            if (!LevelStarted)
            {
                Debug.LogError("Current level is already finalized");
                return;
            }

            LevelStarted = false;
            OnCompleteLevel.Invoke(CurrentLevelIndex);

            SetNextHorder();

            if (DebugLog) Debug.Log($"Level {CurrentLevelIndex} Completed");
        }

        private void FinalizeGame()
        {
            GameWin = true;
            OnGameWin.Invoke();

            if (DebugLog) Debug.Log("Game Completed");
        }

        private void StartHorder()
        {
            if (!LevelStarted)
            {
                Debug.LogError("Does not possible to start horder bacause the current level isn't started. Start level before.");
                return;
            }

            if (HorderStarted)
            {
                Debug.LogError("Does not possible to start horder bacause the current horder isn't finalized. Kill all enemies before.");
                return;
            }

            if (_startHorderCalled)
            {
                return;
            }

            _startHorderCalled = true;

            GameManager.Instance.Delay(Intervals.HordeInterval, () =>
            {
                _startHorderCalled = false;

                HorderStarted = true;
                OnStartHorder.Invoke(CurrentHorderIndex);

                if (DebugLog) Debug.Log($"New horde {CurrentHorderIndex} started on level {CurrentLevelIndex}/{Levels.Length - 1}");
            });
        }

        private void SetNextHorder()
        {
            if (HorderStarted)
            {
                Debug.LogError("Does not possible to set to next horder bacause the current horder isn't finalized. Kill all enemies before.");
                return;
            }

            if (CurrentHorderIndex < CurrentLevel.Horders.Length - 1)
            {
                CurrentHorderIndex++;
                if (DebugLog) Debug.Log($"Set next horder {CurrentHorderIndex}");
            }
        }

        /// <summary>
        /// Start new level after interval
        /// </summary>
        public void StartCurrentLevel()
        {
            if (LevelStarted)
            {
                Debug.LogError("Does not possible to start level bacause the level is already started.");
                return;
            }

            if (_startLevelCalled)
            {
                return;
            }

            _startLevelCalled = true;

            GameManager.Instance.Delay(Intervals.LevelInterval, () =>
            {
                _startLevelCalled = false;

                LevelStarted = true;
                OnStartLevel.Invoke(CurrentLevelIndex);
                StartHorder();

                if (DebugLog) Debug.Log($"Level started {CurrentHorderIndex}");
            });
        }

        /// <summary>
        /// Set to next level if this isn't lasted level
        /// </summary>
        public void SetNextLevel()
        {
            if (LevelStarted)
            {
                Debug.LogError("Does not possible to set next level bacause the current level is started. Finalize the level before");
                return;
            }

            if (_currentLevelIndex < Levels.Length - 1)
            {
                CurrentHorderIndex = 0;
                CurrentLevelIndex++;

                if (DebugLog) Debug.Log($"Set next level {CurrentHorderIndex}");
            }
        }

        /// <summary>
        /// Start first level and horder
        /// </summary>
        public void StartGame()
        {
            if (GameStarted)
            {
                Debug.LogError("The game's already started");
                return;
            }

            GameStarted = true;

            // starting first level
            StartCurrentLevel();
            OnStartGame.Invoke();

            if (DebugLog) Debug.Log("Game Started");
        }
    }
}



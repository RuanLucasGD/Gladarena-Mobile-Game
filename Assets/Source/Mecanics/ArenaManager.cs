using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class ArenaManager : MonoBehaviour
    {
        [System.Serializable]
        public class EnemySpawn
        {
            [SerializeField] private string Name;

            public Character EnemyType;
            [Min(0)] public int MaxEnemiesInScene;
            [Min(0)] public int MaxEnemiesSpawned;

            [HideInInspector] public float SpawnTimer;
            [HideInInspector] public int EnemiesOnScene;
            [HideInInspector] public int SpawnsAmount;

            public bool SpawnCompleted => SpawnsAmount >= MaxEnemiesSpawned;
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
            public Transform Center;
            public float ArenaSize;
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
        public UnityEvent OnStartHorder;
        public UnityEvent OnStartLevel;
        public UnityEvent OnCompleteHorder;
        public UnityEvent OnCompleteLevel;
        public UnityEvent OnCompleteGame;

        private int _currentHorderIndex;
        private int _currentLevelIndex;

        public bool CurrentHorderFinalized { get; private set; }
        public int CurrentLevelIndex { get => _currentLevelIndex; private set => _currentLevelIndex = Mathf.Clamp(value, 0, Levels.Length - 1); }
        public int CurrentHorderIndex { get => _currentHorderIndex; private set => _currentHorderIndex = Mathf.Clamp(value, 0, CurrentLevel.Horders.Length - 1); }
        public bool CanSpawnEnemies => Levels.Length != 0 && !GameWin && GameStarted && !IsOnInterval && GameManager.Instance.Player && !SpawnPaused && !CurrentHorderFinalized;
        public bool IsOnInterval { get; private set; }
        public bool SpawnPaused { get; set; }
        public bool GameStarted { get; private set; }
        public bool GameWin { get; private set; }
        public Level CurrentLevel => Levels[CurrentLevelIndex];
        public Horder CurrentHorder => CurrentLevel.Horders[CurrentHorderIndex];

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
                        Debug.LogError($"Does not exist any {nameof(ArenaManager)} on this scene");
                    }
                }

                return _arenaManager;
            }
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            //Debug.Log($"Level {CurrentLevelIndex + 1}/{Levels.Length}     Horder {CurrentHorderIndex + 1}/{CurrentLevel.Horders.Length}");
        }

        protected virtual void OnDrawGizmos()
        {
            if (!ArenaCaracteristics.Center)
            {
                return;
            }

            Gizmos.DrawWireSphere(ArenaCaracteristics.Center.position, ArenaCaracteristics.ArenaSize);
        }

        /// <summary>
        /// General random position inside arena area.
        /// The random position needs to be out player view
        /// and inside NavMesh area
        /// </summary>
        /// <returns></returns>
        protected Vector3 GenerateSpawnPoint()
        {
            Vector3 GenerateRawPosition()
            {
                var _randomX = Random.Range(-ArenaCaracteristics.ArenaSize, ArenaCaracteristics.ArenaSize);
                var _randomZ = Random.Range(-ArenaCaracteristics.ArenaSize, ArenaCaracteristics.ArenaSize);
                var _rawPosition = Vector3.ClampMagnitude(new Vector3(_randomX, 0, _randomZ), ArenaCaracteristics.ArenaSize) + ArenaCaracteristics.Center.position;
                return _rawPosition;
            }

            var _randomPoint = GenerateRawPosition();

            // check if is out player view
            while (IsSpawnPointOnView(_randomPoint))
            {
                _randomPoint = GenerateRawPosition();
            }

            return _randomPoint;
        }

        protected bool IsSpawnPointOnView(Vector3 point)
        {
            var _camera = Camera.main;
            var _direction = point - _camera.transform.position;

            // the point is on back of the camera
            if (Vector3.Dot(_direction.normalized, _camera.transform.forward) <= 0)
            {
                return false;
            }

            var _pointOnScreen = _camera.WorldToScreenPoint(point);

            return (_pointOnScreen.x > 0) &&
                    (_pointOnScreen.y > 0) &&
                    (_pointOnScreen.x < Screen.width) &&
                    (_pointOnScreen.y < Screen.height);
        }

        protected void CheckGameProgression()
        {
            if (CurrentHorderFinalized)
            {
                return;
            }

            var _horderCompleted = CheckHorderCompleted();
            var _levelCompleted = CheckLevelCompleted(CurrentHorderIndex);
            var _gameCompleted = CheckGameCompleted(CurrentLevelIndex);

            if (!_horderCompleted) return;
            FinalizeCurrentHorder();
            StartNextHorderIntervaled();

            if (!_levelCompleted) return;
            FinalizeCurrentLevel();

            if (!_gameCompleted) return;
            FinalizeGame();
        }

        private bool CheckHorderCompleted()
        {
            // to complete the horder, the player has to kill all enemies on scene and
            // the spawn needs to be has marked as completed

            foreach (var enemySpawn in CurrentHorder.EnemiesSpawn)
            {
                var _hasEnemiesOnScene = enemySpawn.EnemiesOnScene > 0;
                var _spawnedAllEnemies = enemySpawn.SpawnCompleted;

                if (!_hasEnemiesOnScene && _spawnedAllEnemies)
                {
                    return true;
                }
            }

            return false;
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
            IsOnInterval = true;
            CurrentHorderFinalized = true;
            OnCompleteHorder.Invoke();

            if (DebugLog) Debug.Log($"Horder finalized {CurrentHorderIndex}");
        }

        private void FinalizeCurrentLevel()
        {
            OnCompleteLevel.Invoke();

            if (DebugLog) Debug.Log($"Level {CurrentLevelIndex} Completed");
        }

        private void FinalizeGame()
        {
            GameWin = true;
            OnCompleteGame.Invoke();

            if (DebugLog) Debug.Log("Game Completed");
        }

        private void StartNextHorderImmediatly()
        {
            IsOnInterval = false;
            CurrentHorderFinalized = false;
            CurrentHorderIndex++;
            OnStartHorder.Invoke();

            if (DebugLog) Debug.Log($"New horde {CurrentHorderIndex} started on level {CurrentLevelIndex}");
        }

        private void StartNextLevelImmediatly()
        {
            var _nextLevelIsTheLast = ++_currentLevelIndex < Levels.Length - 1;

            if (!_nextLevelIsTheLast)
            {
                CurrentHorderIndex = 0;
                CurrentLevelIndex++;
            }

            OnStartLevel.Invoke();
        }

        /// <summary>
        /// Start new enemies horder after interval
        /// </summary>
        public void StartNextHorderIntervaled()
        {
            if (!CurrentHorderFinalized)
            {
                Debug.LogError("Does not possible to start next horder bacause the current horder isn't finalized. Kill all enemies before.");
                return;
            }

            GameManager.Instance.Delay(Intervals.HordeInterval, StartNextHorderImmediatly);
        }

        /// <summary>
        /// Start new level after interval
        /// </summary>
        public void StartNextLevelIntervaled()
        {
            if (!CurrentHorderFinalized)
            {
                Debug.LogError("Does not possible to start next level bacause the current horder isn't finalized. Kill all enemies before.");
                return;
            }

            GameManager.Instance.Delay(Intervals.LevelInterval, StartNextLevelImmediatly);
        }

        public void StartGame()
        {
            if (GameStarted)
            {
                Debug.LogError("The game's already started");
                return;
            }

            GameStarted = true;
            OnStartGame.Invoke();
            OnStartLevel.Invoke();
            OnStartHorder.Invoke();

            if (DebugLog) Debug.Log("Game Started");
        }
    }
}



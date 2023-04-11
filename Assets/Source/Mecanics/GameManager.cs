using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game.Mecanics
{
    /// <summary>
    /// Main game manager.
    /// The game scene can have only one instance of game manager.
    /// Responsible for manage the game match.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public Camera GameplayCamera;

        [Space]

        public float RestartGameDelay;

        [Space]

        public bool DisablePlayerOnStart;

        [Space]
        public UnityEvent<bool> OnSetPausedGame;

        private bool _gamePaused;
        private PlayerCharacter _player;
        private static GameManager _gameManager;

        public bool GamePaused
        {
            get => _gamePaused; set
            {
                if (value != _gamePaused)
                {
                    // disable characters control when pause game
                    foreach (var c in FindObjectsOfType<Character>(true))
                    {
                        c.enabled = !value;
                    }

                    Time.timeScale = value ? 0 : 1;
                    _gamePaused = value;
                    OnSetPausedGame.Invoke(value);
                }
            }
        }

        public PlayerCharacter Player
        {
            get
            {
                if (!_player)
                {
                    _player = FindObjectOfType<PlayerCharacter>();
                    if (_player)
                    {
                        SetPlayerForwardDirection();
                    }

                }

                return _player;
            }
        }

        public static GameManager Instance
        {
            get
            {
                if (!_gameManager)
                {
                    _gameManager = FindObjectOfType<GameManager>();

                    if (!_gameManager)
                    {
                        _gameManager = new GameObject("Game Manager").AddComponent<GameManager>();
                    }
                }

                return _gameManager;
            }
        }

        public GameManager()
        {
            RestartGameDelay = 2;
            DisablePlayerOnStart = false;
        }

        private void Awake()
        {
            var _anotherManager = FindObjectOfType<GameManager>();

            if (_anotherManager != this)
            {
                Destroy(this.gameObject);
                return;
            }

            if (!Player)
            {
                Debug.LogError("Player not finded on scene");
                return;
            }

            if (ArenaManager.Instance)
            {
                ArenaManager.Instance.OnStartLevel.AddListener(l => SetEnablePlayerControl(true));
                ArenaManager.Instance.OnCompleteLevel.AddListener(l => Player.MovePlayerToCenter());
            }
        }

        void Start()
        {
            if (!GameplayCamera)
            {
                GameplayCamera = Camera.main;
            }

            SetEnablePlayerControl(!DisablePlayerOnStart);

            SetPlayerForwardDirection();
        }

        private void SetPlayerForwardDirection()
        {
            if (!Player)
            {
                return;
            }

            if (!GameplayCamera)
            {
                Debug.LogError("Camera not assigned on Game Manager!");
                return;
            }

            // set player forward direction to move to camera forward
            Player.Forward = Vector3.ProjectOnPlane(GameplayCamera.transform.forward, Vector3.up);
        }

        public void SetEnablePlayerControl(bool enabled)
        {
            Player.CanMove = enabled;
        }

        public void RestartGameImmediatly()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void RestartGameDeleyed()
        {
            Delay(RestartGameDelay, RestartGameImmediatly);
        }

        public void ResetPlayerLife()
        {
            if (!Player)
            {
                return;
            }

            Player.ResetLife();
        }

        public void Delay(float delay, UnityAction onCompleted)
        {
            IEnumerator _Delay()
            {
                yield return new WaitForSeconds(delay);
                onCompleted();
            }

            StartCoroutine(_Delay());
        }
    }
}



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
        public bool DisablePlayerOnStart;

        [Space]
        public UnityEvent<bool> OnSetPausedGame;
        public UnityEvent OnGameOver;

        [Space]
        public string PlayerTag;

        private bool _gamePaused;
        private PlayerCharacter _player;
        private static GameManager _gameManager;

        public bool GamePaused
        {
            get => _gamePaused;
            set
            {
                // lag after pause/continue game... disable this. why? idk
                Player.enabled = !value;

                // disable characters control when pause game
                foreach (var c in FindObjectsOfType<Enemy>(true))
                {
                    c.enabled = !value;
                }

                Time.timeScale = value ? 0 : 1;
                _gamePaused = value;
                OnSetPausedGame.Invoke(value);
            }
        }

        public PlayerCharacter Player
        {
            get
            {
                if (!_player)
                {
                    _player = GameObject.FindWithTag(PlayerTag).GetComponent<PlayerCharacter>();
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

            Player.OnDeath.AddListener(() => OnGameOver.Invoke());
        }

        void Start()
        {
            SetEnablePlayerControl(!DisablePlayerOnStart);
        }

        public void SetEnablePlayerControl(bool enabled)
        {
            Player.CanMove = enabled;
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



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

        private static GameManager _gameManager;


        public PlayerCharacter Player { get; private set; }

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

        private void Awake()
        {
            var _anotherManager = FindObjectOfType<GameManager>();

            if (_anotherManager != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Player = FindObjectOfType<PlayerCharacter>();

            if (!Player)
            {
                Debug.LogError("Player not finded on scene");
                return;
            }
        }

        void Start()
        {
            SetPlayerForwardDirection();

            Player.OnDeath.AddListener(RestartGameDeleyed);
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

        public void RestartGameImmediatly()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void RestartGameDeleyed()
        {
            StartCoroutine(Delay(RestartGameDelay, RestartGameImmediatly));
        }

        private IEnumerator Delay(float delay, UnityAction onCompleted)
        {
            yield return new WaitForSeconds(delay);
            onCompleted();
        }
    }
}



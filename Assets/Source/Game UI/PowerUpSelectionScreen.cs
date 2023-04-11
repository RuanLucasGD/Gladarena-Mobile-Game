using UnityEngine;
using UnityEngine.Events;
using Game.Mecanics;

namespace Game.UI
{
    public class PowerUpSelectionScreen : MonoBehaviour
    {
        public UnityEvent OnSelectPowerUp;

        void Awake()
        {
            HideScreen();

            if (ArenaManager.Instance)
            {
                ArenaManager.Instance.OnStartLevel.AddListener(HideOnStartLevel);
                ArenaManager.Instance.OnCompleteLevel.AddListener(ShowOnLevelCompleted);
            }

            OnSelectPowerUp.AddListener(StartLevelWhenSelecPowerUp);
        }

        private void ShowScreen()
        {
            if (ArenaManager.Instance.GameWin)
            {
                return;
            }

            gameObject.SetActive(true);
        }

        private void HideScreen()
        {
            gameObject.SetActive(false);
        }

        private void ShowOnLevelCompleted(int level)
        {
            GameManager.Instance.Player.OnGetCenter.AddListener(ShowScreen);
            GameManager.Instance.Player.OnGetCenter.AddListener(CleanListeners);
        }

        private void HideOnStartLevel(int level)
        {
            HideScreen();
        }

        private void CleanListeners()
        {
            GameManager.Instance.Player.OnGetCenter.RemoveListener(CleanListeners);
            GameManager.Instance.Player.OnGetCenter.RemoveListener(ShowScreen);
        }

        public void SelectPowerUp(PowerUp powerUp)
        {
            var _powerUp = Instantiate(powerUp.gameObject, Vector3.zero, Quaternion.identity).GetComponent<PowerUp>();

            GameManager.Instance.Player.AddPowerUp(_powerUp);

            HideScreen();
            OnSelectPowerUp.Invoke();
        }

        private void StartLevelWhenSelecPowerUp()
        {
            if (!ArenaManager.Instance)
            {
                return;
            }

            if (!ArenaManager.Instance.GameStarted)
            {
                ArenaManager.Instance.StartGame();
            }
            else
            {
                ArenaManager.Instance.StartCurrentLevel();
            }
        }
    }
}
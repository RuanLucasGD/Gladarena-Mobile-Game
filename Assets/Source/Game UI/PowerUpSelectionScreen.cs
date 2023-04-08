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
            HiddenPowerUpScreen();

            if (ArenaManager.Instance)
            {
                ArenaManager.Instance.OnStartLevel.AddListener(l => HiddenPowerUpScreen());
                ArenaManager.Instance.OnCompleteLevel.AddListener(l => ShowPowerUpScreen());
            }

            OnSelectPowerUp.AddListener(StartLevelWhenSelecPowerUp);
        }

        private void ShowPowerUpScreen()
        {
            if (ArenaManager.Instance.GameWin)
            {
                return;
            }

            gameObject.SetActive(true);
        }

        private void HiddenPowerUpScreen()
        {
            gameObject.SetActive(false);
        }

        public void SelectPowerUp(PowerUp powerUp)
        {
            var _powerUp = Instantiate(powerUp.gameObject, Vector3.zero, Quaternion.identity).GetComponent<PowerUp>();

            GameManager.Instance.Player.AddPowerUp(_powerUp);

            HiddenPowerUpScreen();
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
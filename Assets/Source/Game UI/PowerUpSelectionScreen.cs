using UnityEngine;
using Game.Mecanics;

namespace Game.UI
{
    public class PowerUpSelectionScreen : MonoBehaviour
    {
        void Awake()
        {
            HiddenPowerUpScreen();

            ArenaManager.Instance.OnStartLevel.AddListener(l => HiddenPowerUpScreen());
            ArenaManager.Instance.OnCompleteLevel.AddListener(l => ShowPowerUpScreen());
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

        public void SelectPowerUp()
        {
            ArenaManager.Instance.StartCurrentLevel();
            HiddenPowerUpScreen();
        }
    }
}
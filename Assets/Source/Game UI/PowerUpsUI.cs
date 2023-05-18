using Game.Mecanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PowerUpsUI : MonoBehaviour
    {
        public PowerUpManager powerUpManager;
        public GameObject PowerUpsScreen;
        [Space]
        public PowerUpButtonWidget[] PowerUpsButtons;

        void Start()
        {
            HidePowerUpsUI();

            foreach (var p in PowerUpsButtons)
            {
                p.Button.onClick.AddListener(HidePowerUpsUI);
            }
        }

        public void ShowPowerUpsUI()
        {
            SetupButtons();

            PowerUpsScreen?.SetActive(true);
            GameManager.Instance.GamePaused = true;
        }

        public void HidePowerUpsUI()
        {
            PowerUpsScreen?.SetActive(false);

            if (GameManager.Instance.GamePaused)
            {
                GameManager.Instance.GamePaused = false;
            }
        }

        private void SetupButtons()
        {
            var _powerUps = powerUpManager.GeneratePowerUpsOptionsList(PowerUpsButtons.Length);

            for (int i = 0; i < _powerUps.Length; i++)
            {
                var _p = _powerUps[i];
                var _b = PowerUpsButtons[i];

                if (_p != null && _b != null)
                {
                   
                    _b.Setup(_p);
                    _b.Button.interactable = !_p.IsFullUpgrade();
                }
            }
        }
    }
}
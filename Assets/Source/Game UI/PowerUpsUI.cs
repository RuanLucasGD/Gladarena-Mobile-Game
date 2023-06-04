using Game.Mecanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PowerUpsUI : MonoBehaviour
    {
        public PowerUpManager PowerUpManager;
        public GameObject PowerUpsScreen;

        [Space]
        public Transform PowerUpsUiWidgetParent;
        public PowerUpUiWidget PowerUpUiWidgetPrefab;

        [Space]
        public int PowerUpsToSelect;

        private List<PowerUpUiWidget> _powerUpsButtons;

        void Start()
        {
            _powerUpsButtons = new List<PowerUpUiWidget>();
            HidePowerUpsUI();
        }

        public void ShowPowerUpsUiIfHasUpgrades()
        {
            if (PowerUpManager.CurrentPowerUps.Count == 0)
            {
                ShowPowerUpsUI();
                return;
            }

            foreach (var p in PowerUpManager.AllPowerUps)
            {
                if (!p.IsFullUpgrade())
                {
                    ShowPowerUpsUI();
                    return;
                }
            }
        }

        public void ShowPowerUpsUI()
        {
            SetupButtons(); 
            if (_powerUpsButtons.Count == 0)
            {
                HidePowerUpsUI();
                return;
            }

            PowerUpsScreen?.SetActive(true);
            GameManager.Instance.GamePaused = true;
        }

        public void HidePowerUpsUI()
        {
            PowerUpsScreen?.SetActive(false);
            GameManager.Instance.GamePaused = false;

            for (int i = 0; i < _powerUpsButtons.Count; i++)
            {
                Destroy(_powerUpsButtons[i].gameObject);
            }

            _powerUpsButtons.Clear();
        }

        private void SetupButtons()
        {
            // get random powerups to show on ui
            var _powerUps = PowerUpManager.GenerateRandomPowerUpsList(PowerUpsToSelect);

            // generate powerups buttons to select on ui
            for (int i = 0; i < _powerUps.Length; i++)
            {
                var button = Instantiate(PowerUpUiWidgetPrefab, PowerUpsUiWidgetParent);
                button.Button.onClick.AddListener(HidePowerUpsUI);
                _powerUpsButtons.Add(button);
            }

            for (int i = 0; i < _powerUps.Length; i++)
            {
                var _p = _powerUps[i];
                var _b = _powerUpsButtons[i];

                if (_p != null && _b != null)
                {
                   
                    _b.Setup(_p, PowerUpManager);
                    _b.Button.interactable = !_p.IsFullUpgrade();
                }
            }
        }
    }
}
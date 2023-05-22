using Game.Mecanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PowerUpUiWidget : MonoBehaviour
{
    public PowerUp PowerUp;

    [Space]
    public Text Name;
    public Button Button;
    public Text UpgradeInfo;
    public Text Level;

    private PowerUpManager _powerUpManager;

    private void OnDisable()
    {
    }

    public void Setup(PowerUp powerUp, PowerUpManager manager)
    {
        if (!powerUp)
        {
            return;
        }

        PowerUp = powerUp;
        Name.text = powerUp.PowerUpName;
        UpgradeInfo.text = powerUp.UpgradeInfo();
        Level.text = $"Level {powerUp.CurrentLevelIndex + 1}";
        _powerUpManager = manager;

        Button.onClick.AddListener(ButtonSetPowerUpAction);
    }

    private void ButtonSetPowerUpAction()
    {
        _powerUpManager.AddNewPowerUp(PowerUp);
        Button.onClick.RemoveListener(ButtonSetPowerUpAction);
    }
}

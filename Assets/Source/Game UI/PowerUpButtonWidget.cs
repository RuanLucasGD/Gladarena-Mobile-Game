using Game.Mecanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpButtonWidget : MonoBehaviour
{
    public PowerUp PowerUp;

    [Space]
    public Text Name;
    public Button Button;
    public Text UpgradeInfo;

    private void OnEnable()
    {
        Setup(PowerUp);

        Button.onClick.AddListener(UsePowerUp);
    }

    public void Setup(PowerUp powerUp)
    {
        if (!powerUp)
        {
            return;
        }

        PowerUp = powerUp;
        Name.text = powerUp.PowerUpName;
        UpgradeInfo.text = powerUp.UpgradeInfo();
    }

    private void UsePowerUp()
    {
        if (PowerUp == null)
        {
            return;
        }

        PowerUp.Use();
    }
}

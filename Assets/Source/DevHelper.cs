using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Mecanics;

#if UNITY_EDITOR

namespace Game.Experimental
{
    public class DevHelper : MonoBehaviour
    {
        public GameObject PlayerPrefab;

        [Header("Power Ups")]
        public RestoreLifePowerUp LifePowerUp;

        private bool isShowing;
        private int currentTab;

        private GUIStyle titleLabel;
        private GUIStyle normalLabel;

        private PlayerCharacter Player => FindObjectOfType<PlayerCharacter>();



        // Start is called before the first frame update
        void Start()
        {
            currentTab = 1;

            titleLabel = new GUIStyle();
            titleLabel.fontStyle = FontStyle.Bold;
            titleLabel.fontSize = 40;
            titleLabel.normal.textColor = Color.white;

            normalLabel = new GUIStyle();
            normalLabel.fontStyle = FontStyle.Bold;
            normalLabel.fontSize = 20;
            normalLabel.normal.textColor = Color.white;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                isShowing = !isShowing;
            }

            if (isShowing)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) currentTab = 1;
                if (Input.GetKeyDown(KeyCode.Alpha2)) currentTab = 2;
                if (Input.GetKeyDown(KeyCode.Alpha3)) currentTab = 3;
            }
        }

        void OnGUI()
        {
            if (!isShowing)
            {
                return;
            }

            ShowGui();
        }

        void ShowGui()
        {
            var _currentGuiPosY = 0;

            _currentGuiPosY += 30;
            GUI.Label(new Rect(30, _currentGuiPosY, 50, 50), "Change current GUI table using 1, 2, 3...", normalLabel);

            if (currentTab == 1) PlayerTab(ref _currentGuiPosY);
            if (currentTab == 2) PlayerPowerUps(ref _currentGuiPosY);
            if (currentTab == 3) ArenaProgression(ref _currentGuiPosY);
        }

        void AddDamageOnPlayer()
        {
            if (Player)
            {
                Player.AddDamage(50);
            }
        }

        void KillPlayer()
        {
            if (Player)
            {
                Player.KillCharacter();
            }
        }

        void CreatePlayer()
        {
            if (!Player)
            {
                Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
            }
        }

        void RestorePlayerLife()
        {
            if (Player)
            {
                Player.ResetLife();
            }
        }

        void SetPlayerPowerUp(PowerUp powerUp)
        {
            if (!Player)
            {
                return;
            }

            if (powerUp)
            {
                var _powerUp = Instantiate(powerUp.gameObject, Vector3.zero, Quaternion.identity).GetComponent<PowerUp>();
                Player.SetPowerUp(_powerUp);
            }
            else
            {
                Player.SetPowerUp(null, true);
            }
        }

        void PlayerTab(ref int currentGuiPosY)
        {
            currentGuiPosY += 50;
            GUI.Label(new Rect(30, currentGuiPosY, 50, 50), "Player Informations", normalLabel);

            if (Player && !Player.IsDeath)
            {
                currentGuiPosY += 30;
                GUI.Label(new Rect(30, currentGuiPosY, 50, 50), $"Player Life {(int)Player.CurrentLife}", normalLabel);

                currentGuiPosY += 30;
                if (GUI.Button(new Rect(30, currentGuiPosY, 150, 30), "Kill Play"))
                {
                    KillPlayer();
                }

                currentGuiPosY += 30;
                if (GUI.Button(new Rect(30, currentGuiPosY, 150, 30), "Add Damage on Player"))
                {
                    AddDamageOnPlayer();
                }

                currentGuiPosY += 30;
                if (GUI.Button(new Rect(30, currentGuiPosY, 150, 30), "Restore Player Life"))
                {
                    RestorePlayerLife();
                }
            }
            else
            {
                currentGuiPosY += 30;
                if (GUI.Button(new Rect(30, currentGuiPosY, 150, 30), "Create Player"))
                {
                    CreatePlayer();
                }
            }
        }

        void PlayerPowerUps(ref int currentGuiPosY)
        {
            if (!Player)
            {
                currentGuiPosY += 50;
                GUI.Label(new Rect(30, currentGuiPosY, 50, 50), "Add player on scene to add power up...", normalLabel);
                return;
            }

            currentGuiPosY += 50;
            GUI.Label(new Rect(30, currentGuiPosY, 50, 50), "Player Power Ups", normalLabel);

            currentGuiPosY += 30;
            GUI.Label(new Rect(30, currentGuiPosY, 50, 50), $"Player Life {(int)Player.CurrentLife}", normalLabel);

            currentGuiPosY += 30;
            var _powerUpName = Player.GetPowerUp() ? Player.GetPowerUp().name : "none";
            GUI.Label(new Rect(30, currentGuiPosY, 50, 50), $"Player Power Up: {_powerUpName}", normalLabel);

            if (Player.GetPowerUp())
            {
                currentGuiPosY += 30;
                if (GUI.Button(new Rect(30, currentGuiPosY, 150, 30), "Use Power Up"))
                {
                    Player.UsePowerUp();
                }

                currentGuiPosY += 30;
                if (GUI.Button(new Rect(30, currentGuiPosY, 150, 30), "Remove Power Up"))
                {
                    SetPlayerPowerUp(null);
                }
            }
            else
            {
                currentGuiPosY += 30;
                if (GUI.Button(new Rect(30, currentGuiPosY, 150, 30), "Set Life Power Up"))
                {
                    SetPlayerPowerUp(LifePowerUp);
                }
            }
        }

        void ArenaProgression(ref int currentGuiPosY)
        {
            if (!ArenaManager.Instance)
            {
                currentGuiPosY += 50;
                GUI.Label(new Rect(30, currentGuiPosY, 50, 50), $"None {nameof(ArenaManager)} finded on this scene", normalLabel);
                return;
            }

            currentGuiPosY += 30;
            GUI.Label(new Rect(30, currentGuiPosY, 50, 50), $"Current level {ArenaManager.Instance.CurrentLevelIndex + 1}", normalLabel);

            currentGuiPosY += 30;
            GUI.Label(new Rect(30, currentGuiPosY, 50, 50), $"Current horder {ArenaManager.Instance.CurrentHorderIndex + 1}", normalLabel);

            currentGuiPosY += 30;
            GUI.Label(new Rect(30, currentGuiPosY, 50, 50), $"Can Spawn Enemies {ArenaManager.Instance.CanSpawnEnemies}", normalLabel);
        }
    }
}
#endif
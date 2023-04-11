using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Game.Mecanics;

namespace Game.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        public GameObject MainScreen;
        public GameObject PowerUpsScreen;
        public GameObject StoreScreen;

        [Header("Events")]
        public UnityEvent OnStartGame;
        public UnityEvent OnHideMainMenu;
        public UnityEvent OnShowMenu;
        public UnityEvent OnShowMainScreen;
        public UnityEvent OnShowPowerUpScreen;
        public UnityEvent OnShowStoreScreen;

        public UnityEvent OnHidePowerUpScreen;
        public UnityEvent OnHideStoreScreen;

        private static MainMenuManager _instance;

        public static MainMenuManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<MainMenuManager>();
                }

                return _instance;
            }
        }

        private void Start()
        {
            ShowMenu();
        }

        public void StartGame()
        {
            HideMenu();
        }

        public void ShowMainScreen()
        {
            OnShowMainScreen.Invoke();
        }

        public void ShowStoreScreen()
        {
            OnShowStoreScreen.Invoke();
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void HideMenu()
        {
            MainScreen.SetActive(false);
            PowerUpsScreen.SetActive(false);
            StoreScreen.SetActive(false);

            OnHideMainMenu.Invoke();
        }

        public void ShowMenu()
        {
            MainScreen.SetActive(true);
            PowerUpsScreen.SetActive(true);
            StoreScreen.SetActive(true);

            OnShowMenu.Invoke();
        }
    }
}


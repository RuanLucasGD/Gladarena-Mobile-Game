using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using Game.Mecanics;
using Game.Monetization;

namespace Game.UI
{
    public class GameOverScreen : MonoBehaviour
    {
        public Button RestartButton;
        public Button ReviveWithADS;

        private bool _adsShowed;

        void Start()
        {
            GameManager.Instance.Player.OnDeath.AddListener(EnableScreen);


            if (MonetizationManager.Instance)
            {
                ReviveWithADS.onClick.AddListener(RevivePlayerWithADS);

                if (!MonetizationManager.Instance.IsInitialized)
                {
                    MonetizationManager.Instance.OnInitialized.AddListener(LoadAdsVideo);
                }
            }
            else
            {
                Debug.Log($"No {nameof(MonetizationManager)} finded on this scene. Ads video not showed");
            }

            DisableScreen();
        }

        private void RevivePlayerWithADS()
        {
            MonetizationManager.Instance.ShowRewardedAds(onShow: OnUserViewAds);

            DisableScreen();
        }

        private void DisableScreen()
        {
            gameObject.SetActive(false);
        }

        private void LoadAdsVideo()
        {
            MonetizationManager.Instance.LoadRewardedAds();
        }

        private void EnableScreen()
        {
            gameObject.SetActive(true);

            // check if you have any ADS video to show and check if the user has seen the ads.Ads video can be played only once
            ReviveWithADS.interactable = MonetizationManager.Instance.HasAdsLoaded && !_adsShowed;
        }

        private void OnUserViewAds(UnityAdsShowCompletionState viewState)
        {
            if (viewState == UnityAdsShowCompletionState.COMPLETED)
            {
                RevivePlayer();
                DisableScreen();

                _adsShowed = true;
            }
        }

        private void RevivePlayer()
        {
            if (!GameManager.Instance.Player.IsGrounded)
            {
                MovePlayerToCenter();
            }
            
            GameManager.Instance.Player.ResetLife();
        }

        private void MovePlayerToCenter()
        {
            GameManager.Instance.Player.CharacterController.enabled = false;
            GameManager.Instance.Player.transform.position = Vector3.zero;
            GameManager.Instance.Player.CharacterController.enabled = true;
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Experimental;
using UnityEngine.Advertisements;

public class AdsUiTest : MonoBehaviour
{
    public Button showInterstitialButton;
    public Button showRewardedButton;
    public Button loadInterstitialButton;
    public Button loadRewardedButton;
    public Text coinsText;

    private int coins;

    void Start()
    {
        showInterstitialButton.onClick.AddListener(ShowInterstitialAds);
        showRewardedButton.onClick.AddListener(ShowRewardedAds);

        loadInterstitialButton.onClick.AddListener(LoadInterstitialAds);
        loadRewardedButton.onClick.AddListener(LoadRewardedAds);
    }

    void Update()
    {
        loadInterstitialButton.interactable = MonetizationManager.Instance.CanLoadAds;
        loadRewardedButton.interactable = MonetizationManager.Instance.CanLoadAds;

        showInterstitialButton.interactable = MonetizationManager.Instance.CanShowAds;
        showRewardedButton.interactable = MonetizationManager.Instance.CanShowAds;

        coinsText.text = coins.ToString();

        //Debug.Log(MonetizationManager.Instance.HasAdsLoaded);
    }

    void LoadInterstitialAds()
    {
        MonetizationManager.Instance.LoadInterstitialAds();
    }

    void LoadRewardedAds()
    {
        MonetizationManager.Instance.LoadRewardedAds();
    }

    void ShowInterstitialAds()
    {
        MonetizationManager.Instance.ShowInterstitialAds();
    }

    void ShowRewardedAds()
    {
        MonetizationManager.Instance.ShowRewardedAds(onShow: AddCoin);
    }

    public void AddCoin(UnityAdsShowCompletionState completionState)
    {
        if (completionState == UnityAdsShowCompletionState.COMPLETED)
        {
            coins += 10;
        }
    }
}

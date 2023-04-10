using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Advertisements;

namespace Game.Monetization
{
    /// <summary>
    /// Show ads 
    /// </summary>
    public class MonetizationManager : MonoBehaviour
    {
        /// <summary>
        /// Implement internal callbacks of Unity Ads
        /// </summary>
        private class AdsListener : IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
        {
            public UnityEvent OnAdsInitialized;
            public UnityEvent OnAdsLoaded;
            public UnityEvent OnAdsStarted;
            public UnityEvent<UnityAdsShowCompletionState> OnAdsShowed;

            public UnityEvent<UnityAdsInitializationError, string> OnAdsInitializationFail;
            public UnityEvent<UnityAdsLoadError, string> OnAdsLoadFail;
            public UnityEvent<UnityAdsShowError, string> OnAdsShowFail;

            public AdsListener()
            {
                OnAdsInitialized = new UnityEvent();
                OnAdsLoaded = new UnityEvent();
                OnAdsStarted = new UnityEvent();
                OnAdsShowed = new UnityEvent<UnityAdsShowCompletionState>();

                OnAdsInitializationFail = new UnityEvent<UnityAdsInitializationError, string>();
                OnAdsLoadFail = new UnityEvent<UnityAdsLoadError, string>();
                OnAdsShowFail = new UnityEvent<UnityAdsShowError, string>();
            }

            public void OnInitializationComplete()
            {
                OnAdsInitialized.Invoke();
                Debug.Log("ADS initialized");
            }

            public void OnInitializationFailed(UnityAdsInitializationError error, string message)
            {
                OnAdsInitializationFail.Invoke(error, message);
                Debug.Log($"ADS initialization failed: {message}");
            }

            public void OnUnityAdsAdLoaded(string adUnitId)
            {
                OnAdsLoaded.Invoke();
                Debug.Log("ADS loaded");
            }

            public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
            {
                OnAdsLoadFail.Invoke(error, message);
                Debug.Log($"ADS not loaded: {message}");
            }

            public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
            {
                OnAdsShowed.Invoke(showCompletionState);
                Debug.Log($"ADS showed: {showCompletionState}");
            }

            public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
            {
                OnAdsShowFail.Invoke(error, message);
                Debug.Log($"ADS not showed: {message}");
            }

            public void OnUnityAdsShowStart(string adUnitId)
            {
                OnAdsStarted.Invoke();
                Debug.Log($"ADS started");
            }

            public void OnUnityAdsShowClick(string adUnitId) { }
        }

        // see on unity dashboard/monetization/app
        public const string ANDROID_ADS_ID = "5213116";
        public const string INTERSTITIAL = "Interstitial_Android";
        public const string REWARDED = "Reward_Android";

        private AdsListener _adsListener;
        private static MonetizationManager _monetizationManager;

        public UnityEvent OnInitialized { get; set; }

        public bool HasAdsLoaded { get; set; }
        public bool IsLoadingAds { get; private set; }

        public bool IsInitialized => Advertisement.isInitialized;
        public bool IsSupported => Advertisement.isSupported;
        public bool IsShowingAds => Advertisement.isShowing;
        public bool CanLoadAds => IsInitialized && IsSupported && !HasAdsLoaded;
        public bool CanShowAds => IsInitialized && IsSupported && !IsShowingAds && HasAdsLoaded;

        public static MonetizationManager Instance
        {
            get
            {
                if (!_monetizationManager)
                {
                    _monetizationManager = FindObjectOfType<MonetizationManager>();

                    if (!_monetizationManager)
                    {
                        _monetizationManager = new GameObject("Monetization Manager").AddComponent<MonetizationManager>();
                    }
                }

                return _monetizationManager;
            }
        }

        private void Awake()
        {
            if (this != MonetizationManager.Instance)
            {
                Destroy(this);
            }

            _adsListener = new AdsListener();
            OnInitialized = new UnityEvent();

            DontDestroyOnLoad(this);
            InitializeAds(() => OnInitialized.Invoke());
        }

        private void InitializeAds(UnityAction onInitialize = null, UnityAction<UnityAdsInitializationError, string> onInitializeFail = null)
        {
#if UNITY_ANDROID
            if (onInitialize != null) _adsListener.OnAdsInitialized.AddListener(onInitialize);
            if (onInitializeFail != null) _adsListener.OnAdsInitializationFail.AddListener(onInitializeFail);

            // see on unity dashboard/monetization/app
            Advertisement.Initialize(ANDROID_ADS_ID, Debug.isDebugBuild, _adsListener);
#endif
        }

        private void LoadAds(string placementId, UnityAction onLoad, UnityAction<UnityAdsLoadError, string> onLoadFail)
        {
            if (!CanLoadAds)
            {
                if (!IsInitialized) Debug.LogError("Monetization Manager not initialized");
                return;
            }
            _adsListener.OnAdsLoaded.RemoveAllListeners();
            _adsListener.OnAdsLoadFail.RemoveAllListeners();

            if (onLoad != null) _adsListener.OnAdsLoaded.AddListener(onLoad);
            if (onLoadFail != null) _adsListener.OnAdsLoadFail.AddListener(onLoadFail);

            _adsListener.OnAdsLoaded.AddListener(OnAdsLoaded);
            _adsListener.OnAdsLoadFail.AddListener(OnAdsLoadFail);

            Advertisement.Load(placementId, _adsListener);
        }

        private void ShowAds(string placementId, UnityAction onStart, UnityAction<UnityAdsShowCompletionState> onShow, UnityAction<UnityAdsShowError, string> onShowFail)
        {
            if (!CanShowAds)
            {
                if (!IsInitialized) Debug.Log("ADS not initialized");
                Debug.Log("Is not possible to show ads");
                return;
            }

            if (!HasAdsLoaded)
            {
                Debug.Log("No ads loaded");
                return;
            }

            _adsListener.OnAdsStarted.RemoveAllListeners();
            _adsListener.OnAdsShowed.RemoveAllListeners();
            _adsListener.OnAdsShowFail.RemoveAllListeners();

            if (onStart != null) _adsListener.OnAdsStarted.AddListener(onStart);
            if (onShow != null) _adsListener.OnAdsShowed.AddListener(onShow);
            if (onShowFail != null) _adsListener.OnAdsShowFail.AddListener(onShowFail);

            _adsListener.OnAdsStarted.AddListener(OnAdsStart);
            _adsListener.OnAdsShowed.AddListener(OnAdsShowed);
            _adsListener.OnAdsShowFail.AddListener(OnAdsShowFail);

            Advertisement.Show(placementId, _adsListener);
        }

        private void OnAdsLoaded()
        {
            HasAdsLoaded = true;
            IsLoadingAds = false;
        }

        private void OnAdsLoadFail(UnityAdsLoadError error, string message)
        {
            IsLoadingAds = false;
            HasAdsLoaded = false;
        }

        private void OnAdsStart()
        {
            HasAdsLoaded = false;
        }

        private void OnAdsShowed(UnityAdsShowCompletionState completionState)
        {
            HasAdsLoaded = false;
        }

        private void OnAdsShowFail(UnityAdsShowError error, string message)
        {
            HasAdsLoaded = false;
        }

        /// <summary>
        /// Show ads video with skip option if has ads load 
        /// </summary>
        public void ShowInterstitialAds(UnityAction onStart = null, UnityAction<UnityAdsShowCompletionState> onShow = null, UnityAction<UnityAdsShowError, string> onShowFail = null)
        {
            ShowAds(INTERSTITIAL, onStart, onShow, onShowFail);
        }

        /// <summary>
        /// Show ads video without skip option if has ads load 
        /// </summary>
        public void ShowRewardedAds(UnityAction onStart = null, UnityAction<UnityAdsShowCompletionState> onShow = null, UnityAction<UnityAdsShowError, string> onShowFail = null)
        {
            ShowAds(REWARDED, onStart, onShow, onShowFail);
        }

        /// <summary>
        /// Load ads video without skip option if has ads load
        /// </summary>
        public void LoadInterstitialAds(UnityAction onLoad = null, UnityAction<UnityAdsLoadError, string> onLoadFail = null)
        {
            LoadAds(INTERSTITIAL, onLoad, onLoadFail);
        }

        /// <summary>
        /// Load ads video without skip option if has ads load
        /// </summary>
        public void LoadRewardedAds(UnityAction onLoad = null, UnityAction<UnityAdsLoadError, string> onLoadFail = null)
        {
            LoadAds(REWARDED, onLoad, onLoadFail);
        }
    }
}
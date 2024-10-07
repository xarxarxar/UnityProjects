using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Watermelon.IAPStore;
using Watermelon.Map;

namespace Watermelon
{
    public class UIMainMenu : UIPage
    {
        public readonly float STORE_AD_RIGHT_OFFSET_X = 300F;

        [SerializeField] RectTransform safeAreaRectTransform;

        [Space]
        [SerializeField] RectTransform tapToPlayRect;
        [SerializeField] Button playButton;
        [SerializeField] TMP_Text playButtonText;

        [Space]
        [SerializeField] UIScaleAnimation coinsLabelScalable;
        [SerializeField] CurrencyUIPanelSimple coinsPanel;
        [SerializeField] UIScaleAnimation livesIndicatorScalable;
        [SerializeField] AddLivesPanel addLivesPanel;

        [Space]
        [SerializeField] UIMainMenuButton iapStoreButton;
        [SerializeField] UIMainMenuButton noAdsButton;

        [Space]
        [SerializeField] UINoAdsPopUp noAdsPopUp;

        private TweenCase tapToPlayPingPong;
        private TweenCase showHideStoreAdButtonDelayTweenCase;

        private void OnEnable()
        {
            IAPManager.OnPurchaseComplete += OnAdPurchased;
        }

        private void OnDisable()
        {
            IAPManager.OnPurchaseComplete -= OnAdPurchased;
        }

        public override void Initialise()
        {
            coinsPanel.Initialise();

            iapStoreButton.Init(STORE_AD_RIGHT_OFFSET_X);
            noAdsButton.Init(STORE_AD_RIGHT_OFFSET_X);

            iapStoreButton.Button.onClick.AddListener(IAPStoreButton);
            noAdsButton.Button.onClick.AddListener(NoAdButton);
            coinsPanel.AddButton.onClick.AddListener(AddCoinsButton);
            playButton.onClick.AddListener(PlayButton);

            NotchSaveArea.RegisterRectTransform(safeAreaRectTransform);
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            showHideStoreAdButtonDelayTweenCase?.Kill();

            HideAdButton(true);
            iapStoreButton.Hide(true);
            ShowTapToPlay();

            coinsLabelScalable.Show();
            livesIndicatorScalable.Show();

            UILevelNumberText.Show();
            playButtonText.text = "LEVEL " + (LevelController.MaxReachedLevelIndex + 1);

            showHideStoreAdButtonDelayTweenCase = Tween.DelayedCall(0.12f, delegate
            {
                ShowAdButton();
                iapStoreButton.Show();
            });

            SettingsPanel.ShowPanel();

            MapLevelAbstractBehavior.OnLevelClicked += OnLevelOnMapSelected;

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            if (!isPageDisplayed)
                return;

            showHideStoreAdButtonDelayTweenCase?.Kill();

            isPageDisplayed = false;

            HideTapToPlayButton();

            coinsLabelScalable.Hide();
            livesIndicatorScalable.Hide();

            HideAdButton();

            showHideStoreAdButtonDelayTweenCase = Tween.DelayedCall(0.1f, delegate
            {
                iapStoreButton.Hide();
            });

            MapLevelAbstractBehavior.OnLevelClicked -= OnLevelOnMapSelected;

            SettingsPanel.HidePanel();

            Tween.DelayedCall(0.5f, delegate
            {
                UIController.OnPageClosed(this);
            });
        }

        #endregion

        #region Tap To Play Label

        public void ShowTapToPlay(bool immediately = false)
        {
            if (tapToPlayPingPong != null && tapToPlayPingPong.IsActive)
                tapToPlayPingPong.Kill();

            if (immediately)
            {
                tapToPlayRect.localScale = Vector3.one;

                tapToPlayPingPong = tapToPlayRect.transform.DOPingPongScale(1.0f, 1.05f, 0.9f, Ease.Type.QuadIn, Ease.Type.QuadOut, unscaledTime: true);

                return;
            }

            // RESET
            tapToPlayRect.localScale = Vector3.zero;

            tapToPlayRect.DOPushScale(Vector3.one * 1.2f, Vector3.one, 0.35f, 0.2f, Ease.Type.CubicOut, Ease.Type.CubicIn).OnComplete(delegate
            {

                tapToPlayPingPong = tapToPlayRect.transform.DOPingPongScale(1.0f, 1.05f, 0.9f, Ease.Type.QuadIn, Ease.Type.QuadOut, unscaledTime: true);

            });

        }

        public void HideTapToPlayButton(bool immediately = false)
        {
            if (tapToPlayPingPong != null && tapToPlayPingPong.IsActive)
                tapToPlayPingPong.Kill();

            if (immediately)
            {
                tapToPlayRect.localScale = Vector3.zero;

                return;
            }

            tapToPlayRect.DOScale(Vector3.zero, 0.3f).SetEasing(Ease.Type.CubicIn);
        }

        #endregion

        #region Ad Button Label

        private void ShowAdButton(bool immediately = false)
        {
            if (AdsManager.IsForcedAdEnabled())
            {
                noAdsButton.Show(immediately);
            }
            else
            {
                noAdsButton.Hide(immediately: true);
            }
        }

        private void HideAdButton(bool immediately = false)
        {
            noAdsButton.Hide(immediately);
        }

        private void OnAdPurchased(ProductKeyType productKeyType)
        {
            if (productKeyType == ProductKeyType.NoAds)
            {
                HideAdButton(immediately: true);
            }
        }

        #endregion

        #region Buttons

        private void PlayButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            OnPlayTriggered(LevelController.MaxReachedLevelIndex);
        }

        private void OnLevelOnMapSelected(int levelId)
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            OnPlayTriggered(levelId);
        }

        private void OnPlayTriggered(int levelId)
        {
            if (LivesManager.Lives > 0)
            {
                // start level
                GameController.LoadLevel(levelId);

                Tween.DelayedCall(2f, LivesManager.RemoveLife);
            }
            else
            {
                addLivesPanel.Show((bool resultSuccessfull) =>
                {
                    if (resultSuccessfull)
                    {
                        // start level
                        GameController.LoadLevel(levelId);

                        Tween.DelayedCall(2f, LivesManager.RemoveLife);
                    }
                });
            }
        }

        private void IAPStoreButton()
        {
            if (UIController.GetPage<UIIAPStore>().IsPageDisplayed)
                return;

            UILevelNumberText.Hide(true);

            UIController.HidePage<UIMainMenu>();
            UIController.ShowPage<UIIAPStore>();

            // reopening main menu only after store page was opened throug main menu
            UIController.OnPageClosedEvent += OnIapStoreClosed;
            MapBehavior.DisableScroll();

            AudioController.PlaySound(AudioController.Sounds.buttonSound);
        }

        private void OnIapStoreClosed(UIPage page, System.Type pageType)
        {
            if (pageType.Equals(typeof(UIIAPStore)))
            {
                UIController.OnPageClosedEvent -= OnIapStoreClosed;

                MapBehavior.EnableScroll();
                UIController.ShowPage<UIMainMenu>();
            }
        }





        private void NoAdButton()
        {
            noAdsPopUp.Show();
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
        }

        private void AddCoinsButton()
        {
            IAPStoreButton();
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
        }

        #endregion
    }


}

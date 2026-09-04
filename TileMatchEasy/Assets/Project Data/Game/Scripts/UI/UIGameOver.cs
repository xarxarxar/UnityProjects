using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIGameOver : UIPage
    {
        [SerializeField] RectTransform safeAreaRectTransform;
        
        [SerializeField] UIScaleAnimation levelFailed;
        [SerializeField] UIFadeAnimation backgroundFade;

        [SerializeField] Button menuButton;
        [SerializeField] Button replayButton;
        [SerializeField] Button reviveButton;

        [SerializeField] UIScaleAnimation menuButtonScalable;
        [SerializeField] UIScaleAnimation replayButtonScalable;
        [SerializeField] UIScaleAnimation reviveButtonScalable;

        [SerializeField] LivesIndicator livesIndicator;
        [SerializeField] AddLivesPanel addLivesPanel;

        private TweenCase continuePingPongCase;

        public override void Initialise()
        {
            menuButton.onClick.AddListener(MenuButton);
            replayButton.onClick.AddListener(ReplayButton);
            reviveButton.onClick.AddListener(ReviveButton);

            LivesManager.AddIndicator(livesIndicator);
            NotchSaveArea.RegisterRectTransform(safeAreaRectTransform);
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            WXAdsManager.Instance.RandomPlayCustom();//随机展示格子广告或者横幅广告
            WXAdsManager.Instance.ShowInterstitialAd();//展示插屏广告

            levelFailed.Hide(immediately: true);
            menuButtonScalable.Hide(immediately: true);
            replayButtonScalable.Hide(immediately: true);
            reviveButtonScalable.Hide(immediately: true);

            float fadeDuration = 0.3f;
            backgroundFade.Show(fadeDuration);

            Tween.DelayedCall(fadeDuration * 0.8f, delegate
            {
                levelFailed.Show();
                TextMeshProUGUI failedText = levelFailed.RectTransform.GetComponentInChildren<TextMeshProUGUI>();
                if (failedText != null)
                {
                    failedText.text = "分数 " + LevelController.EndlessScore;
                }


                menuButtonScalable.Show(scaleMultiplier: 1.05f, delay: 0.75f);
                replayButtonScalable.Show(scaleMultiplier: 1.05f, delay: 0.75f);
                reviveButtonScalable.Show(scaleMultiplier: 1.05f, delay: 0.25f);

                continuePingPongCase = reviveButtonScalable.RectTransform.DOPingPongScale(1.0f, 1.05f, 0.9f, Ease.Type.QuadIn, Ease.Type.QuadOut, unscaledTime: true);

                UIController.OnPageOpened(this);
            });

        }

        public override void PlayHideAnimation()
        {
            WXAdsManager.Instance.CloseGridAds();//隐藏格子广告
            WXAdsManager.Instance.CloseBannerAds();//隐藏横幅广告

            backgroundFade.Hide(0.3f);

            Tween.DelayedCall(0.3f, delegate
            {

                if (continuePingPongCase != null && continuePingPongCase.IsActive)
                    continuePingPongCase.Kill();

                UIController.OnPageClosed(this);
            });
        }

        #endregion

        #region Buttons 

        private void ReviveButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            
        }

        private void ReviveCallback(bool watchedRV)
        {
            if (!watchedRV) return;

            UIController.HidePage<UIGameOver>();
            UIController.ShowPage<UIGame>();

            GameController.Revive();
        }

        private void ReplayButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);


            if (LivesManager.Lives > 0)
            {
                LivesManager.RemoveLife();

                UIController.HidePage<UIGameOver>();
                GameController.ReplayLevel();
            }
            else
            {
                addLivesPanel.Show();
            }
        }

        private void MenuButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            UIController.HidePage<UIGameOver>(() =>
            {
                GameController.ReturnToMenu();
            });
        }

        #endregion
    }
}
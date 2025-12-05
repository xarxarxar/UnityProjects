using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace NINESOFT.TUTORIAL_SYSTEM
{
    public class TutorialModule_VideoPanel : TutorialModule
    {
        [SerializeField] private float buttonActivationDelay = 5f;

        [SerializeField] private Vector2Int videoSize = new Vector2Int(1080, 1920);
        [SerializeField] private int videoDepth = 24;

        [SerializeField] private VideoClip videoClip;
        [SerializeField] private RawImage videoImage;

        [SerializeField] private Image timeBar;
        [SerializeField] private GameObject okButton;
        public override IEnumerator ActiveTheModuleEnum()
        {
            Init();

            transform.GetChild(1).gameObject.SetActive(true);

            float t = 0f;
            while (t < buttonActivationDelay)
            {
                t += Time.deltaTime;
                timeBar.fillAmount = t / buttonActivationDelay;
                yield return new WaitForEndOfFrame();
            }
         
            okButton.gameObject.SetActive(true);

            TweenData td = new TweenData();
            td.Obj = okButton.transform;
            td.scale = true;
            td.TweenAnimation(this);

        }

        private void Init()
        {
            VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
            RenderTexture renderTexture = new RenderTexture(videoSize.x, videoSize.y, videoDepth);
            videoPlayer.clip = videoClip;
            videoPlayer.targetTexture = renderTexture;
            videoImage.texture = renderTexture;
        }
    }
}

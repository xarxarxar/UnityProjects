using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageItemView : MonoBehaviour
{
    public RectTransform bg;
    public RectTransform left;
    public RectTransform right;

    public Text stageTxt;
    public List<Image> stars;
    public Button battleBtn;
    public Button sweepBtn;
    public Button nextChapter;

    public Sprite btnImgOn;
    public Sprite btnImgOff;
    public Sprite starOn;
    public Sprite starOff;

    public Text soulTxt;
    public Text gemTxt;
    public RectTransform gemPart;
    public Button infoBtn;

    public StageModel stageModel;

    public void SetUp(StageModel missionModel)
    {
        bg.gameObject.SetActive(missionModel != null);
        left.gameObject.SetActive(missionModel != null);
        right.gameObject.SetActive(missionModel != null);
        nextChapter.gameObject.SetActive(missionModel == null);

        if (missionModel != null)
        {
            stageTxt.text = missionModel.stage.ToString();
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].sprite = (i < missionModel.numStar) ? starOn : starOff;
            }
            stageModel = missionModel;

            sweepBtn.interactable = missionModel.onSweep;
            sweepBtn.image.sprite = missionModel.onSweep ? btnImgOn : btnImgOff;

            if (string.IsNullOrEmpty(missionModel.gem))
            {
                gemPart.gameObject.SetActive(false);
            }
            else
            {
                gemTxt.text = missionModel.gem;
            }
            soulTxt.text = missionModel.soul;
        }
    }

    public void UpdateData(int sweepAmount)
    {
        sweepBtn.interactable = stageModel.numStar == 3 && sweepAmount > 0;
    }
}

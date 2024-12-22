using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Switch : MonoBehaviour, IPointerClickHandler
{


    public Image icon;
    public Sprite[] spriteIcon;

    public Transform stwitchButton;
    public Image imageHandle;
    public Image imageBg;


    private void Start()
    {
        SetSwitch();
    }


    /// <summary>
    /// Switch on off setting
    /// </summary>
    public void SetSwitch()
    {
        stwitchButton.DOKill();

        if (!GameData.HapticOff)
        {
            icon.sprite = spriteIcon[1];

            imageHandle.DOKill();
            imageBg.DOKill();
            imageHandle.DOColor(Utility.HexToColor("29d2af"), 0.15f).SetEase(Ease.OutCubic).SetUpdate(true);
            imageBg.DOColor(Utility.HexToColor("16958e"), 0.15f).SetEase(Ease.OutCubic).SetUpdate(true);

            stwitchButton.transform.DOKill();
            stwitchButton.DOLocalMoveX(42.3f, 0.2f).SetUpdate(true);


        }
        else
        {
            icon.sprite = spriteIcon[0];

            imageHandle.DOKill();
            imageBg.DOKill();
            imageHandle.DOColor(Utility.HexToColor("6b6185"), 0.15f).SetEase(Ease.OutCubic).SetUpdate(true);
            imageBg.DOColor(Utility.HexToColor("3b3253"), 0.15f).SetEase(Ease.OutCubic).SetUpdate(true);

            stwitchButton.transform.DOKill();
            stwitchButton.DOLocalMoveX(-42.3f, 0.2f).SetUpdate(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameData.HapticOff = !GameData.HapticOff;

        if (!GameData.HapticOff)
        {
#if UNITY_IOS
            if (PlayManager.Instance.isHaptic) {
                //iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactMedium);
                //0 : small 1 : light 2 : midium 3 : heavy 4 : success 5 : warring 6 : falure 7 : onoff 
            }
#endif
        }

        SetSwitch();
    }
}

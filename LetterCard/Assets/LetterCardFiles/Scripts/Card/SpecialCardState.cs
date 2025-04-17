using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialCardState : MonoBehaviour
{
    public static SpecialCardState instance;

    private bool isDeleting;//ÕýÔÚÉ¾³ý¿¨ÅÆ
    public bool IsDeleting 
    { 
        get => isDeleting; 
        set 
        {
            if (value != isDeleting)
            {
                isDeleting = value;
                GameManager.Instance.deleteTip.SetActive(isDeleting);
                ButtonManager.instance.drawCardButton.interactable = !isDeleting;
                ButtonManager.instance.playCardButton.interactable = !isDeleting;
            }
        }
    }


    private void Awake()
    {
        instance=this;
    }

}

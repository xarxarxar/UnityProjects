using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                ShowTipManager.instance.ToggleDustbin(value);
            }
        }
    }

    private void Awake()
    {
        instance=this;
    }
}

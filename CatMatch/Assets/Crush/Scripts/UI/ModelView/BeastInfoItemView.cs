using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeastInfoItemView : MonoBehaviour
{
    public Image image;
    public Image iconDie;
    public Text beastName;

    public Text damageTxt;
    public Text damageValue;
    public Text takeDamageTxt;
    public Text takeDamageValue;

    public Slider slider;

    // private BeastScrollView curScrollView;
    // private BeastInfoItem curItem;

    private byte prevIsShowDamage;

    // void Enable()
    // {
    //     prevIsShowDamage = 0;
    // }

    public void SetUp(BeastInfoItem item, int maxDmg)
    {

        if (item.icon != null) image.sprite = item.icon;

        if (iconDie != null)
        {
            if (item.isDie) iconDie.gameObject.SetActive(true);
            else iconDie.gameObject.SetActive(false);
        }

        if (item.showDamage)
        {
            if (prevIsShowDamage == 0 || prevIsShowDamage == 2)
            {
                damageTxt?.gameObject.SetActive(true);
                damageValue?.gameObject.SetActive(true);
                takeDamageTxt?.gameObject.SetActive(false);
                takeDamageValue?.gameObject.SetActive(false);
            }
            prevIsShowDamage = 1;

            damageValue.text = MergeBeast.Utils.FormatNumber(item.damage);
            slider.maxValue = maxDmg;
            slider.value = item.damage;
        }
        else
        {
            if (prevIsShowDamage == 0 || prevIsShowDamage == 1)
            {
                damageTxt?.gameObject.SetActive(false);
                damageValue?.gameObject.SetActive(false);
                takeDamageTxt?.gameObject.SetActive(true);
                takeDamageValue?.gameObject.SetActive(true);
            }
            prevIsShowDamage = 2;

            takeDamageValue.text = MergeBeast.Utils.FormatNumber(item.damaged);

            slider.maxValue = maxDmg;
            slider.value = item.damaged;
        }

        if (item.beastName != null && beastName != null)
        {
            beastName.text = item.beastName;
        }
    }
}

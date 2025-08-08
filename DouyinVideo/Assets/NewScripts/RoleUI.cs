using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleUI : MonoBehaviour
{
    public int bigCount;//大招点数

    public Text hpText;
    public Text nameText;
    public Slider hpSlider;
    public Image roleImage;

    public List<Image> bigObject;//大招点数

    public void TakeDamage(int hp)
    {
        if(hp<0) hp = 0;
        hpText.text=hp.ToString();
        hpSlider.value=hp;
    }

    public void RecoveryHp(int hp)
    {
        if (hp >= 0) hp = 100;
        hpText.text = hp.ToString();
        hpSlider.value = hp;
    }

    public void AttackOther(Vector3 myPos)
    {
        for(int i = 0; i < bigObject.Count; i++)
        {
            if (bigObject[i].color== new Color(0f, 0f, 0f, 150f / 255f))
            {
                Vector3 startPoint = Camera.main.WorldToScreenPoint(myPos);

                Vector3 endPoint = RectTransformUtility.WorldToScreenPoint(null, bigObject[i].transform.position);
                VideoGameManager.instance.PlayFlyEffect(startPoint, endPoint);
                bigObject[i].color = Color.white;
                break;
            }
        }

    }

    public void SetSliderValue(int value)
    {
        hpSlider.value = value;
        hpText.text = value.ToString();
    }

    //清除大招点数
    public void ClearBig()
    {
        for (int i = 0; i < bigObject.Count; i++)
        {
            bigObject[i].color = new Color(0f, 0f, 0f, 150f / 255f);
        }
    }
}

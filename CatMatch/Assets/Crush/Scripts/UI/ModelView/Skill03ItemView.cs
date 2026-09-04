using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill03ItemView : MonoBehaviour
{
    public Button button;
    public Image processCircleImg;

    public Text timeTxt;
    private int beastWorldIndex;
    private int countDown;
    BeastBase beast;

    public void SetUp(BeastBase beast)
    {
        button.image.sprite = beast.skillDes[2].skillSp;
        this.beastWorldIndex = beast.curIndex;
        this.countDown = beast.skillDes[2].countDown;
        foreach (var sk in beast.skillDes)
        {
            Debug.Log("Skill03ItemView " + sk.skillName + " countDown:" + sk.countDown);
        }
        timeTxt.text = countDown.ToString();

        this.beast = beast;

        StartCoroutine(CountDownIE());
    }

    IEnumerator CountDownIE()
    {
        button.interactable = false;
        int tmpCountDown = countDown;
        timeTxt.text = tmpCountDown.ToString();
        processCircleImg.fillAmount = 1;
        while (tmpCountDown > 0)
        {
            yield return new WaitForSeconds(1f);
            tmpCountDown -= 1;
            timeTxt.text = tmpCountDown.ToString();
            processCircleImg.fillAmount = tmpCountDown * 1f / countDown;
        }
        timeTxt.text = "";
        processCircleImg.fillAmount = 0;
        button.interactable = true;
    }

    public void OnClick()
    {
        StartCoroutine(CountDownIE());

        var pref = BeastPrefs.Instance.GetBeastAllInfo(beast.beastId)?.skill3;
        if (pref != null)
        {
            var go = ObjectPool.Instance.GetGameObject(pref, Vector3.zero, Quaternion.identity);
            go.GetComponent<SkillBase>().Passive(beast);
        }
    }
}

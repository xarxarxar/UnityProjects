using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Slider subHealthBar;
    public Image subFill;

    // public Gradient gradient;
    public GameObject criticalPref;
    // public GameObject dogePref;

    public Image fill;
    // float parentOldPosY = 1000;

    private IEnumerator hideIE;
    private BigInteger maxHealth;
    private BigInteger maxSubHealth;

    public void SetMaxHealth(BigInteger health, Team curTeam, BigInteger subHealth)
    {
        maxHealth = health;

        slider.maxValue = 1;
        slider.value = 1;

        if (curTeam == Team.Enemy) fill.color = Color.red;
        else fill.color = Color.green;

        // fill.color = gradient.Evaluate(1f);

        gameObject.SetActive(false);

        // if (parentOldPosY == 1000)
        // parentOldPosY = transform.parent.position.y;

        if (subHealth == 0)
        {
            subHealthBar.gameObject.SetActive(false);
            maxSubHealth = 0;
        }
        else
        {
            maxSubHealth = subHealth;
            subHealthBar.maxValue = 1;
            subHealthBar.value = 1;
            if (curTeam == Team.Enemy) subFill.color = Color.yellow;
            else subFill.color = Color.blue;
        }
    }

    // public void SetHealth(BigInteger health, float offsetByJump, bool critical, bool doge,
    //     Element attackerElement, Element attackedElement)
    // {
    //     if (doge)
    //     {
    //         // Debug.Log("health bar doge");
    //         var criticalGO = ObjectPool.Instance.GetGameObject(criticalPref, transform.position, UnityEngine.Quaternion.identity);
    //         var criticalOb = criticalGO.GetComponent<Critical>();
    //         criticalOb.SetUp("闪避", null, Color.green);
    //         criticalOb.Move(0);
    //         return;
    //     }

    //     var parentPos = transform.parent.position;

    //     if (offsetByJump != 0) parentPos.y = parentOldPosY + offsetByJump;
    //     else parentPos.y = parentOldPosY;

    //     transform.parent.position = parentPos;

    //     slider.value = (float)((double)health / (double)maxHealth);
    //     // fill.color = gradient.Evaluate(slider.normalizedValue);

    //     if (critical)
    //     {
    //         var criticalGO = ObjectPool.Instance.GetGameObject(criticalPref, transform.position, UnityEngine.Quaternion.identity);
    //         var criticalOb = criticalGO.GetComponent<Critical>();
    //         var sp = GameData.Instance.critSprites.GetCritSprite(attackerElement, attackedElement);
    //         criticalOb.SetUp("暴击", sp, Color.red);
    //         criticalOb.Move(0);
    //     }

    //     if (hideIE != null) StopCoroutine(hideIE);

    //     hideIE = Hide();
    //     StartCoroutine(hideIE);
    // }

    public void SetHealth(UnityEngine.Vector3 position, BigInteger health, BigInteger curSubHp, bool critical, bool doge,
        Element attackerElement, Element attackedElement)
    {
        if (doge)
        {
            // Debug.Log("health bar doge");
            // var criticalGO = ObjectPool.Instance.GetGameObject(dogePref, transform.position, UnityEngine.Quaternion.identity);
            // criticalGO.GetComponent<Critical>().Move(0);
            var criticalGO = ObjectPool.Instance.GetGameObject(criticalPref, transform.position, UnityEngine.Quaternion.identity);
            var criticalOb = criticalGO.GetComponent<Critical>();
            criticalOb.SetUp("Doge", null, Color.green);
            criticalOb.Move(0);
            
            gameObject.SetActive(false);
            return;
        }

        if (position != Constant.Vector3Default)
            transform.position = position;

        if (curSubHp > 0)
        {
            subHealthBar.gameObject.SetActive(true);
            subHealthBar.value = (float)((double)curSubHp / (double)maxSubHealth);
        }
        else
        {
            subHealthBar.gameObject.SetActive(false);
            slider.value = (float)((double)health / (double)maxHealth);
        }

        // fill.color = gradient.Evaluate(slider.normalizedValue);

        if (critical)
        {
            var criticalGO = ObjectPool.Instance.GetGameObject(criticalPref, transform.position, UnityEngine.Quaternion.identity);
            var criticalOb = criticalGO.GetComponent<Critical>();
            var sp = GameData.Instance.critSprites.GetCritSprite(attackerElement, attackedElement);
            criticalOb.SetUp("Crit", sp, Color.red);
            criticalOb.Move(0);
        }

        if (hideIE != null) StopCoroutine(hideIE);

        hideIE = Hide();
        StartCoroutine(hideIE);
    }

    public void ResetY()
    {
        // var parentPos = transform.parent.position;
        // parentPos.y = parentOldPosY;
        // transform.parent.position = parentPos;
    }

    public void OnSpecialSkill(AbilityType specialSkill)
    {
        if (specialSkill != AbilityType.None)
        {
            // Debug.Log("healthbar:" + specialSkill);

            var criticalGO = ObjectPool.Instance.GetGameObject(criticalPref, transform.position, UnityEngine.Quaternion.identity);
            var criticalOb = criticalGO.GetComponent<Critical>();
            criticalOb.SetUp(GameData.Instance.monsterDesSprites.GetAbilityDesSprite(specialSkill).title, null, Color.yellow);
            criticalOb.Move(0.2f);

            // var txt = go.transform.GetChild(0).GetComponent<Text>();
            // txt.text = GameData.Instance.monsterDesSprites.GetAbilityDesSprite(specialSkill).title;
            // txt.color = Color.yellow;
            // go.GetComponent<Critical>().Move(0.2f);
        }
    }

    public void PushMessage(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            // Debug.Log("healthbar:" + specialSkill);

            var criticalGO = ObjectPool.Instance.GetGameObject(criticalPref, transform.position, UnityEngine.Quaternion.identity);
            var criticalOb = criticalGO.GetComponent<Critical>();
            criticalOb.SetUp(message, null, Color.yellow);
            criticalOb.Move(0.2f);

            // var txt = go.transform.GetChild(0).GetComponent<Text>();
            // txt.text = message;
            // txt.color = Color.yellow;
            // go.GetComponent<Critical>().Move(0.2f);
        }
    }

    private IEnumerator Hide()
    {
        yield return new WaitForSeconds(0.35f);

        gameObject.SetActive(false);
    }
}

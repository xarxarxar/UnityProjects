using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelStats : MonoBehaviour
{
    public EnemyScrollViewStats enemyScrollViewStats;
    public MyScrollViewStats myScrollViewStats;
    private List<BeastInfoItem> beastInfoItemsAtEndGame;

    [SerializeField] private Image damageBtn;
    [SerializeField] private Image takeDamageBtn;
    [SerializeField] private Sprite btnOn;
    [SerializeField] private Sprite btnOff;

    public Text title;
    public Text contentDmgBtn;
    public Text contentTakeDmgBtn;

    void Enable()
    {
        title.text = LangManager.Instance.Get("Stats");
        contentDmgBtn.text = LangManager.Instance.Get("Damage");
        contentTakeDmgBtn.text = LangManager.Instance.Get("DamageTaken");
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void BeastInfoItemsAtEndGameCache(List<BeastInfoItem> tmp)
    {
        beastInfoItemsAtEndGame = tmp;

        ClickDamageBtn();
    }

    public void ClickDamageBtn()
    {
        enemyScrollViewStats.UpdateBeastInfoItems(beastInfoItemsAtEndGame, true);
        myScrollViewStats.UpdateBeastInfoItems(beastInfoItemsAtEndGame, true);

        damageBtn.sprite = btnOn;
        takeDamageBtn.sprite = btnOff;
    }

    public void ClickTakeDamageBtn()
    {
        takeDamageBtn.sprite = btnOn;
        damageBtn.sprite = btnOff;

        enemyScrollViewStats.UpdateBeastInfoItems(beastInfoItemsAtEndGame, false);
        myScrollViewStats.UpdateBeastInfoItems(beastInfoItemsAtEndGame, false);
    }
}

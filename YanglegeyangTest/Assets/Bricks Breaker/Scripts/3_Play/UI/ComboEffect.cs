using UnityEngine;
using DG.Tweening;

public class ComboEffect : MonoBehaviour
{
    public SpriteRenderer imageCombo;
    public SpriteRenderer imageCombo_overay;

    public void SetCombo()
    {
        //Combo count
        int count = CtrGame.instance.comboCount;
        Sprite s = CtrUI.instance.spriteCombo[count - 1];
        imageCombo.sprite = s;
        imageCombo_overay.sprite = s;

        if (count == 10)
        {
            //Lucky bonus when combo is 10
            CtrUI.instance.LuckyBonus();

        }


        //Play sound according to the combo
        if (count <= 1)
        {
            SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_block1_destory);
        }
        if (count == 2 || count == 3)
        {
            SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_block2_destory);
        }
        else if (count == 4 || count == 5)
        {
            SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_block3_destory);
        }
        else if (count == 6 || count == 7)
        {
            SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_block4_destory);
        }
        else if (count == 8 || count == 9)
        {
            SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_block5_destory);
        }
        else
        {
            SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_block6_dsstory);
        }

        imageCombo.DOKill();
        imageCombo.transform.DOScale(count + 0.5f, 0f);

        //Camera shaking
        CtrGame.instance.tiltCamera.Shaking(CtrGame.instance.comboCount * 0.02f);
        imageCombo.transform.DOScale(1f, 0.15f).SetEase(Ease.OutBounce);
        int score = CtrGame.instance.comboCount * 10;
        CtrGame.instance.turnScore += score;

        imageCombo.DOKill();
        imageCombo_overay.DOKill();
        imageCombo.DOFade(1f, 0f).SetEase(Ease.OutCubic);
        imageCombo_overay.DOFade(1f, 0f).SetEase(Ease.OutCubic);

        imageCombo_overay.DOFade(0f, 1f).SetEase(Ease.Linear);
        imageCombo.DOFade(0f, 1f).SetEase(Ease.Linear).SetDelay(0.15f)
            .OnComplete(() => { PoolManager.Despawn(this.gameObject); });
    }
}

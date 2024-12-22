using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Block : BlockBase
{
    public SpriteRenderer shadow;
    public ParticleSystem fxHit;

    private int bHealth;

    private float bSpriteCount;
    private float valueB = 0.1f;
    private float valueA = 0.11f;

    private void OnEnable()
    {
        Reset();
    }

    public override void Reset()
    {
        shadow.DOKill();
        shadow.DOFade(0f, 0f);
        mySprite.DOKill();
        mySprite.DOFade(0f, 0f);

        mySprite.transform.DOKill();
        mySprite.transform.DOScale(1f, 0f);

        transform.DOKill();
        transform.DOLocalMoveY(0.3f, 0f);
    }


    public override void SetData(int health)
    {
        int turn = CtrGame.instance.turnCount;
        bSpriteCount = (valueB * turn) + (valueA * turn);

        bHealth = health;

        if (bSpriteCount > CtrBlock.instance.blockSprites.Length - 1)
        {
            bSpriteCount = CtrBlock.instance.blockSprites.Length - 1;
        }

        mySprite.sprite = CtrBlock.instance.blockSprites[(int) bSpriteCount];

        InAnimation();
        base.SetData(health);
    }

    void InAnimation()
    {
        shadow.DOFade(1f, 0.2f).SetEase(Ease.OutCubic);
        mySprite.DOFade(1f, 0.2f).SetEase(Ease.OutCubic);
        transform.DOLocalMoveY(0f, 0.2f).SetEase(Ease.OutCubic);
    }

    public override void InDamage(Collision2D collision)
    {
        base.InDamage(collision);
        if (bHealth <= bSpriteCount)
        {
            int h = blockHealth - 1;
            if (h < 0)
            {
                h = 0;
            }

            mySprite.sprite = CtrBlock.instance.blockSprites[h];
        }
        else
        {
            float value = (float) blockHealth / (((float) bHealth / bSpriteCount));
            mySprite.sprite = CtrBlock.instance.blockSprites[(int) value];
        }
    }

    public override void HitFx()
    {
        textHealht.transform.DOKill();
        textHealht.transform.DOScale(0.1f, 0f);
        textHealht.transform.DOScale(0.125f, 0.05f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            textHealht.transform.DOScale(0.1f, 0.05f).SetEase(Ease.OutCubic);
        });

        fxHit.Play();
        //PoolManager.Despawn(PoolManager.Spawn(CtrPool.instance.pFxBlockHit, transform.position, Quaternion.identity), 1f);

        mySprite.DOKill();
        mySprite.transform.DOScale(0.95f, 0.1f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            mySprite.transform.DOScale(1f, 0.1f).SetEase(Ease.OutCirc);
        });
        //spriteHit.DOKill();
        //spriteHit.DOFade(0f, 0f);
        //spriteHit.DOFade(1f, 0.05f).SetEase(Ease.OutCubic).OnComplete(()=> {
        //    spriteHit.DOFade(0f, 0.1f).SetEase(Ease.Linear);
        //});

        base.HitFx();
    }

}
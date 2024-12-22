using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BlockBase : MonoBehaviour {
    [HideInInspector] public BlockGroup blockGroup;
    public SpriteRenderer mySprite;
    public TextMeshPro textHealht;
    [HideInInspector] public bool isBall = false;
    [HideInInspector] public int blockHealth;
    [HideInInspector] public bool isDestory = false;

    public virtual void SetData (int health) {
        isDestory = false;
        if (textHealht != null) {
            textHealht.text = Utility.ChangeThousandsSeparator(health);
            this.blockHealth = health;
        }
    }

    private void OnCollisionEnter2D (Collision2D collision) {
        if (isDestory) return;

        if (collision.gameObject.CompareTag("Ball")) {
            InDamage(collision);
        }
    }

    public virtual void InDamage (Collision2D collision) {
        HitFx();

        //볼데미지 만큼 체력 깍기
        blockHealth -= collision.gameObject.GetComponent<Ball>().damage;
        textHealht.text = Utility.ChangeThousandsSeparator(blockHealth);

        if (blockHealth <= 0) {
            if (CtrGame.instance.comboCount < 10) {
                CtrGame.instance.comboCount += 1;
            }

            ComboEffect comboEffect = PoolManager.Spawn(CtrPool.instance.pComboEffect, transform.position, Quaternion.identity).GetComponent<ComboEffect>();
            comboEffect.SetCombo();


            //블럭체력이 0이되서 파괴됨
            blockHealth = 0;
            Destory();
            CtrBlock.instance.CheckAllClear();
        }
    }

    //바닥 라인에 닿았을경우
    //public void OnTriggerEnter2D (Collider2D collision) {
    //    if (collision.gameObject.CompareTag("InTrigger")) {
    //        if (!CtrGame.instance.isGameOver) {
    //            CtrGame.instance.isGameOver = true;
    //            Debug.Log("InTrigger");
    //            CtrGame.instance.GameOver();
    //        }

    //        Destory();
    //    }
    //}

    //블럭 파괴
    public virtual void Destory () {
        if (isDestory) return;
        isDestory = true;

        if (!isBall) {
            //공이 아닐경우에는 블럭 이펙트 및 블럭 파괴 사운드 재생
            PoolManager.Despawn(PoolManager.Spawn(CtrPool.instance.pFxBlockBoom, transform.position, Quaternion.identity), 1f);

            //todo savedata
            PlayManager.Instance.countBreakeBrick++;

#if UNITY_IOS
            if (PlayManager.Instance.isHaptic) {
                //iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactMedium);
                //0 : small 1 : light 2 : midium 3 : heavy 4 : success 5 : warring 6 : falure 7 : onoff 
            }
#endif
        } 
        //초기화
        Reset();
        blockGroup.blockBases.Remove(this);
        PoolManager.Despawn(this.gameObject);
    }

    public virtual void Reset () { }
    public virtual void HitFx () { }
}

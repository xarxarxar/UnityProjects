using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonRocket : MonoBehaviour {

    bool isRocketReady = false;
    public GameObject rocketArea;
    float reloadCount = 0;

#if UNITY_EDITOR
    float reloadMaxCount = 2;
#else
    float reloadMaxCount = 25;
#endif

    public Image rocketFiilAmount;
    public Transform[] rokectShotPos;
    public ParticleSystem fxRocketOn;
    public Image imageRocket;

    public float ReloadCount {
        get {
            return reloadCount;
        }
        set {
            reloadCount = value;
        }
    }

    public float ReloadMaxCount {
        get {
            return reloadMaxCount;
        }
        set {
            reloadMaxCount = value;
        }
    }



    public void Click_Rocket () {
        if (CtrGame.instance.IsLock) return;
        if (!isRocketReady) return;
        isRocketReady = false;

        SoundManager.Instance.PlayEffect(SoundList.sound_rocket_sfx_ready);

        //Hide player UI
        Player.instance.gameObject.SetActive(false);
        CtrUI.instance.textBallCount.gameObject.SetActive(false);

        //Turn on line selection UI
        rocketArea.SetActive(true);
        reloadCount = reloadMaxCount;

        buttonClose.SetActive(true);

        //Button off
        gameObject.SetActive(false);
    }


    public void CheckRocketCoolTime () {
        if (isRocketReady) return;

        //Progress settings
        reloadCount += 1;
        SetFillAmount();
    }

    public void SetFillAmount () {
        rocketFiilAmount.DOFillAmount(reloadCount / reloadMaxCount, 0.15f).SetEase(Ease.InOutCubic);

        if (reloadCount >= reloadMaxCount) {
            SoundManager.Instance.PlayEffect(SoundList.sound_rocket_sfx_cool);
            Reload();
        } 
    }

    public void Reload () {
        //Reload completed
        reloadCount = reloadMaxCount;
        rocketFiilAmount.transform.GetChild(0).gameObject.SetActive(true);
        imageRocket.DOFade(1f, 0f);
        isRocketReady = true;
        fxRocketOn.Play();
    }

    public GameObject buttonClose;

    public void Cancel () {
        //Turn off the rocket launcher panel
        rocketArea.SetActive(false);

        //Cancel button off
        buttonClose.SetActive(false);

        //Turn on the rocket button
        gameObject.SetActive(true);

        //Reload
        Reload();
    }


    public void Click_Rocket_Area (int num) {

        SoundManager.Instance.PlayEffect(SoundList.sound_rocket_sfx_launch);

        //Cancel button off
        buttonClose.SetActive(false);

        //Button and Progress Settings
        rocketFiilAmount.transform.GetChild(0).gameObject.SetActive(false);
        imageRocket.DOFade(0.5f, 0f);

        //Cool time start setting
        rocketFiilAmount.fillAmount = 0;
        reloadCount = 0;


        SetFillAmount();


        //Turn on the button
        gameObject.SetActive(true);

        //Show player UI
        Player.instance.gameObject.SetActive(true);
        CtrUI.instance.textBallCount.gameObject.SetActive(true);

        //Rocket launch
        Vector2 pos = rokectShotPos[num].transform.position;
        pos.y -= 3f;

        GameObject roket = PoolManager.Spawn(CtrPool.instance.pRoket, pos, Quaternion.identity);
        roket.GetComponent<Roket>().SetRoket();
        //PoolManager.Despawn(roket, 1.5f);

        //Turn off line selection UI
        rocketArea.SetActive(false);



    }
}

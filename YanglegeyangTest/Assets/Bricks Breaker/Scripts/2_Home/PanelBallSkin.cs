using System.Collections.Generic;
using UnityEngine;

public enum CostType {
    Default,
    Coin,
    Gem,
    Ads
}

[System.Serializable]
public class BallSkinData {
    [HideInInspector]
    public int id;
    [HideInInspector]
    public bool isUnlock;

    public string ballName;
    public CostType costType;
    public int cost;
}

public class PanelBallSkin : PanelBase {

    public BallSkinData[] ballDatas;
    public GameObject pListSlot;
    public Transform listTransform;
    public PanelBallSkinList selectSkinList;
    public List<PanelBallSkinList> panelBallLists = new List<PanelBallSkinList>();
    public Sprite[] ballSprites;
    
    /// <summary>
    /// Ball Skin Data Settings
    ///Set each list with the ball data.
    /// </summary>
    public override void SetData () {
        for (int i = 0; i < ballDatas.Length; i++) {
            ballDatas[i].id = i;
            GameObject obj = Instantiate(pListSlot);
            obj.transform.SetParent(listTransform, false);

            PanelBallSkinList skinList = obj.GetComponent<PanelBallSkinList>();
            skinList.ballSkinData = ballDatas[i];
            skinList.SetList();

            panelBallLists.Add(skinList);
        }

        panelBallLists[GameData.SelectBallNum].Select();
    }

    

    /// <summary>
    /// Get the Ballskin Sprite by name.
    /// </summary>
    public Sprite GetBallSprite (string name) {
        Debug.Log(name);

        foreach (Sprite sprite in ballSprites) {

            if (name == sprite.name) {
                return sprite;
            }
        }
        return null;
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TeamBuffSprite
{
    public TeamBuff teamBuff;
    public List<BullForSprite> buffForSprites;
}

[System.Serializable]
public class BullForSprite
{
    public Element buffFor;
    public BeastClass buffForClass = BeastClass.None;

    public Sprite sprite;
}

public class TeamBuffSprites : MonoBehaviour
{
    private static TeamBuffSprites instance;
    public static TeamBuffSprites Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("TeamBuffSprites");
                instance = gameObject.AddComponent<TeamBuffSprites>();
                DontDestroyOnLoad(gameObject);
            }
            return instance;
        }
        private set { }
    }
    public Sprite defaultSprite;

    public List<TeamBuffSprite> teamBuffSprites;
    public Dictionary<int, Sprite> teamBuffSpritePair;

    private void Awake()
    {
        if (instance != null && instance.GetInstanceID() != this.GetInstanceID())
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as TeamBuffSprites;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        teamBuffSpritePair = new Dictionary<int, Sprite>();
        var elementLength = Enum.GetNames(typeof(Element)).Length;

        foreach (var teamBuffSprite in teamBuffSprites)
        {
            foreach (var buffForSprite in teamBuffSprite.buffForSprites)
            {
                var key = TeamBuffHelper.GenKey(buffForSprite.buffFor, buffForSprite.buffForClass, (int)teamBuffSprite.teamBuff);
                // Debug.Log("key 2222222:" + key);
                teamBuffSpritePair[key] = buffForSprite.sprite;
            }
        }
    }

    public Sprite GetIcon(int key)
    {
        return teamBuffSpritePair.ContainsKey(key) ? teamBuffSpritePair[key] : defaultSprite;
    }
}

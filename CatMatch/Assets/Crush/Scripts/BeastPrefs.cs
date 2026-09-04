using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[System.Serializable]
public class BeastAllInfo
{
    public BeastId beastId;
    public GameObject pref;
    public GameObject skill3;
}

public class BeastPrefs : MonoBehaviour
{
    private static BeastPrefs instance;
    public static BeastPrefs Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("BeastPrefs");
                instance = gameObject.AddComponent<BeastPrefs>();
                DontDestroyOnLoad(gameObject);
            }
            return instance;
        }
        private set { }
    }

    public List<BeastAllInfo> prefs;
    public Dictionary<BeastId, BeastAllInfo> namePrefs;

    private void Awake()
    {
        if (instance != null && instance.GetInstanceID() != this.GetInstanceID())
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as BeastPrefs;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        namePrefs = new Dictionary<BeastId, BeastAllInfo>();
        foreach (var pref in prefs)
        {
            namePrefs[pref.beastId] = pref;
        }

        var playerData = PlayerData.instance;
    }

    public GameObject GetBeastPref(BeastId beastId)
    {
        return namePrefs[beastId].pref;
    }

    public BeastAllInfo GetBeastAllInfo(BeastId beastId)
    {
        return namePrefs[beastId];
    }
}

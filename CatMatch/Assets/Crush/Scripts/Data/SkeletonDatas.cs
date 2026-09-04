using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[System.Serializable]
public class SkeletonBeasts
{
    public BeastId beastId;
    public SkeletonGraphic graphic;
}

public class SkeletonDatas : MonoBehaviour
{
   private static SkeletonDatas instance;
    public static SkeletonDatas Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("SkeletonDatas");
                instance = gameObject.AddComponent<SkeletonDatas>();
                DontDestroyOnLoad(gameObject);
            }
            return instance;
        }
        private set { }
    }

    public List<SkeletonBeasts> skeletonBeasts;
    public Dictionary<BeastId, SkeletonGraphic> skeletonBeastPair;

    private void Awake()
    {
        if (instance != null && instance.GetInstanceID() != this.GetInstanceID())
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as SkeletonDatas;
            DontDestroyOnLoad(gameObject);
        }

        skeletonBeastPair = new Dictionary<BeastId, SkeletonGraphic>();

       for (int i = 0; i < skeletonBeasts.Count; i++)
       {
           skeletonBeastPair.Add(skeletonBeasts[i].beastId, skeletonBeasts[i].graphic);
       }
    }
}

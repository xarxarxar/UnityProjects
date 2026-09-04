using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelInfo : MonoBehaviour
{
    public GameObject scrollViewGO;
    public BeastScrollView beastScrollView;
    public SpriteDuplicator mark;

    [SerializeField] Transform skill03Parent;
    [SerializeField] GameObject skill03ItemViewPref;

    private Dictionary<int, GameObject> cacheSkill03 = new Dictionary<int, GameObject>();

    public void RestartGame()
    {
        beastScrollView.Clear();

        var tmp = transform.position;
        tmp.y = mark.bottomY;

        // Debug.Log(mark.transform.position.y + " zz: " + mark.size.y * 0.5f);

        transform.position = tmp;

        while (skill03Parent.childCount > 0)
        {
            var go = skill03Parent.GetChild(0).gameObject;
            ObjectPool.Instance.ReleaseObject(go);
        }
    }

    public void InitSkill3(BeastBase beast)
    {
        // var go = ObjectPool.Instance.GetGameObject(skill03ItemViewPref, Vector3.zero, Quaternion.identity);
        // go.transform.SetParent(skill03Parent);
        // go.transform.localScale = new Vector3(1, 1, 1);
        // go.GetComponent<Skill03ItemView>().SetUp(beast);
        // cacheSkill03[beast.curIndex] = go;
    }

    public void OnBeastDie(BeastBase beast)
    {
        if (cacheSkill03.ContainsKey(beast.curIndex))
            ObjectPool.Instance.ReleaseObject(cacheSkill03[beast.curIndex]);
    }
}

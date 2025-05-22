using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    public DecalDatabase decalDatabase_meijia;
    public DecalDatabase decalDatabase_tiehua;

    public GameObject decalItemPrefab_Normal;//普通toggle的预制体
    public GameObject decalItemPrefab_Purple;//紫色toggle的预制体
    public GameObject decalItemPrefab_Golden;//金色toggle的预制体

    public Transform tattooContent;
    //public Transform nailContent;

    public Text descriptionText;//描述该道具的Text
    public Text priceText;//该道具的价格
    public Text equipText;//该道具是否已装备
    public Button buyButton;
    public Button equipButton;
    private GameObject lastEquippedItemUI; // 用于记录上一个已装备的Item UI


    public Material targetMaterial_meijia; // 你用来显示贴花的材质
    public Material targetMaterial_tiehua; // 你用来显示贴花的材质
    public Material myhandMeijiaMaterial; // 我的手的真正游戏中的材质
    public Material myhandTiehuaMaterial; // 我的手的真正游戏中的材质
    public GameObject renderHand;//用来展示的手的模型
    public Camera renderCamera;//用来展示模型的摄像机
    public Text countText;//显示有多少皮肤和皮肤总数
    public Text coinText;//显示有多少金币

    private void OnEnable()
    {
        coinText.text=GameManager.instance.gameInfo.coinCount.ToString();
    }

    void Start()
    {
        PopulateDecalUI(decalDatabase_meijia);
        if (tattooContent.childCount > 0)
        {
            var firstToggle = tattooContent.GetChild(0).GetComponent<Toggle>();
            firstToggle.isOn = true;
        }
    }

    public void PopulateDecalUI(DecalDatabase decalDatabase)
    {
        ClearContent(tattooContent);

        Material myhandMaterial=null;
        Material targetMaterial = null;
        if (decalDatabase== decalDatabase_meijia)
        {
            myhandMaterial=myhandMeijiaMaterial;
            targetMaterial= targetMaterial_meijia;
            MoveRotateScale(renderHand.transform,new Vector3(0.07f,-0.65f,5.58f),new Vector3(0,0,3.42f),new Vector3(1,1,1));
            TweenCameraSize(renderCamera,0.5f);
        }
        else
        {
            myhandMaterial = myhandTiehuaMaterial;
            targetMaterial = targetMaterial_tiehua;
            MoveRotateScale(renderHand.transform, new Vector3(-0.16f, 3.14f, 5.58f), new Vector3(0, 0, 3.42f), new Vector3(1, 1, 1));
            TweenCameraSize(renderCamera, 2.06f);
        }
        // 获取已拥有的皮肤数量
        int ownedCount = decalDatabase.decalList.Count(decal => decal.isOwned);
        countText.text = $"全部：<color=#008DD4>{ownedCount}</color>/{decalDatabase.decalList.Count}";

        decalDatabase.decalList.Sort((a, b) =>
        {
            // 1. isOwned: false 前面
            if (a.isOwned != b.isOwned)
                return a.isOwned ? -1 : 1;

            // 2. isEquip: true 前面（仅在 isOwned 相同前提下）
            //if (a.isOwned && b.isOwned && a.isEquip != b.isEquip)
            //    return a.isEquip ? -1 : 1;

            // 3. level: Normal < Purple < Golden
            return a.level.CompareTo(b.level);
        });

        for (int i = 0; i < decalDatabase.decalList.Count; i++)
        {
            DecalData decal = decalDatabase.decalList[i];
            GameObject prefab = GetPrefabByLevel(decal.level);


            Transform parentContent = tattooContent;

            GameObject item = Instantiate(prefab, parentContent);

            //item.transform.SetSiblingIndex(0);

            item.transform.Find("是否已拥有Label").gameObject.SetActive(decal.isOwned);
            item.transform.Find("是否已装备Label").gameObject.SetActive(decal.isEquip);

            if (decal.isEquip)
            {
                lastEquippedItemUI = item;
            }

            Toggle toggle = item.GetComponent<Toggle>();
            if (toggle != null)
            {
                ToggleGroup toggleGroup = parentContent.GetComponent<ToggleGroup>();
                if (toggleGroup != null)
                {
                    toggle.group = toggleGroup;
                }

                // 捕获当前循环变量，避免闭包问题
                DecalData currentDecal = decal;

                toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        descriptionText.text = currentDecal.description;
                        priceText.text = currentDecal.isOwned ? "" : currentDecal.price.ToString();
                        equipText.text = currentDecal.isEquip ? "已装备" : "装备";
                        
                        buyButton.gameObject.SetActive(!currentDecal.isOwned);
                        equipButton.gameObject.SetActive(currentDecal.isOwned);

                        if (targetMaterial != null && currentDecal.texture != null)
                        {
                            targetMaterial.SetTexture("_DecalTex", currentDecal.texture);
                        }

                        // 清除旧的监听
                        buyButton.onClick.RemoveAllListeners();
                        equipButton.onClick.RemoveAllListeners();

                        buyButton.onClick.AddListener(() =>
                        {
                            if (CoinManager.instance.ReduceCoin(currentDecal.price))
                            {
                                coinText.text = GameManager.instance.gameInfo.coinCount.ToString();

                                currentDecal.isOwned = true;

                                // 重新设置 description 和 price（避免新创建的 description 为空）
                                currentDecal.UpdateDescriptionAndPrice();

                                // 关键：重新生成 UI，重新排序和刷新界面
                                PopulateDecalUI(decalDatabase);

                                // 不需要下面这堆代码了
                                // item.transform.Find("是否已拥有Label").gameObject.SetActive(true);
                                // buyButton.gameObject.SetActive(false);
                                // equipButton.gameObject.SetActive(true);
                                // priceText.text = "";
                                // equipText.text = "装备";
                                // int ownedCount = decalDatabase.decalList.Count(d => d.isOwned);
                                // countText.text = $"全部：<color=#008DD4>{ownedCount}</color>/{decalDatabase.decalList.Count}";
                            }
                            else
                            {
                                Debug.Log("金币不足");
                            }
                        });

                        equipButton.onClick.AddListener(() =>
                        {
                            // 所有皮肤取消装备
                            foreach (var d in decalDatabase.decalList)
                            {
                                d.isEquip = false;
                            }

                            currentDecal.isEquip = true;

                            // 设置当前贴图
                            if (myhandMaterial != null && currentDecal.texture != null)
                            {
                                myhandMaterial.SetTexture("_DecalTex", currentDecal.texture);
                            }

                            //隐藏上一个已装备标签
                            if (lastEquippedItemUI != null)
                            {
                                Transform lastLabel = lastEquippedItemUI.transform.Find("是否已装备Label");
                                if (lastLabel != null)
                                    lastLabel.gameObject.SetActive(false);
                            }

                            //显示当前的已装备标签
                            Transform currentLabel = item.transform.Find("是否已装备Label");
                            if (currentLabel != null)
                                currentLabel.gameObject.SetActive(true);

                            // 更新引用
                            lastEquippedItemUI = item;

                            // 更新UI文本
                            equipText.text = "已装备";
                        });
                    }
                });
            }

            var icon = item.transform.Find("Icon").GetChild(0)?.GetComponent<Image>();
            if (icon != null)
            {
                icon.sprite = decal.GetThumbnail();
            }
        }


    }
    


    void ClearContent(Transform content)
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    GameObject GetPrefabByLevel(SkinLevel level)
    {
        switch (level)
        {
            case SkinLevel.Normal: return decalItemPrefab_Normal;
            case SkinLevel.Purple: return decalItemPrefab_Purple;
            case SkinLevel.Golden: return decalItemPrefab_Golden;
            default: return decalItemPrefab_Normal;
        }
    }

    void SetupItemUI(GameObject item, DecalData decal)
    {
        // 描述文本
        if (descriptionText != null)
            descriptionText.text = decal.description;

        // 装备文本和按钮显示
        if (decal.isOwned)
        {
            if (equipText != null)
                equipText.text = decal.isEquip ? "已装备" : "装备";

            if (buyButton != null)
                buyButton.gameObject.SetActive(false);

            if (equipButton != null)
                equipButton.gameObject.SetActive(true);
        }
        else
        {
            if (priceText != null)
                priceText.text = $"{decal.price}";

            if (buyButton != null)
                buyButton.gameObject.SetActive(true);

            if (equipButton != null)
                equipButton.gameObject.SetActive(false);

            if (equipText != null)
                equipText.text = "";
        }
    }

    /// <summary>
    /// 在指定时间内同时移动、旋转和缩放目标物体，返回 Sequence。
    /// </summary>
    /// <param name="target">要操作的物体 Transform</param>
    /// <param name="targetPosition">目标位置</param>
    /// <param name="targetRotation">目标旋转角度（欧拉角）</param>
    /// <param name="targetScale">目标缩放</param>
    /// <param name="duration">动画持续时间（默认 1 秒）</param>
    /// <returns>返回 DOTween 的 Sequence 对象</returns>
    public static Sequence MoveRotateScale(Transform target, Vector3 targetPosition, Vector3 targetRotation, Vector3 targetScale, float duration = 1f)
    {
        Sequence sequence = DOTween.Sequence();

        // 使用本地坐标移动
        sequence.Join(target.DOLocalMove(targetPosition, duration));

        // 使用本地旋转
        sequence.Join(target.DOLocalRotate(targetRotation, duration));

        // 本地缩放（这个本来就是 localScale，不用改）
        sequence.Join(target.DOScale(targetScale, duration));

        return sequence;
    }

    /// <summary>
    /// 使用 DOTween 动画改变摄像机的 Orthographic Size。
    /// </summary>
    /// <param name="cam">目标摄像机</param>
    /// <param name="targetSize">目标大小</param>
    /// <param name="duration">持续时间</param>
    /// <returns>返回 Tween 对象</returns>
    public static Tween TweenCameraSize(Camera cam, float targetSize, float duration = 1f)
    {
        return DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, targetSize, duration);
    }
}

[System.Serializable]
public class SkinData
{
    public string id;             // 唯一标识符
    public string name;           // 显示名称
    public SkinType type;         // 类型
    public Sprite previewIcon;    // UI 预览图
    public GameObject prefab;     // 实际应用在手上的预制体
    public bool isUnlocked;       // 是否已解锁
}

public enum SkinType
{
    NailArt,    // 美甲
    Tattoo,     // 贴画
    // Future: Ring, Bracelet, etc.
}

public enum SkinLevel
{
    Normal,//普通
    Purple,//紫色
    Golden//金色
}

[System.Serializable]
public class DecalData
{
    public Texture2D texture;
    public SkinLevel level;
    public string description;
    public int price;
    public bool isOwned;
    public bool isEquip;
    public SkinType type;

    private static readonly List<string> meijiaDescription = new List<string>
    {
        "每轮倒计时长+0.3秒",
        "每轮倒计时长+0.6秒",
        "每轮倒计时长+1秒"
    };

    private static readonly List<string> tiehuaDescription = new List<string>
    {
        "每轮PK胜利时额外+1金币",
        "每轮PK胜利时额外+2金币",
        "每轮PK胜利时额外+5金币"
    };

    public void UpdateDescriptionAndPrice()
    {
        int index = (int)level;
        if (type == SkinType.NailArt)
        {
            description = meijiaDescription[index];
            price = 100 + index * 50;
        }
        else if (type == SkinType.Tattoo)
        {
            description = tiehuaDescription[index];
            price = 1000 + index * 500;
        }
    }

    public Sprite GetThumbnail()
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}





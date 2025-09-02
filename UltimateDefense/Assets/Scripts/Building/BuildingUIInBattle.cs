using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 这个是要显示在局内的建筑菜单栏里的元素
/// </summary>
public class BuildingUIInBattle : MonoBehaviour
{
    public BuildingBase buildBase;
    private Button buildButton;
    private Image icon;
    public int _index;

    private void OnDisable()
    {
        
    }
    
    public void Init(BuildingBase data, int index)
    {
        icon =transform.GetChild(0).GetComponent<Image>();
        buildButton= GetComponent<Button>();
        _index = index;
        icon.sprite = data.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        buildBase= data;

        float zRotation = buildBase.transform.GetChild(0).eulerAngles.z;
        icon.rectTransform.localEulerAngles = new Vector3(0, 0, zRotation);
        buildButton.onClick.AddListener(Build);
    }

    public void UpdateUI(BuildingBase data, int index)
    {
        buildButton.onClick.RemoveAllListeners();
        _index = index;
        icon.sprite = data.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        buildBase = data;
        float zRotation = buildBase.transform.GetChild(0).eulerAngles.z;
        icon.rectTransform.localEulerAngles = new Vector3(0, 0, zRotation);
        buildButton.onClick.AddListener(Build);
    }

    public void Build()
    {
        if (BuildManager.instance.SelectedGrid && !BuildManager.instance.SelectedGrid.IsOccupied)
        {
            BuildingBase building = Instantiate(buildBase, BuildManager.instance.SelectedGrid.transform.position,
                Quaternion.identity, BuildManager.instance.buildingParent);
            BuildManager.instance.SelectedGrid.Occupy(building);
            building.Init(10, BuildManager.instance.SelectedGrid);

            //播放流光
            // 起点：按钮位置（UI坐标 → 屏幕坐标）
            Vector3 screenStart = RectTransformUtility.WorldToScreenPoint(null, buildButton.transform.position);

            // 终点：世界坐标 → 屏幕坐标
            Vector3 screenEnd = Camera.main.WorldToScreenPoint(BuildManager.instance.SelectedGrid.transform.position);

            //GameUIManager.Instance.PlayFlyEffect(screenStart, screenEnd);
        }
    }
}

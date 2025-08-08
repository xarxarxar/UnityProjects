using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUIInBattle : MonoBehaviour
{
    public BuildingBase buildBase;
    public Button buildButton;
    public Image icon;

    private void OnEnable()
    {
        buildButton.onClick.AddListener(Build);
        icon.sprite = buildBase.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        float zRotation = buildBase.transform.GetChild(0).eulerAngles.z;
        icon.rectTransform.localEulerAngles = new Vector3(0, 0, zRotation);
    }

    private void OnDisable()
    {
        buildButton.onClick.RemoveAllListeners();
    }

    public void Build()
    {
        if (BuildManager.instance.SelectedGrid && !BuildManager.instance.SelectedGrid.IsOccupied)
        {
            BuildingBase building = Instantiate(buildBase, BuildManager.instance.SelectedGrid.transform.position, 
                Quaternion.identity, BuildManager.instance.buildingParent);
            BuildManager.instance.SelectedGrid.Occupy(building);
            building.Init(10);

            //播放流光
            // 起点：按钮位置（UI坐标 → 屏幕坐标）
            Vector3 screenStart = RectTransformUtility.WorldToScreenPoint(null, buildButton.transform.position);

            // 终点：世界坐标 → 屏幕坐标
            Vector3 screenEnd = Camera.main.WorldToScreenPoint(BuildManager.instance.SelectedGrid.transform.position);

            GameUIManager.Instance.PlayFlyEffect(screenStart, screenEnd);
        }
    }
}

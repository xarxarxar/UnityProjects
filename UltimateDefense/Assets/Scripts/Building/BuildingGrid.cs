using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 这个是显示在局外的建筑菜单栏里的元素
/// </summary>
public class BuildingGrid : MonoBehaviour
{
    [SerializeField] private Text _nameText;//名称Text
    [SerializeField] private Text _desText;//描述Text
    [SerializeField] private Image _iconImage;//图标
    public int _index;//该科技节点所对应的index

    public void Init(BuildingGridData data,int index)
    {
        _index = index;
        _nameText.text= data.Name;
        _desText.text= data.Des;
        _iconImage.sprite= data.Icon;

        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log($"我是{_index}");
        });
    }

    public void UpdateUI(BuildingGridData data, int index)
    {
        _index = index;
        _nameText.text = data.Name;
        _desText.text = data.Des;
        _iconImage.sprite = data.Icon;
    }
}

[System.Serializable]
public class BuildingGridData
{
    public string Name;
    public string Des;
    public Sprite Icon;
    public BuildingBase BuildingBase;
    public int Price;//它在局内建造的价格
}

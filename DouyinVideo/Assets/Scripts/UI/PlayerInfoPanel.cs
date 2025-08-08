using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanel : MonoBehaviour
{
    [SerializeField] private Text _passCountText;

    private void OnEnable()
    {
        _passCountText.text=DataManager.Instance.PlayerInfo.PassCount.ToString();
    }
}

using DanielLochner.Assets.SimpleScrollSnap;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class DrawBlessPanel : BasePanel
{
    [SerializeField]private LoopListView2 _loopListView;
    [SerializeField] private BindableButton _drawButton;//抽奖按钮
    [SerializeField] private RewardStruct _cost;//抽奖所需的花费
    [SerializeField] private SimpleScrollSnap _simpleScrollSnap;//抽奖所在的区域

    public override void OnEnable()
    {
        base.OnEnable();
        if (BlessManager.Instance == null)
        {
            Debug.Log("BlessManager.Instance == null");
        }


        _drawButton.AddListener(DrawBless);
        //拥有的钻石数量要大于等于花费的数量
        //_drawButton.interactable=MetaCurrencyManager.Instance.DiamondCount.Value>= BlessManager.Instance.BlessCost;
        
        _simpleScrollSnap.OnPanelCentered.AddListener(OnPanelCentered);
    }

    private void OnDisable()
    {
        _simpleScrollSnap.GoToPanel(1);
        _drawButton.RemoveAllListeners();
    }

    //抽取祝福
    private void DrawBless()
    {
        _simpleScrollSnap.Velocity += Random.Range(10000, 20000) * Vector2.right;

        

        //更新按钮UI
        //_drawButton.interactable = MetaCurrencyManager.Instance.DiamondCount.Value >= BlessManager.Instance.BlessCost;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            _simpleScrollSnap.Velocity += Random.Range(10000, 20000) * Vector2.right;
            _simpleScrollSnap.OnPanelSelected.AddListener((value) =>
            {
                Debug.Log($"{_simpleScrollSnap.Content.GetChild(value).name}");
            });
        }
        if(Input.GetKeyUp(KeyCode.Q))
        {
            _simpleScrollSnap.GoToPanel(1);
        }
    }

    private void OnPanelCentered(int index,int preIndex)
    {
        Debug.Log($"{_simpleScrollSnap.Content.GetChild(index).name}");
    }
}

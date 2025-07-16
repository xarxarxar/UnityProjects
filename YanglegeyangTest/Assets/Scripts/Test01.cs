using DanielLochner.Assets.SimpleScrollSnap;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test01 : MonoBehaviour
{
    [SerializeField] private SimpleScrollSnap _simpleScrollSnap;//
    [SerializeField] private ScrollRect _scrollRect;//

    private void Start()
    {
        Debug.Log($"{_scrollRect.decelerationRate}");
        //_simpleScrollSnap.OnPanelSelected.AddListener(OnPanelSelected);
        //_simpleScrollSnap.Velocity += Random.Range(10000, 20000) * Vector2.up;
        //_simpleScrollSnap.GoToNextPanel();
        // 保留方向，改变长度
        
        StartCoroutine(SpinAndStop());
    }

    //元素改变
    private void OnPanelSelected(int a)
    {
        //Debug.Log($"{_simpleScrollSnap.SelectedPanel}");
        Debug.Log($"{_simpleScrollSnap.Content.GetChild(_simpleScrollSnap.CenteredPanel).name}");
        
    }

    public IEnumerator SpinAndStop()
    {
        int panelCount= _simpleScrollSnap.NumberOfPanels;
        yield return new WaitForSeconds(0.5f);
        for(int i = 0; i < 100; i++)
        {
            _simpleScrollSnap.GoToNextPanel();
            yield return new WaitForSeconds(EaseOutCubic(i));
        }
        
    }

    private void Update()
    {
        
        if (Input.GetKeyUp(KeyCode.Q))
        {
            _simpleScrollSnap.GoToNextPanel();
        }
    }

    private float EaseOutCubic(float t)
    {
        Debug.Log($"{Mathf.Pow(t - 50, 2) / 250000}");
        return Mathf.Pow(t-50, 2)/25000+0.01f;
        
    }
}

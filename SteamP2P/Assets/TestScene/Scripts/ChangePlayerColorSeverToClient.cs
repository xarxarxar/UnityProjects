using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������߼����ǣ�
/// ������ͨ��������ͬ�����ͻ���
/*�������ϵ�ÿһ���������壬
ÿ����ı�һ��PlayerColor��ֵ��
Ȼ�������ͨ��SyncVar���ı�ǰ���ֵ���͸�ÿһ���ͻ��ˣ�
������ÿһ���ͻ��˶�Ӧ���������hook����Ҳ����ColorChanged(Color oldColor, Color newColor)������
��ֵ�ı������Ч�����õ��Լ��ĳ����С�*/
/// </summary>
public class ChangePlayerColorSeverToClient : NetworkBehaviour //������Ҫ�̳�NetworkBehaviour
{
    [SyncVar(hook = nameof(ColorChanged))]
    Color PlayerColor = Color.white;

    MaterialPropertyBlock prop;

    //ע��hook���������ڷ������ϵ���
    void ColorChanged(Color oldColor, Color newColor)
    {
        prop.SetColor("_Color", newColor);
        GetComponent<Renderer>().SetPropertyBlock(prop);
        Debug.Log("color changed hook");
    }

    private void Start()
    {
        prop = new MaterialPropertyBlock();
        //��������һ�� Renderer ����Ĳ������Եġ����д���ͨ�� MaterialPropertyBlock ���ı����Ĳ������ԣ�������Ӱ����ʵ�����ʵ����
        GetComponent<Renderer>().GetPropertyBlock(prop);
    }

    float duringTimer = 0f;
    private void Update()
    {
        //����ע�������һ��API isServer��
        //��˵��������δ���ֻ���ڷ����������У����ͻ����ǲ������е�
        if (isServer)
        {
            if (duringTimer < 2f)
            {
                duringTimer += Time.deltaTime;
            }
            else
            {
                duringTimer = 0f;
                PlayerColor = PlayerColor == Color.white ? Color.black : Color.white;
                Debug.Log("color changed sever");
            }
        }
    }

}

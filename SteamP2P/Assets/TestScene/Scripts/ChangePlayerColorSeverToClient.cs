using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 整体的逻辑就是：
/// 将变量通过服务器同步给客户端
/*服务器上的每一个主角物体，
每两秒改变一次PlayerColor的值，
然后服务器通过SyncVar将改变前后的值发送给每一个客户端，
接下来每一个客户端对应的物体调用hook函数也就是ColorChanged(Color oldColor, Color newColor)函数，
将值改变带来的效果作用到自己的场景中。*/
/// </summary>
public class ChangePlayerColorSeverToClient : NetworkBehaviour //这里需要继承NetworkBehaviour
{
    [SyncVar(hook = nameof(ColorChanged))]
    Color PlayerColor = Color.white;

    MaterialPropertyBlock prop;

    //注意hook函数不会在服务器上调用
    void ColorChanged(Color oldColor, Color newColor)
    {
        prop.SetColor("_Color", newColor);
        GetComponent<Renderer>().SetPropertyBlock(prop);
        Debug.Log("color changed hook");
    }

    private void Start()
    {
        prop = new MaterialPropertyBlock();
        //用于设置一个 Renderer 组件的材质属性的。这行代码通过 MaterialPropertyBlock 来改变对象的材质属性，而不会影响材质的其他实例。
        GetComponent<Renderer>().GetPropertyBlock(prop);
    }

    float duringTimer = 0f;
    private void Update()
    {
        //尤其注意代码中一个API isServer，
        //这说明下面这段代码只会在服务器上运行，而客户端是不会运行的
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

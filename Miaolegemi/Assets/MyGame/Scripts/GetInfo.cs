using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using WeChatWASM;

public class GetInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 异步函数，获取玩家数据
    public async void GetCardData(Action<UserData> successAction)
    {
        // 创建一个用于存储用户数据的变量
        UserData userData = null;

        // 调用云函数 "TestCloudFunction02" (假设这是你之前定义的获取玩家数据的云函数名称)
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "get-userdata",  // 替换为你获取玩家数据的云函数名称
            data = "获取玩家信息",  // 你可以根据需要传递参数

            success = (res) =>
            {
                Debug.Log("GetCardData success");

                // 解析从云函数返回的结果
                if (res.result != null)
                {
                    Debug.Log(res.result);
                    // 将 JSON 转换为 UserData 对象
                    userData = JsonUtility.FromJson<UserData>(res.result.ToString());

                    // 调用传入的成功回调函数，传递 userData
                    successAction?.Invoke(userData);
                }
            },
            fail = (res) =>
            {
                Debug.LogError("GetCardData failed: " + res.errMsg);
            },
            complete = (res) =>
            {
                Debug.Log("GetCardData complete");
            }
        });

        // 等待任务完成
        while (userData == null)
        {
            await Task.Yield();  // 等待帧结束，防止阻塞主线程
        }
    }
}

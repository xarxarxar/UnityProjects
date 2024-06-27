using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

/// <summary>
/// Lua管理器
/// 提供 lua解析器
/// 保证解析器的唯一性
/// </summary>
public class LuaMgr : BaseManager<LuaMgr>
{
    //执行Lua语言的函数
    //释放垃圾
    //销毁
    //重定向
    private LuaEnv luaEnv;


    /// <summary>
    /// 得到Lua中的_G
    /// </summary>
    public LuaTable Global
    {
        get
        {
            return luaEnv.Global;
        }
    }


    /// <summary>
    /// 初始化解析器
    /// </summary>
    public void Init()
    {
        //已经初始化了 别初始化 直接返回
        if (luaEnv != null)
            return;
        //初始化
        luaEnv = new LuaEnv();
        //加载lua脚本 重定向
        luaEnv.AddLoader(MyCustomLoader);
        luaEnv.AddLoader(MyCustomABLoader);
    }

    //自动执行
    private byte[] MyCustomLoader(ref string filePath)
    {
        //通过函数中的逻辑 去加载 Lua文件 
        //传入的参数 是 require执行的lua脚本文件名
        //拼接一个Lua文件所在路径
        string path = Application.dataPath + "/Lua/" + filePath + ".lua";

        //有路径 就去加载文件 
        //File知识点 C#提供的文件读写的类
        //判断文件是否存在
        if (File.Exists(path))
        {
            return File.ReadAllBytes(path);
        }
        else
        {
            Debug.Log("MyCustomLoader重定向失败，文件名为" + filePath);
        }


        return null;
    }


    //Lua脚本会放在AB包 
    //最终我们会通过加载AB包再加载其中的Lua脚本资源 来执行它
    //重定向加载AB包中的LUa脚本
    private byte[] MyCustomABLoader(ref string filePath)
    {
        //Debug.Log("进入AB包加载 重定向函数");
        ////从AB包中加载lua文件
        ////加载AB包
        //string path = Application.streamingAssetsPath + "/lua";
        //AssetBundle ab = AssetBundle.LoadFromFile(path);

        ////加载Lua文件 返回
        //TextAsset tx = ab.LoadAsset<TextAsset>(filePath + ".lua");
        ////加载Lua文件 byte数组
        //return tx.bytes;

        //通过我们的AB包管理器 加载的lua脚本资源
        TextAsset lua = ABMgr.GetInstance().LoadRes<TextAsset>("lua", filePath + ".lua");
        if (lua != null)
            return lua.bytes;
        else
            Debug.Log("MyCustomABLoader重定向失败，文件名为：" + filePath);

        return null;
    }


    /// <summary>
    /// 传入lua文件名 执行lua脚本
    /// </summary>
    /// <param name="fileName"></param>
    public void DoLuaFile(string fileName)
    {
        string str = string.Format("require('{0}')", fileName);
        DoString(str);
    }

    /// <summary>
    /// 执行Lua语言
    /// </summary>
    /// <param name="str"></param>
    public void DoString(string str)
    {
        if(luaEnv == null)
        {
            Debug.Log("解析器为初始化");
            return;
        }
        luaEnv.DoString(str);
    }

    /// <summary>
    /// 释放lua 垃圾
    /// </summary>
    public void Tick()
    {
        if (luaEnv == null)
        {
            Debug.Log("解析器为初始化");
            return;
        }
        luaEnv.Tick();
    }

    /// <summary>
    /// 销毁解析器
    /// </summary>
    public void Dispose()
    {
        if (luaEnv == null)
        {
            Debug.Log("解析器为初始化");
            return;
        }
        luaEnv.Dispose();
        luaEnv = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using XLua;

public class LuaManager : MonoBehaviour
{
    //public static LuaManager instance;
    public LuaEnv luaEnv;
    public string luaString;

    private void Awake()
    {
        //instance = this;
        luaEnv=new LuaEnv();
    }
}

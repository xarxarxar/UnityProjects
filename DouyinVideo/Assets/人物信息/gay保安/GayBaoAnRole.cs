using UnityEngine;

/// <summary>
/// 只会说给我一个接口的保安
/// </summary>
public class GayBaoAnRole : BaseRole
{
    public override void Big()
    {
        //什么也不用做
        Broadcast.instance.BroadCastNews($"{RoleName}使用了大招,说给给给给我一个接口",roleColor);
    }

    
}

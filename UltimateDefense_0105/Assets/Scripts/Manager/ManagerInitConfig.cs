using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManagerInitConfig", menuName = "Config/Manager Init Config")]
public class ManagerInitConfig : ScriptableObject
{
    [System.Serializable]
    public class ManagerGroup
    {
        [Tooltip("这一组中所有 Manager 将在同一顺序等级中 Init（可以并行）")]
        public List<ManagerBase> managers; // 需继承 ManagerBase
    }

    [Tooltip("按顺序定义 Manager 初始化的顺序（上面的先初始化）")]
    public List<ManagerGroup> initOrder = new List<ManagerGroup>();

    public List<ManagerBase> allManagers=new List<ManagerBase>();
}

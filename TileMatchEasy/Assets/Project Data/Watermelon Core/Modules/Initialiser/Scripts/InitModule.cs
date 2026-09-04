using UnityEngine;

namespace Watermelon
{
    // 初始化模块的抽象基类，继承自 ScriptableObject
    public abstract class InitModule : ScriptableObject
    {
        // 模块名称，隐藏在 Inspector 中
        [HideInInspector]
        [SerializeField]
        protected string moduleName;

        // 抽象方法，用于创建组件，必须由子类实现
        public abstract void CreateComponent(Initialiser initialiser);

        // 构造函数，默认模块名称为 "Default Module"
        public InitModule()
        {
            moduleName = "Default Module";
        }
    }
}

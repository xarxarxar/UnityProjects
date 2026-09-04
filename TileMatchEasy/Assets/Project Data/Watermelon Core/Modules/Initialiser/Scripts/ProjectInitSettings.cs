#pragma warning disable 0649

using UnityEngine;

namespace Watermelon
{
    // 在Unity编辑器中创建设置选项卡和菜单项
    [SetupTab("Init Settings", priority = 1, texture = "icon_puzzle")] // 设置初始化选项卡
    [CreateAssetMenu(fileName = "Project Init Settings", menuName = "Settings/Project Init Settings")] // 创建资产菜单项
    [HelpURL("https://docs.google.com/document/d/1ORNWkFMZ5_Cc-BUgu9Ds1DjMjR4ozMCyr6p_GGdyCZk")] // 帮助文档链接
    public class ProjectInitSettings : ScriptableObject // 继承自ScriptableObject，允许在Unity中创建可配置的对象
    {
        [SerializeField] InitModule[] coreModules; // 核心模块数组，序列化以在Inspector中显示
        public InitModule[] CoreModules => coreModules; // 公有属性，提供对核心模块的访问

        [SerializeField] InitModule[] modules; // 初始化模块数组，序列化以在Inspector中显示
        public InitModule[] Modules => modules; // 公有属性，提供对初始化模块的访问

        // 初始化方法，接受一个Initialiser实例作为参数
        public void Initialise(Initialiser initialiser)
        {
            // 遍历核心模块并创建相应的组件
            for (int i = 0; i < coreModules.Length; i++)
            {
                if (coreModules[i] != null) // 检查模块是否不为空
                {
                    coreModules[i].CreateComponent(initialiser); // 调用模块的创建组件方法
                }
            }

            // 遍历其他初始化模块并创建相应的组件
            for (int i = 0; i < modules.Length; i++)
            {
                if (modules[i] != null) // 检查模块是否不为空
                {
                    modules[i].CreateComponent(initialiser); // 调用模块的创建组件方法
                }
            }
        }
    }
}

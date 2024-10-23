#pragma warning disable 0649
#pragma warning disable 0414

using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Watermelon
{
    // 设置该脚本的执行顺序和帮助文档链接
    [DefaultExecutionOrder(-999)]
    [HelpURL("https://docs.google.com/document/d/1ORNWkFMZ5_Cc-BUgu9Ds1DjMjR4ozMCyr6p_GGdyCZk")]
    public class Initialiser : MonoBehaviour
    {
        // 序列化的字段，允许在Inspector面板中设置
        [SerializeField] ProjectInitSettings initSettings; // 项目初始化设置
        [SerializeField] Canvas systemCanvas; // 系统画布
        [SerializeField] EventSystem eventSystem; // 事件系统

        [Space]
        [SerializeField] ScreenSettings screenSettings; // 屏幕设置

        // 静态字段，用于全局访问
        public static Canvas SystemCanvas; // 全局系统画布
        public static GameObject InitialiserGameObject; // 当前初始化器的游戏对象

        // 静态属性，表示初始化状态
        public static bool IsInititalized { get; private set; } // 是否已初始化
        public static bool IsStartInitialized { get; private set; } // 是否已开始初始化
        public static ProjectInitSettings InitSettings { get; private set; } // 全局初始化设置

        // Awake方法，在对象激活时调用
        public void Awake()
        {
            screenSettings.Initialise(); // 初始化屏幕设置

            // 检查是否已初始化，确保只执行一次
            if (!IsInititalized)
            {
                IsInititalized = true;

                // 设置静态属性
                InitSettings = initSettings;
                SystemCanvas = systemCanvas;
                InitialiserGameObject = gameObject;

                // 根据条件添加相应的输入模块
#if MODULE_INPUT_SYSTEM
                eventSystem.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();
#endif

                DontDestroyOnLoad(gameObject); // 在场景切换时保持该对象存在

                initSettings.Initialise(this); // 调用初始化设置的方法
            }
        }

        // Start方法，在Awake之后调用
        public void Start()
        {
            Initialise(true); // 默认加载场景

            
        }

        // 初始化方法，根据参数决定加载方式
        public void Initialise(bool loadingScene)
        {
            if (!IsStartInitialized) // 检查是否已开始初始化
            {
                IsStartInitialized = true;

                // 根据参数加载场景或进行简单加载
                if (loadingScene)
                {
                    // 加载游戏场景
                    GameLoading.LoadGameScene(() => {
                        CallWechat.Init();//初始化微信调用
                        }); 
                    }
                else
                {
                    GameLoading.SimpleLoad(); // 进行简单加载
                }
            }
        }

        // 检查特定模块是否已初始化
        public static bool IsModuleInitialised(Type moduleType)
        {
            ProjectInitSettings projectInitSettings = InitSettings;

            InitModule[] coreModules = null; // 核心模块数组
            InitModule[] initModules = null; // 初始化模块数组

#if UNITY_EDITOR
            // 如果未初始化，尝试获取项目初始化设置
            if (!IsInititalized)
            {
                projectInitSettings = RuntimeEditorUtils.GetAssetByName<ProjectInitSettings>();
            }
#endif

            // 如果项目初始化设置不为空，获取核心和初始化模块
            if (projectInitSettings != null)
            {
                coreModules = projectInitSettings.CoreModules;
                initModules = projectInitSettings.Modules;
            }

            // 检查核心模块是否已初始化
            for (int i = 0; i < coreModules.Length; i++)
            {
                if (coreModules[i].GetType() == moduleType)
                {
                    return true; // 模块已初始化
                }
            }

            // 检查初始化模块是否已初始化
            for (int i = 0; i < initModules.Length; i++)
            {
                if (initModules[i].GetType() == moduleType)
                {
                    return true; // 模块已初始化
                }
            }

            return false; // 模块未初始化
        }

        // 当对象被销毁时调用
        private void OnDestroy()
        {
            IsInititalized = false; // 重置初始化状态

#if UNITY_EDITOR
            SaveController.Save(true); // 在编辑器中保存状态
#endif
        }

        // 应用程序获得或失去焦点时调用
        private void OnApplicationFocus(bool focus)
        {
#if !UNITY_EDITOR
            if (!focus) SaveController.Save(); // 在失去焦点时保存状态
#endif
        }
    }
}

// -----------------
// Initialiser v 0.4.4
// -----------------
// 更新日志
// v 0.4.4
// • 添加基于输入模块类型的事件系统初始化
// v 0.4.3
// • 修复了编辑器添加核心模块的错误
// v 0.4.2
// • 添加了加载场景的逻辑
// v 0.4.1
// • 修复了模块移除时的错误
// v 0.3.1
// • 添加了文档链接
// • 初始类名称更改为Initialiser
// • 修复了重新编译的问题
// v 0.2
// • 添加了排序功能
// • 在初始化后销毁Initializer MonoBehaviour
// v 0.1
// • 添加了基础版本

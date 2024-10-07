using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Watermelon
{
    [MethodDrawer(typeof(ButtonAttribute))]
    public class ButtonMethodDrawer : MethodDrawer
    {
        public override void DrawMethod(UnityEngine.Object target, MethodInfo methodInfo)
        {
            object[] attributes = methodInfo.GetCustomAttributes(typeof(ButtonAttribute), false);
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes != null)
                {
                    ButtonAttribute buttonAttribute = (ButtonAttribute)attributes[i];
                    if (!string.IsNullOrEmpty(buttonAttribute.VisibilityConditionName))
                    {
                        MethodInfo conditionMethod = target.GetType().GetMethod(buttonAttribute.VisibilityConditionName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (conditionMethod != null && conditionMethod.ReturnType == typeof(bool) && conditionMethod.GetParameters().Length == 0)
                        {
                            bool conditionValue = (bool)conditionMethod.Invoke(target, null);
                            if (buttonAttribute.VisibilityOption == ButtonVisibility.ShowIf)
                            {
                                if(!conditionValue)
                                {
                                    continue;
                                }
                            }
                            else if (buttonAttribute.VisibilityOption == ButtonVisibility.HideIf)
                            {
                                if(conditionValue)
                                {
                                    continue;
                                }
                            }
                        }

                        FieldInfo conditionField = target.GetType().GetField(buttonAttribute.VisibilityConditionName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (conditionField != null && conditionField.FieldType == typeof(bool))
                        {
                            bool conditionValue = (bool)conditionField.GetValue(target);
                            if (buttonAttribute.VisibilityOption == ButtonVisibility.ShowIf)
                            {
                                if (!conditionValue)
                                {
                                    continue;
                                }
                            }
                            else if (buttonAttribute.VisibilityOption == ButtonVisibility.HideIf)
                            {
                                if (conditionValue)
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    string buttonText = string.IsNullOrEmpty(buttonAttribute.Text) ? methodInfo.Name : buttonAttribute.Text;

                    if (GUILayout.Button(buttonText))
                    {
                        object[] attributeParams = buttonAttribute.Params;
                        if (!attributeParams.IsNullOrEmpty())
                        {
                            ParameterInfo[] methodParams = methodInfo.GetParameters();
                            if (attributeParams.Length == methodParams.Length)
                            {
                                bool allowInvoke = true;
                                for (int p = 0; p < attributeParams.Length; p++)
                                {
                                    if (attributeParams[p].GetType() != methodParams[p].ParameterType)
                                    {
                                        allowInvoke = false;

                                        Debug.LogWarning(string.Format("Invalid parameters are specified ({0})", buttonText), target);

                                        break;
                                    }
                                }

                                if (allowInvoke)
                                {
                                    object returnObject = methodInfo.Invoke(target, buttonAttribute.Params);

                                    if(returnObject != null)
                                        Debug.Log(returnObject);
                                }
                            }
                            else
                            {
                                Debug.LogWarning(string.Format("Invalid parameters are specified ({0})", buttonText), target);
                            }
                        }
                        else
                        {
                            object returnObject = methodInfo.Invoke(target, null);

                            if (returnObject != null)
                                Debug.Log(returnObject);
                        }
                    }
                }
            }
        }
    }
}

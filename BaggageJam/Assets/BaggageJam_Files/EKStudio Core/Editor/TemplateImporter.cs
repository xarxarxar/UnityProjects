using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor.PackageManager;

namespace EKStudio
{
    [InitializeOnLoad]
    public class TemplateImporter
    {
        private const string ImportKey = "TemplateImported";

        static TemplateImporter()
        {
            // Check if the asset has already been imported
            bool hasImported = EditorPrefs.GetBool(ImportKey, false);

            // If it hasn't been imported before, show the window
            if (!hasImported)
            {
                EditorApplication.delayCall += () =>
                {
                    if (!SessionState.GetBool("TemplateWindowShown", false))
                    {
                        SessionState.SetBool("TemplateWindowShown", true);
                        WelcomeWindow.ShowWindow();
                    }
                };

                // Mark as imported
                EditorPrefs.SetBool(ImportKey, true);
            }
        }
    }

    public class WelcomeWindow : EditorWindow
    {
        private static readonly JObject RegistryEntry = new JObject
    {
        { "name", "package.openupm.com" },
        { "url", "https://package.openupm.com" },
        { "scopes", new JArray { "com.coffee.ui-particle" } }
    };

        private Texture2D checkIcon;
        private bool isRegistryAdded;

        [MenuItem("Tools/EKStudio Template Setup")]
        public static void ShowWindow()
        {
            WelcomeWindow window = GetWindow<WelcomeWindow>("Setup Guide");

            // Set a fixed window position at the center of the screen
            window.position = new Rect(
                (Screen.currentResolution.width - 450) / 2,
                (Screen.currentResolution.height - 480) / 2,
                450, 480
            );

            window.minSize = new Vector2(470, 300);
            window.CheckRegistryStatus();
        }

        private void OnEnable()
        {
            checkIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/BaggageJam_Files/EKStudio Core/Editor/check.png");
            CheckRegistryStatus();
            minSize = new Vector2(450, 480);
            maxSize = new Vector2(450, 480);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            // Title
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label("Welcome to the Game Template Setup!", titleStyle);

            GUILayout.Space(5);

            // Description
            GUIStyle descStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
            };
            GUILayout.Label("To make it work properly, click the Install buttons below.", descStyle);

            GUILayout.Space(20);

            // Draw Horizontal Line
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));

            GUILayout.Space(10);

            // Install Button with Check Image
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (isRegistryAdded && checkIcon != null)
            {
                GUILayout.Label(checkIcon, GUILayout.Width(46), GUILayout.Height(46)); // Check image
            }

            if (GUILayout.Button("Install UI Particle Package", ButtonStyle, GUILayout.Width(250), GUILayout.Height(40)))
            {
                SetupScopedRegistry();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Draw Horizontal Line
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
            GUILayout.Space(10);

            // Template Documentation Button


            GUILayout.Space(10);

            // Close Button (Fixed at the Bottom)
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Game Template Documentation", ButtonStyle, GUILayout.Width(250), GUILayout.Height(40)))
            {
                Application.OpenURL("https://bouncy-lemur-ac3.notion.site/Baggage-Jam-Game-Template-192cc6421bf2802fa386d4b3fd702a6f");
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUIStyle closeButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedWidth = 120,
                fixedHeight = 40
            };

            if (GUILayout.Button("Close", closeButtonStyle))
            {
                Close();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        private void CheckRegistryStatus()
        {
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");

            if (!File.Exists(manifestPath))
            {
                isRegistryAdded = false;
                return;
            }

            string manifestContent = File.ReadAllText(manifestPath);
            JObject manifestJson = JObject.Parse(manifestContent);

            if (!manifestJson.ContainsKey("scopedRegistries"))
            {
                isRegistryAdded = false;
                return;
            }

            JArray scopedRegistries = (JArray)manifestJson["scopedRegistries"];
            isRegistryAdded = scopedRegistries.Any(r => r["url"]?.ToString() == "https://package.openupm.com");
        }

        private void SetupScopedRegistry()
        {
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");

            if (!File.Exists(manifestPath))
            {
                EditorUtility.DisplayDialog("Error", "Could not find manifest.json", "OK");
                return;
            }

            string manifestContent = File.ReadAllText(manifestPath);
            JObject manifestJson = JObject.Parse(manifestContent);

            if (!manifestJson.ContainsKey("scopedRegistries"))
            {
                manifestJson["scopedRegistries"] = new JArray();
            }

            JArray scopedRegistries = (JArray)manifestJson["scopedRegistries"];
            bool registryExists = scopedRegistries.Any(r => r["url"]?.ToString() == "https://package.openupm.com");

            if (!registryExists)
            {
                scopedRegistries.Add(RegistryEntry);
                File.WriteAllText(manifestPath, manifestJson.ToString());
                AssetDatabase.Refresh(); // **AssetDatabase'ı yenile**
                Client.Resolve(); // **Package Manager'ı yeniden başlat**
                isRegistryAdded = true;
                EditorUtility.DisplayDialog("Success", "Scoped Registry has been added!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Info", "Scoped Registry already exists.", "OK");
            }
        }
    }

}

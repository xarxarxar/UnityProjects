using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using System;
using  UltimateSlider;


[CustomEditor(typeof(SliderManager))]
public class SliderManagerEditor : Editor
{
    #region Edior Variables
    SliderManager myTarget;
    int selectedAction = 0;
    int selectedActionAnimations = 0;
//    int selectedAction2 = 0;
    GUIStyle style = null;
    GUIStyle style2 = null;
    GUIStyle style3 = null;
    GUIStyle style4 = null;
//    float lastEditorUpdateTime;
    bool showChild;
    bool showGeneralEvents;
    bool ShowSlideEvents;
    bool ShowSpecialEvents;
    float timer;
    int counter;
    #endregion

    #region Main Editor Functions
    private void OnEnable()
    {
      //  if (EditorApplication.isPlaying)
      //      return;

        myTarget = (SliderManager)target;

        if (!EditorApplication.isPlaying)
        { 
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();

//        lastEditorUpdateTime = Time.realtimeSinceStartup;
        EditorApplication.update += OnEditorUpdate;
        }
    }

    private void OnDisable()
    {
 
        EditorApplication.update -= OnEditorUpdate;
        GUIStyle newstyle = new GUIStyle();
        EditorStyles.label.font = newstyle.font;
        EditorStyles.label.fontSize = newstyle.fontSize;
        EditorStyles.helpBox.font = newstyle.font;
        EditorStyles.helpBox.fontSize = newstyle.fontSize;
        style2 = null;
        style = null;
        style3 = null;
        style4 = null;
    }

    private string[] guids2 = null;
    private Texture2D texx;
    private string[] guids3;
    private Font fonti1;
    private string[] guids4;
    private Font fonti2;
    private string[] guids5;
    private Font fonti3;
    private string[] guids6;
    private Font fonti4;
    private string[] guids7;
    private Font fonti5;
    
    private string[] guidsText1;
    private string[] guidsText2;
    private Texture2D texx1;
    private Texture2D texx2;
    private int lastCounter = -1;
    public override void OnInspectorGUI()
    {

        #region Pre-Setup
        Color DefaultBackGroundColor = GUI.backgroundColor;
        Color DefaultContentColor = GUI.contentColor;
        var rect = EditorGUILayout.BeginVertical();

        if (EditorGUIUtility.isProSkin)
        {
            if(guids2 == null)
             guids2 = AssetDatabase.FindAssets("DarkThemSITPSD t:texture2D");

            if(texx == null)
             texx = (Texture2D) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids2[0]), typeof(Texture2D));
             
            if(texx != null)
             EditorGUI.DrawTextureTransparent(new Rect(rect.x - 17, rect.y + 1, rect.width + 25f, rect.height + 5000),texx);
        }
        else
        {
            EditorGUI.DrawRect(new Rect(rect.x - 4, rect.y + 1, rect.width + 2.5f, rect.height + 5000),
            Color.gray);
        }
        EditorGUILayout.EndVertical();
        this.serializedObject.Update();
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.button);

            if(guids3 == null)
             guids3 = AssetDatabase.FindAssets("sliderFont t:Font");

            if(fonti1 == null)
             fonti1 = (Font) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids3[0]), typeof(Font));
             
            if (fonti1 != null)
            {
                style.font = fonti1;
                style.fontSize = 25;
                style.fontStyle = FontStyle.Bold;

            }
           
            // style.normal.background = MakeTex(2,2, Color.cyan);
            style.fixedWidth = 110;
            style.fixedHeight = 70;
            style.stretchHeight = true;
            style.stretchWidth = true;
        }
        if (style2 == null)
        {
            style2 = new GUIStyle(GUI.skin.label);
            
            if(guids4 == null)
             guids4 = AssetDatabase.FindAssets("sliderFont2 t:Font");

            if(fonti2 == null)
            fonti2 = (Font) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids4[0]), typeof(Font));
            
             if (fonti2 != null)
            {
                style2.font = fonti2;
                style2.fontSize = 25;
                style2.fontStyle = FontStyle.Bold;

            }
             
            style2.fixedWidth = 110;
            style2.fixedHeight = 70;
            style2.stretchHeight = true;
            style2.stretchWidth = true;
        }
        if (style3 == null)
        {
            style3 = new GUIStyle(GUI.skin.button);

            if(guids5 == null)
            guids5 = AssetDatabase.FindAssets("sliderFont2 t:Font");

            fonti3 = (Font) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids5[0]), typeof(Font));
            
            if (fonti3 != null)
            {
                style3.font = fonti3;
                style3.fontSize = 18;
                style3.fontStyle = FontStyle.Bold;
            }
          
            style3.fixedWidth = 110;
            style3.fixedHeight = 70;
            style3.stretchHeight = true;
            style3.stretchWidth = true;
        }
        if (style4 == null)
        {
            style4 = new GUIStyle(GUI.skin.button);

            if(guids6 == null)
             guids6  = AssetDatabase.FindAssets("sliderFont2 t:Font");

            if(fonti4 == null)
            fonti4 = (Font) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids6[0]), typeof(Font));
            
            if (fonti4 != null)
            {
                style4.font = fonti4;
                style4.fontSize = 20;
                style4.fontStyle = FontStyle.Bold;
            }
          
            style4.fixedWidth = 90;
            style4.fixedHeight = 50;
            style4.stretchHeight = true;
            style4.stretchWidth = true;
        }
        if(guids7 == null)
         guids7 = AssetDatabase.FindAssets("sliderFont2 t:Font");
        if(fonti5 == null)
         fonti5 = (Font) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids7[0]), typeof(Font));
        if (fonti5 != null)
        {
            EditorStyles.label.font = style2.font;
            EditorStyles.label.fontSize = 15;
            EditorStyles.helpBox.font = style2.font;
            EditorStyles.helpBox.fontSize = 13;
        }
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        if (lastCounter != counter)
        {
            lastCounter = counter;
            guidsText1 = AssetDatabase.FindAssets("iconMain t:Texture2D");
            texx1 = (Texture2D) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guidsText1[0]), typeof(Texture2D));
            string thisName = "icontipsd" + counter + " t:Texture2D";
            guidsText2 = AssetDatabase.FindAssets(thisName);
            texx2 = (Texture2D) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guidsText2[0]), typeof(Texture2D));

        }
        
        if(texx1 != null)
        GUILayout.Label(texx1);
        if (texx2 != null)
            GUILayout.Label(texx2);
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal(); 
   

        selectedAction = EditorPrefs.GetInt("SelectedAction", 0);
        selectedActionAnimations = EditorPrefs.GetInt("selectedActionAnimations", 0);
        showGeneralEvents = EditorPrefs.GetInt("showGeneralEvents", 0) == 0 ? false : true;
        ShowSlideEvents = EditorPrefs.GetInt("ShowSlideEvents", 0) == 0 ? false : true;
        ShowSpecialEvents = EditorPrefs.GetInt("ShowSpecialEvents", 0) == 0 ? false : true;
        showChild = EditorPrefs.GetInt("showChild", 0) == 0 ? false:true;
        #endregion

        #region Validation Check 
        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if(myTarget.enabled == false)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Please stop the game and Make changes that been shown in the Console !", MessageType.Error);
                EditorGUILayout.Space();

                GUI.color = Color.yellow;
                if(GUILayout.Button("Stop Playing The Game"))
                {
                  EditorApplication.isPlaying = false;
                }
                GUI.color = Color.white;
                return;
            }

            if (myTarget.sliderParent != null && myTarget.sliderParent.gameObject.activeInHierarchy == false)
            {
                EditorGUILayout.Space();

                EditorGUILayout.HelpBox("Slider Parent Object is not Active", MessageType.Error);
                EditorGUILayout.Space();
                GUI.color = Color.yellow;
                if (GUILayout.Button("Active Parent Object"))
                {
                    myTarget.sliderParent.gameObject.SetActive(true);
                }
                GUI.color = Color.white;
                EditorGUILayout.Space();
                EditorGUILayout.ObjectField("Slider Parent Object", myTarget.sliderParent, typeof(Transform) , true);
            }
            // show statistics
            CreateLineSimple();
            EditorGUILayout.LabelField("Slides Amount : " + myTarget.sliderPagesCount);
            CreateLineSimple();
 
                EditorGUILayout.LabelField("Current Slide Number : " + myTarget.currentSliderNumber);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField("Current slide Object", myTarget.objectsofSlides[myTarget.currentSliderNumber], typeof(Transform) , true);
         
            EditorGUILayout.EndHorizontal();
            CreateLineSimple();
            EditorGUILayout.LabelField("Is Sliding : " + (myTarget.sliding  ? "true" : "false"));
            CreateLineSimple();
            EditorGUILayout.LabelField("Movement Count : " + myTarget.movemetnCount);
            CreateLineSimple();
            EditorGUILayout.LabelField("Time Passed On Sliding : " + (int)(myTarget.totalTimeSliding));
            CreateLineSimple();
            EditorGUILayout.LabelField("Slide Control Type : " +  myTarget.controlType.ToString());
            CreateLineSimple();
            EditorGUILayout.HelpBox("Cant Change Settings When the Game Is Runing !", MessageType.Info);
            EditorGUILayout.Space();
             
            return;
        }

        if (myTarget.sliderParent == null)
        {

            myTarget.parentsofSlides.Clear();
            myTarget.objectsofSlides.Clear();

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Please assign a slider Content parent !", MessageType.Error);
            EditorGUILayout.Space();
 
            GUI.color = Color.yellow;
            GUIContent content2 = new GUIContent("Slider Content Parent", "Assign the GameObject you want to be as your slider parent here.\nThe GameObject muste have at least two child objects. ");
            myTarget.sliderParent = (Transform)EditorGUILayout.ObjectField(content2, myTarget.sliderParent, typeof(Transform), true);
             
            EditorGUILayout.Space();
            return;
        
        }
       

        if (myTarget.sliderParent.childCount < 2)
        {
            myTarget.parentsofSlides.Clear();
            myTarget.objectsofSlides.Clear();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Slider parent must have at least two child objects ! Current is : " + myTarget.sliderParent.childCount, MessageType.Warning);
            EditorGUILayout.Space();
            GUI.color = Color.yellow;
            var serializedobject = new SerializedObject(target);
            var property = serializedobject.FindProperty("sliderParent");
            serializedobject.Update();
            EditorGUILayout.PropertyField(property, true);
            serializedobject.ApplyModifiedProperties();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            return;
        
        }
        else
        {
            GUI.color = new Color(123, 255, 11, 255);// Color.green;

            if(myTarget.slideEventClass.Count != myTarget.sliderPagesCount)
                checkSlidesEventList();

             
        }

        GUI.color = Color.white;

        #endregion

        #region  Tabs

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        string[] ActionLabels = new string[] { "Main", "Movements", "Controllers", "Events",  "Animations" , "Utility" };
        //string[] ActionLabelsAnimations = new string[] { "Focused", "Previous", "Next"};

   

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); 
        GUI.backgroundColor = Color.green;
        GUI.contentColor = Color.cyan;
        selectedAction = GUILayout.SelectionGrid(selectedAction, ActionLabels, 3 , style);
        GUI.backgroundColor = DefaultBackGroundColor;
        GUI.contentColor = DefaultContentColor;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        #endregion
 
        #region Main
        if (selectedAction == 0)
        {
       
            EditorGUILayout.Space();
         
            EditorGUILayout.HelpBox("You can change the main Content Parent of the slider here at any time !" , MessageType.Info );
            EditorGUILayout.Space();
             
            GUIContent content2 = new GUIContent("Slider Content Parent", "Assign the GameObject you want to be as your slider parent here.\nThe GameObject muste have at least two child objects. ");
            myTarget.sliderParent = (Transform)EditorGUILayout.ObjectField(content2, myTarget.sliderParent, typeof(Transform), true);
            EditorGUILayout.Space();
          
            if (myTarget.sliderParent == null)
                return;

            EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Total Childs : " + myTarget.sliderParent.childCount, MessageType.None);
                EditorGUILayout.Space();
              
            showChild = EditorGUILayout.Toggle("Show Childs" , showChild);

            if (showChild)
            {
                EditorPrefs.SetInt("showChild", 1);

                for (int i = 0; i < myTarget.sliderParent.childCount; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUIContent content3 = new GUIContent("Child " + i, "your number " + i + " child object of the parent");
                    EditorGUILayout.ObjectField(content3, myTarget.sliderParent.GetChild(i), typeof(Transform), true);
                    if(myTarget.startSliderNumber == i)
                    {
                        GUI.color = Color.magenta;
                    }
                    if(GUILayout.Button("Start Sliding from this child"))
                    {
                        myTarget.startSliderNumber = i;
                    }
                    EditorGUILayout.EndHorizontal();

                    GUI.color = Color.white;
                }


                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("if u Enable save function , this selection will be Ignored !", MessageType.Info);
                EditorGUILayout.Space();
            }
            else
            {
                EditorPrefs.SetInt("showChild", 0);
            }



        }
        else if (selectedAction == 1)
        {

            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUIContent content3 = new GUIContent("Slider Direction", "Set your space and position between slides");
            myTarget.sliderMoveRange = EditorGUILayout.Vector3Field(content3, myTarget.sliderMoveRange);
 
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            EditorGUILayout.Space();

            var serializedobject = new SerializedObject(target);
            var property = serializedobject.FindProperty("movementType");
            serializedobject.Update();
            EditorGUILayout.PropertyField(property, true);
            serializedobject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            if (myTarget.movementType != MovementType.Immediate)
            {
                GUIContent content10 = new GUIContent("Slider Speed ", "Set the speed of slides movement animation");
                myTarget.speed = EditorGUILayout.FloatField(content10, myTarget.speed);
                if (myTarget.speed < 0.1f)
                {
                    myTarget.speed = 0.1f;
                }
                EditorGUILayout.Space();
               // GUIContent content11 = new GUIContent("Delay ", "Set the delay before starting the slide movement animation");
              //  myTarget.delay = EditorGUILayout.FloatField(content11, myTarget.delay);
                EditorGUILayout.Space();

                GUIContent content14 = new GUIContent("Ignore Time Scale ", "if enabled the animation works even when the time scale is set to zero");
                myTarget.ignoreTimeScale = EditorGUILayout.Toggle(content14, myTarget.ignoreTimeScale);
                EditorGUILayout.Space();
            }



            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            EditorGUILayout.Space();


            GUIContent content12 = new GUIContent("On End Slider Do", "what should slider do when it reaches the last slide ");
            myTarget.endSliderChangeType = (EndPageChangeType)EditorGUILayout.EnumPopup(content12, myTarget.endSliderChangeType);
            EditorGUILayout.Space();

            if(myTarget.endSliderChangeType == EndPageChangeType.Reset)
            {
                GUIContent content122 = new GUIContent("Reset Speed Multiple", "how fast the slider should reset ? 1 means no effect");
                myTarget.resetSpeedScaleFactor = EditorGUILayout.FloatField(content122, myTarget.resetSpeedScaleFactor);
                if (myTarget.resetSpeedScaleFactor < 0)
                {
                    myTarget.resetSpeedScaleFactor = 0;
                }
            }

            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            EditorGUILayout.Space();
            


        }
        else if (selectedAction == 2)
        {
            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            EditorGUILayout.Space();
            GUIContent content3 = new GUIContent("Control type ", "Set how do u want to change your slides");
            myTarget.controlType = (ControlType)EditorGUILayout.EnumPopup(content3, myTarget.controlType);
            EditorGUILayout.Space();

            if (myTarget.controlType == ControlType.Automatically)
            {
                EditorGUILayout.Space();
                GUIContent contente3 = new GUIContent("Type", "how the slider should move automatically");
                myTarget.automaticType = (AutomaticType)EditorGUILayout.EnumPopup(contente3, myTarget.automaticType);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                GUIContent contente4 = new GUIContent("Waiting Time", "how much time slider should wait to go to the next slide automatically");
                myTarget.automaticTimeToChangeSlide =  EditorGUILayout.Vector2Field(contente4, myTarget.automaticTimeToChangeSlide);
                if (myTarget.automaticTimeToChangeSlide.x < 0)
                    myTarget.automaticTimeToChangeSlide.x = 0;
                if (myTarget.automaticTimeToChangeSlide.y < 0)
                    myTarget.automaticTimeToChangeSlide.y = 0;
                EditorGUILayout.Space();
                EditorGUILayout.Space();

            }
            else if (myTarget.controlType == ControlType.No_Controllers)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("When there are no controllers set u can still move the slider with swipe on the screen if u like !", MessageType.Info);
                EditorGUILayout.Space();
            }
            else if (myTarget.controlType == ControlType.Arrows)
            {
                EditorGUILayout.Space();
                if (myTarget.negativeArrowObject == null || myTarget.positiveArrowObject == null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("You need to assign two ui images to be your arrows for this controller !", MessageType.Error);
                    EditorGUILayout.Space();
                    if (myTarget.positiveArrowObject == null)
                        GUI.color = Color.yellow;
                    else
                        GUI.color = Color.green;

                    GUIContent content5 = new GUIContent("Positive Arrow", "Set the Positive Slider Arrow with an Image Component");
                    myTarget.positiveArrowObject = (Image)EditorGUILayout.ObjectField(content5, myTarget.positiveArrowObject, typeof(Image), true);
                    if(myTarget.positiveArrowObject == myTarget.negativeArrowObject)
                    {
                        myTarget.positiveArrowObject = null;
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.Space();
                    if (myTarget.negativeArrowObject == null)
                        GUI.color = Color.yellow;
                    else GUI.color = Color.green;

                    GUIContent content6 = new GUIContent("Negative Arrow", "Set the Negative Slider Arrow with an Image Component");
                    myTarget.negativeArrowObject = (Image)EditorGUILayout.ObjectField(content6, myTarget.negativeArrowObject, typeof(Image), true);
                    if (myTarget.negativeArrowObject == myTarget.positiveArrowObject)
                    {
                        myTarget.negativeArrowObject = null;
                    }
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = Color.green;
                    EditorGUILayout.Space();
                    GUIContent content5 = new GUIContent("Positive Arrow", "you can change the arrow at any time");
                    myTarget.positiveArrowObject = (Image)EditorGUILayout.ObjectField(content5, myTarget.positiveArrowObject, typeof(Image), true);
                    EditorGUILayout.Space();
                    GUIContent content6 = new GUIContent("Negative Arrow", "you can change the arrow at any time");
                    myTarget.negativeArrowObject = (Image)EditorGUILayout.ObjectField(content6, myTarget.negativeArrowObject, typeof(Image), true);
                    GUI.color = Color.white;
                }

            }
            else if (myTarget.controlType == ControlType.KeyBoard)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Only Letter Or Digit can be set to be used as key controllers !", MessageType.Info);
                EditorGUILayout.Space();

                EditorGUILayout.Space();
                if (myTarget.positiveKey == System.String.Empty || myTarget.negativeKey == System.String.Empty)
                {
                    GUI.color = Color.yellow;
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("You need to assign two keyboard keys to work as your slider controller ! \n\nThese keys must be Letter Or Digit and cant be the Same !", MessageType.Error);
                    EditorGUILayout.Space();
                    GUI.color = Color.white;

                    if (myTarget.positiveKey == System.String.Empty)
                        GUI.color = Color.yellow;
                    else
                        GUI.color = Color.green;

                    GUIContent content5 = new GUIContent("Positive Key", "Set the Positive Keyboard Key");
                    myTarget.positiveKey =  EditorGUILayout.TextField(content5, myTarget.positiveKey);
                    if (myTarget.positiveKey == myTarget.negativeKey)
                    {
                        myTarget.positiveKey = System.String.Empty;
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.Space();

                    if (myTarget.negativeKey == System.String.Empty)
                        GUI.color = Color.yellow;
                    else GUI.color = Color.green;

                    GUIContent content6 = new GUIContent("Negative Key", "Set the Negative Keyboard Key");
                    myTarget.negativeKey = EditorGUILayout.TextField(content6, myTarget.negativeKey);
                    if (myTarget.negativeKey == myTarget.positiveKey)
                    {
                        myTarget.negativeKey = System.String.Empty;
                    }
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = Color.green;
                    EditorGUILayout.Space();
                    GUIContent content5 = new GUIContent("Positive Key", "you can change the Positive Keyboard Key at any time");
                    myTarget.positiveKey = EditorGUILayout.TextField(content5, myTarget.positiveKey);
                    if(myTarget.positiveKey == myTarget.negativeKey)
                    {
                        myTarget.positiveKey = System.String.Empty;
                    }

                    EditorGUILayout.Space();
                    GUIContent content6 = new GUIContent("Negative Key", "you can change the Negative Keyboard Key at any time");
                    myTarget.negativeKey = EditorGUILayout.TextField(content6, myTarget.negativeKey);
                    if (myTarget.negativeKey == myTarget.positiveKey)
                    {
                        myTarget.negativeKey = System.String.Empty;
                    }
                    GUI.color = Color.white;
                }

 
                if (!myTarget.positiveKey.All(c => Char.IsLetterOrDigit(c)))
                {
                    myTarget.positiveKey = System.String.Empty;
                }


                if (!myTarget.negativeKey.All(c => Char.IsLetterOrDigit(c)))
                {
                    myTarget.positiveKey = System.String.Empty;
                }

                myTarget.positiveKey = myTarget.positiveKey.ToLower();
                myTarget.negativeKey = myTarget.negativeKey.ToLower();


                if (myTarget.positiveKey.Length > 1)
                {
                    myTarget.positiveKey = myTarget.positiveKey.Remove(0);
                }


                if (myTarget.negativeKey.Length > 1)
                {
                    myTarget.negativeKey = myTarget.negativeKey.Remove(0);
                }


            }
            else if (myTarget.controlType == ControlType.Buttons)
            {

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                while (myTarget.buttonList.Count < myTarget.sliderPagesCount)
                {
                    myTarget.buttonList.Add(null);
                }

                while (myTarget.buttonList.Count > myTarget.sliderPagesCount)
                {
                    myTarget.buttonList.RemoveAt(myTarget.buttonList.Count - 1);
                }

                bool allButtonsFull = true;
                for (int i = 0; i < myTarget.buttonList.Count; i++)
                {
                    if (myTarget.buttonList[i] == null)
                    {
                        allButtonsFull = false;
                    }
                }

                // remove duplicated elemetns ( not null ones)
                myTarget.buttonList = RemoveDuplicatesIterative(myTarget.buttonList);


                if (!allButtonsFull)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("You need to assign " + myTarget.sliderPagesCount + " buttons to the top list to be able to switch to any one of your slides", MessageType.Error);
                    EditorGUILayout.Space();
                    GUI.color = Color.yellow;
                }
                else
                {
                    GUI.color = Color.green;

                }


                var serializedobject = new SerializedObject(target);
                var property = serializedobject.FindProperty("buttonList");
                serializedobject.Update();
                EditorGUILayout.PropertyField(property, true);
                serializedobject.ApplyModifiedProperties();
                GUI.color = Color.white;

            }
            else if (myTarget.controlType == ControlType.Scroll)
            {

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (myTarget.scrollBar == null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("You need to assign a Scroll Bar to be able to change your slides", MessageType.Error);
                    EditorGUILayout.Space();
                    GUI.color = Color.yellow;
                }
                else
                {
                    GUI.color = Color.green;

                }


                EditorGUILayout.Space();
                EditorGUILayout.Space();

                var serializedobject = new SerializedObject(target);
                var property = serializedobject.FindProperty("scrollBar");
                serializedobject.Update();
                EditorGUILayout.PropertyField(property, true);
                serializedobject.ApplyModifiedProperties();

                GUI.color = Color.white;

            }



            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            GUIContent content4 = new GUIContent("Use Swipe", "Enable if u like to use Swipe as a second control");
            myTarget.swipeEnabled = EditorGUILayout.Toggle(content4, myTarget.swipeEnabled);

            if (myTarget.swipeEnabled)
            {
                EditorGUILayout.Space();
                GUIContent content6 = new GUIContent("Swipe Distance Need", "Set How Much Drag Needed to start sliding when u are swiping on the screen");
                myTarget.dragDistanceNeed = EditorGUILayout.FloatField(content6, myTarget.dragDistanceNeed);
                if (myTarget.dragDistanceNeed < 0)
                {
                    myTarget.dragDistanceNeed = 0;
                }
                EditorGUILayout.Space();

                EditorGUILayout.Space();
                GUIContent content7 = new GUIContent("Swipe Time Limit", "if the touch or click stay more then this time the swipe will be canceled ");
                myTarget.sliderMoveTimeNeed = EditorGUILayout.FloatField(content7, myTarget.sliderMoveTimeNeed);
                if (myTarget.sliderMoveTimeNeed < 0)
                {
                    myTarget.sliderMoveTimeNeed = 0;
                }
                EditorGUILayout.Space();

            }
 
            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            EditorGUILayout.Space();

        }
        else if (selectedAction == 3)
        {
       
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;

            showGeneralEvents = EditorGUILayout.Toggle("show Constant Events ", showGeneralEvents);

            if (showGeneralEvents)
            {
                EditorPrefs.SetInt("showGeneralEvents", 1);

                EditorGUILayout.Space();
                GUI.backgroundColor = DefaultBackGroundColor;
                EditorGUILayout.HelpBox("General Events : These events happens every time when the slider is moving", MessageType.None);
                EditorGUILayout.Space();
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                this.serializedObject.Update();
                GUI.backgroundColor = Color.yellow;
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("OnStartSliding"), true);
                this.serializedObject.ApplyModifiedProperties();

                this.serializedObject.Update();
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("OnUpdateSliding"), true);
                this.serializedObject.ApplyModifiedProperties();


                this.serializedObject.Update();
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("OnEndSliding"), true);
                this.serializedObject.ApplyModifiedProperties();
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.Space();

                GUI.backgroundColor = DefaultBackGroundColor;
            }
            else
            {
                EditorPrefs.SetInt("showGeneralEvents", 0);
            }

            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            EditorGUILayout.Space();

            ShowSlideEvents = EditorGUILayout.Toggle("Show Slides Events", ShowSlideEvents);

            if (ShowSlideEvents)
            {
                checkSlidesEventList();
                EditorPrefs.SetInt("ShowSlideEvents", 1);
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Slide Events : For each slide u can assign Special events", MessageType.None);
                EditorGUILayout.Space();
                GUI.backgroundColor = Color.yellow;

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                this.serializedObject.Update();
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("slideEventClass"), true);
                this.serializedObject.ApplyModifiedProperties();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                GUI.backgroundColor = DefaultBackGroundColor;
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            else
            {
                EditorPrefs.SetInt("ShowSlideEvents", 0);
            }


            GUI.color = Color.cyan;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUI.color = Color.white;

            ShowSpecialEvents = EditorGUILayout.Toggle("Show Special Events", ShowSpecialEvents);

            if (ShowSpecialEvents)
            {

                EditorPrefs.SetInt("ShowSpecialEvents", 1);
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Special Events : These Events happens in special times", MessageType.None);
                EditorGUILayout.Space();
                GUI.backgroundColor = Color.yellow;
                this.serializedObject.Update();
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("OnSliderFirstSlide"), true);
                this.serializedObject.ApplyModifiedProperties();

                this.serializedObject.Update();
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("OnSliderLastSlide"), true);
                this.serializedObject.ApplyModifiedProperties();
           
                GUI.backgroundColor = DefaultBackGroundColor;
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            else
            {
                EditorPrefs.SetInt("ShowSpecialEvents", 0);
            }


           
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            EditorGUILayout.Space();
        }
        else if (selectedAction == 4)
        {
            // GUILayout.BeginHorizontal();
            // GUILayout.FlexibleSpace();
            // GUI.backgroundColor = Color.magenta;
            // GUI.contentColor = Color.cyan;
            // selectedActionAnimations = GUILayout.SelectionGrid(selectedActionAnimations, ActionLabelsAnimations, 3 , style4);
            // GUI.backgroundColor = DefaultBackGroundColor;
            // GUI.contentColor = DefaultContentColor;
            // GUILayout.FlexibleSpace();
            // GUILayout.EndHorizontal();
            
            CreateLineSimple();

            if (selectedActionAnimations == 0)
            { 
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Animation Model for the slider that is currently on Focuse", MessageType.None);
                EditorGUILayout.Space();
            
            GUIContent content1 = new GUIContent("Focused Slider Animation", "select the animation effect u want for the focused slider");
            myTarget.animationSelection =   (animationSelection)EditorGUILayout.EnumPopup(content1, myTarget.animationSelection);
            EditorGUILayout.Space();
            if(myTarget.animationSelection != animationSelection.None)
            { 
                GUIContent content4 = new GUIContent("Direction", "set the target amount u want the object to reach");
                myTarget.animationDirection =  EditorGUILayout.Vector3Field(content4, myTarget.animationDirection);
                EditorGUILayout.Space();
                GUIContent content2 = new GUIContent("Type", "select the Effect Type");
                myTarget.animationType = (AnimationType)EditorGUILayout.EnumPopup(content2, myTarget.animationType);
                EditorGUILayout.Space();
                GUIContent content3 = new GUIContent("occurrence", "when the animation should happen");
                myTarget.animationTime = (animationTime)EditorGUILayout.EnumPopup(content3, myTarget.animationTime);
                EditorGUILayout.Space();


                if (myTarget.animationTime == animationTime.Always)
                {

                    GUIContent content6 = new GUIContent("format", "the format of playing the animation forever");
                    myTarget.animationAlwaysTimeFormat = (animationAlwaysTimeFormat)EditorGUILayout.EnumPopup(content6, myTarget.animationAlwaysTimeFormat);


                }
                else if (myTarget.animationTime == animationTime.Specified)
                {
          

                    myTarget.selectedCurrentValueOfSliding = 
                        EditorGUILayout.Slider(  myTarget.selectedCurrentValueOfSliding ,0f, 1f );

                    EditorGUILayout.HelpBox("Specifie Happening Time :  \n 0 means when a slider starts moving  , " +
                                            " \n 1 means the end of the movement , \n 0.5 means the middle of the movement ," +
                                            " \n you can set the value at any percent and the animation will happen in time related to that.", MessageType.None);
              
                }


                EditorGUILayout.Space();
                GUIContent content77 = new GUIContent("Speed", "animation effect Speed");
                myTarget.animationEffectSpeed = EditorGUILayout.FloatField(content77, myTarget.animationEffectSpeed);
                if (myTarget.animationEffectSpeed < 0)
                {
                    myTarget.animationEffectSpeed = 0;
                }

                EditorGUILayout.Space();
                GUIContent content7 = new GUIContent("Delay", "waiting time before each animation playing");
                myTarget.alwaysanimationTimeDelay = EditorGUILayout.FloatField(content7, myTarget.alwaysanimationTimeDelay);
                if (myTarget.alwaysanimationTimeDelay < 0)
                {
                    myTarget.alwaysanimationTimeDelay = 0;
                }

                EditorGUILayout.Space();
            } 
            
            CreateLineSimple();
            }

            /*
            if (selectedActionAnimations == 1)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Animation Model for the slider that is before Focused slider", MessageType.None);
                EditorGUILayout.Space();
                
                GUIContent content2pre = new GUIContent("previous Slider Animation",
                    "select the animation effect u want for pre slider");
                myTarget.preAnimationSelection =
                    (animationSelection) EditorGUILayout.EnumPopup(content2pre, myTarget.preAnimationSelection);
                EditorGUILayout.Space();

                CreateLineSimple();
            }

            if (selectedActionAnimations == 2)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Animation Model for the slider that is after Focused slider", MessageType.None);
                EditorGUILayout.Space();
                
                GUIContent content3after = new GUIContent("Next Slider Animation",
                    "select the animation effect u want for the after slider");
                myTarget.afterAnimationSelection =
                    (animationSelection) EditorGUILayout.EnumPopup(content3after, myTarget.afterAnimationSelection);
                EditorGUILayout.Space();

                CreateLineSimple();
            }
            */

        } 
        else if (selectedAction == 5)
        {
            CreateLineSimple();

            GUIContent content14d = new GUIContent("Dont Destroy On Load", "Check if you dont want the slider to be destroyed when you are changing the scene");
            myTarget.dontDestroyOnLoadFlag = EditorGUILayout.Toggle(content14d, myTarget.dontDestroyOnLoadFlag);

            if (myTarget.dontDestroyOnLoadFlag && myTarget.cam != null)
            {
                EditorGUILayout.Space();
                GUIContent content14c = new GUIContent("Dont Destroy Camera On Load", "Check if you dont want the Camera to be destroyed when you are changing the scene");
                myTarget.dontDestroyOnLoadCameraFlag = EditorGUILayout.Toggle(content14c, myTarget.dontDestroyOnLoadCameraFlag);
            }
            
            CreateLineSimple();
            
         
            GUIContent content14 = new GUIContent("Save and Load", "if enabled the slider position will be saved and loaded  when the game Restarts");
            myTarget.saveSliderPosition = EditorGUILayout.Toggle(content14, myTarget.saveSliderPosition);

            if(myTarget.saveSliderPosition)
            {
                EditorGUILayout.BeginHorizontal();
                GUI.color = Color.red;
                GUIContent content15 = new GUIContent("Delete Save Data", "Delete the saving data if there is any key from before");
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                if (GUILayout.Button(content15))
            {
                PlayerPrefs.DeleteKey("Slider_Current_Position_Value" + myTarget.gameObject.GetInstanceID());
            }

                GUI.color = Color.white;

                EditorGUILayout.EndHorizontal();

            if(PlayerPrefs.HasKey("Slider_Current_Position_Value" + myTarget.gameObject.GetInstanceID()))
            EditorGUILayout.HelpBox("Current slider Saved Position is : " + PlayerPrefs.GetInt("Slider_Current_Position_Value" + myTarget.gameObject.GetInstanceID()), MessageType.Info);
            else
                EditorGUILayout.HelpBox("There is no saved position data for this slider yet ", MessageType.Info);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUIContent content16 = new GUIContent("Fast Slider Loading", "Enable if u want the slider to be loaded on the saved position Immediatley");
            myTarget.fastLoading = EditorGUILayout.Toggle(content16, myTarget.fastLoading);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Camera Utility
            GUIContent content17 = new GUIContent("Camera");
            myTarget.cam = (Camera)EditorGUILayout.ObjectField(content17, myTarget.cam, typeof(Camera), true);
            EditorGUILayout.Space();
            if(myTarget.cam != null)
            {

                GUIContent content18lock = new GUIContent("lock On Slider", "Enable if u want the camera to be locked on the slider");
                myTarget.lockCameraOnSlider = EditorGUILayout.Toggle(content18lock, myTarget.lockCameraOnSlider);
                EditorGUILayout.Space();
                GUIContent content18 = new GUIContent("Type" , "Select Camera Type Based on your SliderObjects (3D or 2D )" );
                myTarget.cameraType = (UltimateSlider.CameraType)EditorGUILayout.EnumPopup(content18, myTarget.cameraType);


                if(myTarget.cameraType == UltimateSlider.CameraType.Orthographic_2D)
                {
        
                    myTarget.cam.orthographic = true;
                    GUIContent content18c = new GUIContent("Size", "orthographicSize Camera Size");
                    myTarget.cam.orthographicSize = EditorGUILayout.FloatField(content18c, myTarget.cam.orthographicSize);

                    if (myTarget.cam.orthographicSize < 0)
                    {
                        myTarget.cam.orthographicSize = 0;
                    }
                }
                else
                {
                    myTarget.cam.orthographic = false;
                    GUIContent content18c = new GUIContent("Field Of View", "Camera Field Of View");
                    myTarget.cam.fieldOfView = EditorGUILayout.FloatField(content18c, myTarget.cam.fieldOfView);

                    if (myTarget.cam.fieldOfView < 0)
                    {
                        myTarget.cam.fieldOfView = 0;
                    }
                }
                EditorGUILayout.Space();

                GUIContent content18cam = new GUIContent("Camera Offset", "Set the Difference in position between camera and slider Parent");

                myTarget.cameraOffset = EditorGUILayout.Vector3Field(content18cam, myTarget.cameraOffset);

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Camera Shakes will be added on the next update", MessageType.Info);

            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            EditorGUILayout.Space();

            GUIContent content19 = new GUIContent("Disable In-Active Slides", "To Increase Performance , Enable if u dont want the slides out side of the camera view to be active");
            myTarget.disableInActiveSlides = EditorGUILayout.Toggle(content19, myTarget.disableInActiveSlides);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUIContent content20h = new GUIContent("Use Handles", "use handles to show the slider current position");
            EditorGUILayout.Space();
            myTarget.showOnHandles = EditorGUILayout.Toggle(content20h, myTarget.showOnHandles);

            EditorGUILayout.Space();

        

    
            if (myTarget.showOnHandles)
            {
                GUIContent content20h2 = new GUIContent("Handles Move Slider", "enable if you want the handles to also move the sliders");
                EditorGUILayout.Space();
                myTarget.handleCanMoveSlider = EditorGUILayout.Toggle(content20h2, myTarget.handleCanMoveSlider);
                EditorGUILayout.Space();
                GUIContent content20s2 = new GUIContent("Disabled Handle Sprite", "the sprite shown when the related handle is Disabled");
                EditorGUILayout.Space();
                myTarget.disttActiveHandelSprite = (Sprite)EditorGUILayout.ObjectField(content20s2, myTarget.disttActiveHandelSprite, typeof(Sprite), false);

                GUIContent content20s1 = new GUIContent("Enabled Handle Sprite", "the sprite shown when the related handle is Enabled");
                EditorGUILayout.Space();
                myTarget.activeHandelSprite = (Sprite)EditorGUILayout.ObjectField(content20s1, myTarget.activeHandelSprite , typeof(Sprite) , false);

                EditorGUILayout.Space();

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                while (myTarget.HandlesList.Count < myTarget.sliderPagesCount)
                {
                    myTarget.HandlesList.Add(null);
                }

                while (myTarget.HandlesList.Count > myTarget.sliderPagesCount)
                {
                    myTarget.HandlesList.RemoveAt(myTarget.HandlesList.Count - 1);
                }

                bool allButtonsFull = true;
                for (int i = 0; i < myTarget.HandlesList.Count; i++)
                {
                    if (myTarget.HandlesList[i] == null)
                    {
                        allButtonsFull = false;
                    }
                }

                // remove duplicated elemetns ( not null ones)
                myTarget.HandlesList = RemoveDuplicatesIterative(myTarget.HandlesList);


                if (!allButtonsFull)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("You need to assign " + myTarget.sliderPagesCount + " UI Imaegs to work as your slider handles", MessageType.Error);
                    EditorGUILayout.Space();
                    GUI.color = Color.yellow;
                }
                else
                {
                    GUI.color = Color.green;

                }


                var serializedobject = new SerializedObject(target);
                var property = serializedobject.FindProperty("HandlesList");
                serializedobject.Update();
                EditorGUILayout.PropertyField(property, true);
                serializedobject.ApplyModifiedProperties();
                GUI.color = Color.white;

            }

            CreateLineSimple();

            /// Text Utility 
            GUIContent content20 = new GUIContent("Show On Text" , "Show current slide number on a Text with a selected format");
            EditorGUILayout.Space();
            myTarget.showPageNumberText = (Text)EditorGUILayout.ObjectField(content20, myTarget.showPageNumberText, typeof(Text), true);
 
            EditorGUILayout.Space();
            if (myTarget.showPageNumberText != null)
            {
                GUIContent content21 = new GUIContent("Select Type" , "choose how the slider number will be shown on the text");
                myTarget.showOnTextTypes =  (ShowOnTextTypes)EditorGUILayout.EnumPopup(content21, myTarget.showOnTextTypes);
                EditorGUILayout.Space();

                string SampleText = "Sample \n" + "type 1: " + "1 of 5" + "\n" +
                    "type 2: " + "1 / 5" + "\n" +
                    "type 3: " + "1" + "\n" +
                     "type 4: " + "10%" + "\n" +
                     "type 5: " + "two" + "\n" + 
                    "type 6: " + "two of five" + "\n";

                EditorGUILayout.HelpBox(SampleText, MessageType.None);


            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUI.color = Color.white;
            EditorGUILayout.Space();
              
        }
        else if (selectedAction == 6)
        {
            base.DrawDefaultInspector();
        }
        #endregion

        #region Finalizing

        if (myTarget.sliderParent != null)
        {
            for (int i = 0; i < myTarget.sliderParent.childCount; i++)
            {
                if(myTarget.sliderParent.GetChild(i).GetComponent<RectTransform>() != null)
                { 
                    myTarget.sliderParent.GetChild(i).GetComponent<RectTransform>().anchorMin = myTarget.sliderParent.GetComponent<RectTransform>().anchorMin;
                    myTarget.sliderParent.GetChild(i).GetComponent<RectTransform>().anchorMax = myTarget.sliderParent.GetComponent<RectTransform>().anchorMax;
                    myTarget.sliderParent.GetChild(i).GetComponent<RectTransform>().anchoredPosition = myTarget.sliderParent.GetComponent<RectTransform>().anchoredPosition;
                    myTarget.sliderParent.GetChild(i).GetComponent<RectTransform>().sizeDelta = myTarget.sliderParent.GetComponent<RectTransform>().sizeDelta;
                    myTarget.sliderParent.GetChild(i).GetComponent<RectTransform>().offsetMin = myTarget.sliderParent.GetComponent<RectTransform>().offsetMin;
                    myTarget.sliderParent.GetChild(i).GetComponent<RectTransform>().offsetMax = myTarget.sliderParent.GetComponent<RectTransform>().offsetMax;

                }

                myTarget.sliderParent.GetChild(i).position = 
                    new Vector3(myTarget.sliderParent.position.x + ( myTarget.sliderMoveRange.x * i )
                    , myTarget.sliderParent.position.y + (myTarget.sliderMoveRange.y * i), myTarget.sliderParent.position.z + (myTarget.sliderMoveRange.z * i));
            }

            if(myTarget.cam != null)
            {
                myTarget.cam.transform.position = myTarget.sliderParent.position + myTarget.cameraOffset;
            }
        }

        myTarget.sliderPagesCount = myTarget.sliderParent.childCount;

        GUIStyle newstyle = new GUIStyle();
        EditorStyles.label.font = newstyle.font;
        EditorStyles.label.fontSize = newstyle.fontSize;
        EditorStyles.helpBox.font = newstyle.font;
        EditorStyles.helpBox.fontSize = newstyle.fontSize;
        style2 = null;
        style = null;
        style3 = null;
        style4 = null;
        
        EditorPrefs.SetInt("SelectedAction", selectedAction);
        EditorPrefs.SetInt("selectedActionAnimations" , selectedActionAnimations);
        
        serializedObject.ApplyModifiedProperties();

     
         #endregion
    }

    #endregion

    #region Helper Functions

    private bool changePic;
    protected virtual void OnEditorUpdate()
    {
        // In here you can check the current realtime, see if a certain
        // amount of time has elapsed, and perform some task.
        timer += Time.deltaTime;
        if (timer > 5f)
        {
            timer = 0;
            counter = (counter + 1) % 5;
            this.serializedObject.Update();
        }
    }
    void CreateLineSimple()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUI.color = Color.cyan;
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUI.color = Color.white;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
    void checkSlidesEventList()
    {
        while (myTarget.slideEventClass.Count < myTarget.sliderPagesCount)
        {
            myTarget.slideEventClass.Add(new SlideEventClass());
        }

        while (myTarget.slideEventClass.Count > myTarget.sliderPagesCount)
        {
            myTarget.slideEventClass.RemoveAt(myTarget.slideEventClass.Count - 1);
        }
 
        // remove duplicated elemetns ( not null ones)
        myTarget.slideEventClass = RemoveDuplicatesIterativeEvents(myTarget.slideEventClass);

 
    }
    public static List<Image> RemoveDuplicatesIterative(List<Image> items)
    {
        List<Image> result = new List<Image>();
        for (int i = 0; i < items.Count; i++)
        {
            // Assume not duplicate.
            bool duplicate = false;
            for (int z = 0; z < i; z++)
            {
                if (items[z] != null && items[z] == items[i])
                {
                    // This is a duplicate.
                    duplicate = true;
                    break;
                }
            }
            // If not duplicate, add to result.
            if (!duplicate)
            {
                result.Add(items[i]);
            }
        }
        return result;
    }
    public static List<SlideEventClass> RemoveDuplicatesIterativeEvents(List<SlideEventClass> items)
    {
        List<SlideEventClass> result = new List<SlideEventClass>();
        for (int i = 0; i < items.Count; i++)
        {
            // Assume not duplicate.
            bool duplicate = false;
            for (int z = 0; z < i; z++)
            {
                if (items[z] != null && items[z] == items[i])
                {
                    // This is a duplicate.
                    duplicate = true;
                    break;
                }
            }
            // If not duplicate, add to result.
            if (!duplicate)
            {
                result.Add(items[i]);
            }
        }
        return result;
    }
    #endregion
}

#endif



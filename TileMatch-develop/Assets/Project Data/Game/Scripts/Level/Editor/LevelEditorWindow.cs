#pragma warning disable 649

using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Watermelon
{
    public class LevelEditorWindow : LevelEditorBase
    {
        //used variables
        private const string RETURN_LABEL = "← Back";
        private LevelRepresentation selectedLevelRepresentation;
        private LevelsHandler levelsHandler;
        private CellTypesHandler gridHandler;
        private TabHandler tabHandler;
        private string levelLabel;

        //level drawing
        private Rect drawRect;
        private int currentOffset;
        private float xSize;
        private float ySize;
        private float elementSize;
        private int selectedLayerWidth;
        private int selectedLayerHeight;
        private Event currentEvent;
        private Vector2 elementUnderMouseIndex;
        private Vector2Int elementPosition;
        private float buttonRectX;
        private float buttonRectY;
        private Rect buttonRect;
        private const int BOTTOM_PADDING = 8;
        private int selectedLayerIndex;
        private string[] layersOptions;
        private Color defaultColor;
        private int maxLayerHeight;
        private int maxLayerWidth;
        private float halfElementSize;
        private bool tempBoolValue;
        private float elementOffset;
        private float defaultLabelWidth;

        //Database 
        private const string LEVELS_PROPERTY_NAME = "levels";
        private const string TILES_PROPERTY_NAME = "tiles";
        private const string TILE_EFFECTS_PROPERTY = "tileEffects";
        private const string BACKGROUND_DATA_PROPERTY = "backgroundData";
        private const string SELECTED_COLOR_PROPERTY = "selectedColor";
        private const string UNSELECTED_COLOR_PROPERTY = "unselectedColor";
        private const string TINT_COLOR_PROPERTY = "tintColor";
        private const string BACKGROUND_COLOR_PROPERTY = "backgroundColor";
        private const string GRID_COLOR_PROPERTY = "gridColor";
        private SerializedProperty levelsSerializedProperty;
        private SerializedProperty tilesProperty;
        private SerializedProperty backgroundDataProperty;
        private SerializedProperty tileEffectsSerializedProperty;
        private SerializedProperty selectedColorSerializedProperty;
        private SerializedProperty unselectedColorSerializedProperty;
        private SerializedProperty tintColorSerializedProperty;
        private SerializedProperty backgroundColorSerializedProperty;
        private SerializedProperty gridColorSerializedProperty;

        //Draw levels Tab
        private const string LEVELS_TAB_NAME = "Levels";
        private const int SIDEBAR_WIDTH = 320;
        private const int BUTTONS_WIDTH = 200;
        private const string TEST_LEVEL = "Test level";

        //PlayerPrefs
        private const string PREFS_LEVEL = "editor_level_index";
        private const string PREFS_WIDTH = "editor_sidebar_width";
        private const string PREFS_BUTTONS_WIDTH = "editor_buttons_width";

        //edit layers tab
        private const string MOUSE_BUTTONS_INSTRUCTION = "Left click and drag to draw. Right click to select effect. Mouse wheel to switch layer.";

        private const int INFO_HEIGH = 94; //found out using Debug.Log(infoRect) on worst case scenario
        private const string LEVEL_PASSED_VALIDATION = "Level passed validation.";
        private const string LAYERS_LABEL = "Layers";
        private const string CURRENT_LAYER_LABEL = "Current layer:";
        private const string WARNING_LABEL = "Warning";
        private const string OK_LABEL = "Ok";
        private const string CANCEL_LABEL = "Cancel";
        private const string LAYER_REMOVE_WARNING = "Are you sure you want to remove bottom layer ?";

        //edit variables tab
        private Rect infoRect;

        //Tiles tab
        private const string TILES_OBJECTS_TAB_NAME = "Tiles";
        private const string SPECIAL_EFFECTS_TAB_NAME = "Special effects";
        private const string PREFAB_PROPERTY_NAME = "prefab";
        private const string AVAILABLE_FROM_LEVEL_PROPERTY_NAME = "availableFromLevel";
        private const float PROPERTIES_WIDTH_MARGIN = 4;

        private ReorderableList tilesReordableList;
        private Rect workingRect;
        private Rect firstHalfRect;
        private Rect secondHalfRect;
        //Backgrounds tab
        private const string BACKGROUNDS_TAB_NAME = "Backgrounds";
        private const string BACKGROUND_PREFAB_PROPERTY_NAME = "backgroundPrefab";
        private const string BACKGROUND_LEVEL_PROPERTY_NAME = "availableFromLevel";
        private const string COLLECTION_ID_PROPERTY_NAME = "collectionId";
        private ReorderableList backgroundReordableList;

        private bool validationNeededAfterDrag;
        private int currentSideBarWidth;
        private int currentButtonsWidth;
        private EditorSceneController editorSceneController;
        private bool lastActiveLevelOpened;
        private CellTypesHandler.ExtraProp tempExtraProp;
        private int menuIndex1;
        private int menuIndex2;
        private int menuIndex3;
        private bool preparedForMouseDrag;
        private bool[,] dragArray;
        private float layerButtonsHeight;
        private Rect separatorRect;
        private bool separatorIsDragged;
        private TintMap tintMap;
        private bool buttonSeparatorIsDragged;
        private bool drawEditVariablesTabDisplayed;
        private GUIContent editColorsContent;
        private int previousSelectedLayerIndex;

        protected override WindowConfiguration SetUpWindowConfiguration(WindowConfiguration.Builder builder)
        {
            return builder.SetWindowMinSize(new Vector2(700, 500)).Build();
        }

        protected override Type GetLevelsDatabaseType()
        {
            return typeof(LevelDatabase);
        }

        public override Type GetLevelType()
        {
            return typeof(LevelData);
        }

        protected override void ReadLevelDatabaseFields()
        {
            levelsSerializedProperty = levelsDatabaseSerializedObject.FindProperty(LEVELS_PROPERTY_NAME);
            tilesProperty = levelsDatabaseSerializedObject.FindProperty(TILES_PROPERTY_NAME);
            backgroundDataProperty = levelsDatabaseSerializedObject.FindProperty(BACKGROUND_DATA_PROPERTY);
            tileEffectsSerializedProperty = levelsDatabaseSerializedObject.FindProperty(TILE_EFFECTS_PROPERTY);

            selectedColorSerializedProperty = levelsDatabaseSerializedObject.FindProperty(SELECTED_COLOR_PROPERTY);
            unselectedColorSerializedProperty = levelsDatabaseSerializedObject.FindProperty(UNSELECTED_COLOR_PROPERTY);
            tintColorSerializedProperty = levelsDatabaseSerializedObject.FindProperty(TINT_COLOR_PROPERTY);
            backgroundColorSerializedProperty = levelsDatabaseSerializedObject.FindProperty(BACKGROUND_COLOR_PROPERTY);
            gridColorSerializedProperty = levelsDatabaseSerializedObject.FindProperty(GRID_COLOR_PROPERTY);

        }

        protected override void InitialiseVariables()
        {
            levelsHandler = new LevelsHandler(levelsDatabaseSerializedObject, levelsSerializedProperty);

            gridHandler = new CellTypesHandler();
            TileEffectType[] tileEffectTypes = (TileEffectType[])Enum.GetValues(typeof(TileEffectType));

            for (int i = 0; i < tileEffectTypes.Length; i++)
            {
                gridHandler.AddExtraProp(new CellTypesHandler.ExtraProp(i, tileEffectTypes[i].ToString()));
            }

            tabHandler = new TabHandler();
            tabHandler.AddTab(new TabHandler.Tab(LEVELS_TAB_NAME, DrawLevelsTab));
            tabHandler.AddTab(new TabHandler.Tab(TILES_OBJECTS_TAB_NAME, DrawTilesTab, OnBeforeOpenedTilesTab));
            tabHandler.AddTab(new TabHandler.Tab(BACKGROUNDS_TAB_NAME, DrawBackgroundsTab, OnBeforeOpenedBackgroundsTab));
            tabHandler.AddTab(new TabHandler.Tab(SPECIAL_EFFECTS_TAB_NAME, DrawSpecialEffectsTab));
            defaultColor = GUI.color;
            validationNeededAfterDrag = false;
            currentSideBarWidth = PlayerPrefs.GetInt(PREFS_WIDTH, SIDEBAR_WIDTH);
            currentButtonsWidth = PlayerPrefs.GetInt(PREFS_BUTTONS_WIDTH, BUTTONS_WIDTH);
            CreateEditorSceneController();
        }



        private void OpenLastActiveLevel()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Styles();

                if (!lastActiveLevelOpened)
                {
                    if ((levelsSerializedProperty.arraySize > 0) && PlayerPrefs.HasKey(PREFS_LEVEL))
                    {
                        int levelIndex = Mathf.Clamp(PlayerPrefs.GetInt(PREFS_LEVEL, 0), 0, levelsSerializedProperty.arraySize - 1);
                        levelsHandler.CustomList.SelectedIndex = levelIndex;
                        levelsHandler.OpenLevel(levelIndex);
                    }

                    lastActiveLevelOpened = true;
                }
            }
        }

        protected override void Styles()
        {
            if (tabHandler != null)
            {
                tabHandler.SetDefaultToolbarStyle();
                gridHandler.SetDefaultLabelStyle();
                editColorsContent = new GUIContent(WatermelonEditor.Styles.GetIcon("icon_menu"), "Edit used colors");
            }
        }

        public override void OpenLevel(UnityEngine.Object levelObject, int index)
        {
            PlayerPrefs.SetInt(PREFS_LEVEL, index);
            PlayerPrefs.Save();
            selectedLevelRepresentation = new LevelRepresentation(levelObject);
            levelLabel = selectedLevelRepresentation.GetLevelLabel(levelsHandler.SelectedLevelIndex, stringBuilder);
            selectedLevelRepresentation.ValidateLevel();
            UpdateLayersOptions();

            if (editorSceneController == null)
            {
                CreateEditorSceneController();
            }

            previousSelectedLayerIndex = int.MinValue;
            InitCellsForGameView();
        }

        public override string GetLevelLabel(UnityEngine.Object levelObject, int index)
        {
            return new LevelRepresentation(levelObject).GetLevelLabel(index, stringBuilder);
        }

        public override void ClearLevel(UnityEngine.Object levelObject)
        {
            new LevelRepresentation(levelObject).Clear();
        }

        public override void LogErrorsForGlobalValidation(UnityEngine.Object levelObject, int index)
        {
            LevelRepresentation level = new LevelRepresentation(levelObject);
            level.ValidateLevel();

            if (!level.IsLevelCorrect)
            {
                Debug.LogWarning("Logging validation errors for level #" + (index + 1) + " :");

                foreach (string error in level.errorLabels)
                {
                    Debug.LogWarning(error);
                }
            }
            else
            {
                Debug.Log("Level # " + +(index + 1) + " passed validation.");
            }
        }

        protected override void DrawContent()
        {
            tabHandler.DisplayTab();
        }

        #region Levels Tab
        private void DrawLevelsTab()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            DisplaySidebar();
            HandleChangingSideBar();
            DisplaySelectedLevel();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void HandleChangingSideBar()
        {
            separatorRect = EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.MinWidth(8), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.AddCursorRect(separatorRect, MouseCursor.ResizeHorizontal);


            if (separatorRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    separatorIsDragged = true;
                    levelsHandler.IgnoreDragEvents = true;
                    Event.current.Use();
                }
            }

            if (separatorIsDragged)
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    separatorIsDragged = false;
                    levelsHandler.IgnoreDragEvents = false;
                    PlayerPrefs.SetInt(PREFS_WIDTH, currentSideBarWidth);
                    PlayerPrefs.Save();
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseDrag)
                {
                    currentSideBarWidth = Mathf.RoundToInt(Event.current.delta.x) + currentSideBarWidth;
                    Event.current.Use();
                }
            }
        }

        private void DisplaySidebar()
        {
            OpenLastActiveLevel();

            EditorGUILayout.BeginVertical(GUILayout.Width(currentSideBarWidth));
            levelsHandler.DisplayReordableList();
            levelsHandler.DrawRenameLevelsButton();
            levelsHandler.DrawGlobalValidationButton();

            if(GUILayout.Button("Clear selection", WatermelonEditor.Styles.button_01))
            {
                ClearSelection();
                PlayerPrefs.DeleteKey(PREFS_LEVEL);
                PlayerPrefs.Save();
            }

            EditorGUILayout.EndVertical();
        }
        private void DisplaySelectedLevel()
        {
            if (drawEditVariablesTabDisplayed)
            {
                DrawEditVariablesTab();
                return;
            }


            if (levelsHandler.SelectedLevelIndex == -1)
            {
                EditorGUILayout.Space();
                return;
            }


            EditorGUILayout.BeginVertical();
            DrawEditLayersTab();
            EditorGUILayout.EndVertical();
        }



        private void UpdateLayersOptions()
        {
            selectedLayerIndex = -1;

            if (selectedLevelRepresentation.layersProperty.arraySize > 0)
            {
                selectedLayerIndex = 0;
            }

            layersOptions = new string[selectedLevelRepresentation.layersProperty.arraySize];

            for (int i = 0; i < layersOptions.Length; i++)
            {
                if (i == 0)
                {
                    layersOptions[i] = selectedLevelRepresentation.GetLayerName(i) + " [top]";
                }
                else if (i == layersOptions.Length - 1)
                {
                    layersOptions[i] = selectedLevelRepresentation.GetLayerName(i) + " [bottom]";
                }
                else
                {
                    layersOptions[i] = selectedLevelRepresentation.GetLayerName(i);
                }
            }
        }

        private void CreateEditorSceneController()
        {
            GameObject gameObject = new GameObject("[Level editor]");
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            editorSceneController = new EditorSceneController(gameObject);

            if (tilesProperty.arraySize == 0)
            {
                Debug.LogError("Level editor needs at least one object tiles array to work properly");
            }

            if (tilesProperty.GetArrayElementAtIndex(0).FindPropertyRelative(PREFAB_PROPERTY_NAME).objectReferenceValue != null)
            {
                editorSceneController.InitMaterials(tilesProperty.GetArrayElementAtIndex(0).FindPropertyRelative(PREFAB_PROPERTY_NAME).objectReferenceValue as GameObject);
            }
            else
            {
                Debug.LogError("Null prefab at index 0.");
            }


        }

        private void InitCellsForGameView()
        {
            bool value;
            editorSceneController.Clear();
            int offset = selectedLevelRepresentation.GetOffset();
            editorSceneController.EvenLayerSize = new Vector2Int(selectedLevelRepresentation.bottomLayerWidthProperty.intValue, selectedLevelRepresentation.bottomLayerWidthProperty.intValue);
            editorSceneController.OddLayerSize = new Vector2Int(selectedLevelRepresentation.bottomLayerWidthProperty.intValue + offset, selectedLevelRepresentation.bottomLayerWidthProperty.intValue + offset);
            GameObject[] prefabs = new GameObject[Mathf.Clamp(selectedLevelRepresentation.elementsPerLevelProperty.intValue, 1, tilesProperty.arraySize - 1)];

            for (int i = 0; i < prefabs.Length; i++)
            {
                prefabs[i] = tilesProperty.GetArrayElementAtIndex(i).FindPropertyRelative(PREFAB_PROPERTY_NAME).objectReferenceValue as GameObject;
            }

            editorSceneController.prefabs = prefabs;
            editorSceneController.setsAmount = selectedLevelRepresentation.SetsAmount;

            int layerWidth;
            int layerHeight;

            for (int i = 0; i < selectedLevelRepresentation.layersProperty.arraySize; i++)
            {
                layerWidth = selectedLevelRepresentation.GetLayerWidth(i);
                layerHeight = selectedLevelRepresentation.GetLayerHeight(i);

                for (int y = 0; y < layerHeight; y++)
                {
                    for (int x = 0; x < layerWidth; x++)
                    {
                        value = selectedLevelRepresentation.GetCellValue(i, x, y);
                        editorSceneController.InitCells(i, x, y, value, selectedLevelRepresentation.layersProperty.arraySize);
                    }
                }
            }
        }

        private void SetAsCurrentLevel()
        {
            GlobalSave globalSave = SaveController.GetGlobalSave();

            LevelSave gameSave = globalSave.GetSaveObject<LevelSave>("level");
            gameSave.RealLevelIndex = levelsHandler.SelectedLevelIndex;
            gameSave.DisplayLevelIndex = levelsHandler.SelectedLevelIndex;
            gameSave.MaxReachedLevelIndex = levelsHandler.SelectedLevelIndex;

            SaveController.SaveCustom(globalSave);
            GameController.AutoRunLevelInEditor = true;

            EditorApplication.ExecuteMenuItem("Edit/Play");

            if (editorSceneController != null)
            {
                editorSceneController.Destroy();
                editorSceneController = null;
            }
        }

        #endregion


        #region Tiles Tab

        private void OnBeforeOpenedTilesTab()
        {
            tilesReordableList = new ReorderableList(levelsDatabaseSerializedObject, tilesProperty);
            tilesReordableList.drawHeaderCallback += TilesDrawHeader;
            tilesReordableList.drawElementCallback += TilesDrawElement;
        }

        private void TilesDrawHeader(Rect rect)
        {
            GUI.Label(rect, TILES_OBJECTS_TAB_NAME);
        }

        private void TilesDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            firstHalfRect = new Rect(rect.x, rect.y, (rect.width / 2f) - PROPERTIES_WIDTH_MARGIN, rect.height);
            secondHalfRect = new Rect(rect.x + firstHalfRect.width + (PROPERTIES_WIDTH_MARGIN * 2f), rect.y, firstHalfRect.width, rect.height);
            EditorGUI.PropertyField(firstHalfRect, tilesProperty.GetArrayElementAtIndex(index).FindPropertyRelative(PREFAB_PROPERTY_NAME));
            EditorGUI.PropertyField(secondHalfRect, tilesProperty.GetArrayElementAtIndex(index).FindPropertyRelative(AVAILABLE_FROM_LEVEL_PROPERTY_NAME));
        }

        private void DrawTilesTab()
        {
            EditorGUILayout.BeginVertical();
            tilesReordableList.DoLayoutList();
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Backgrounds Tab

        private void OnBeforeOpenedBackgroundsTab()
        {
            backgroundReordableList = new ReorderableList(levelsDatabaseSerializedObject, backgroundDataProperty);
            backgroundReordableList.drawHeaderCallback += BackgroundsDrawHeader;
            backgroundReordableList.drawElementCallback += BackgroundsDrawElement;
        }

        private void BackgroundsDrawHeader(Rect rect)
        {
            GUI.Label(rect, BACKGROUNDS_TAB_NAME);
        }

        private void BackgroundsDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            firstHalfRect = new Rect(rect.x, rect.y, (rect.width / 2f) - PROPERTIES_WIDTH_MARGIN - PROPERTIES_WIDTH_MARGIN, rect.height);
            secondHalfRect = new Rect(rect.x + firstHalfRect.width + (PROPERTIES_WIDTH_MARGIN * 2f), rect.y, firstHalfRect.width, rect.height);
            EditorGUI.PropertyField(firstHalfRect, backgroundDataProperty.GetArrayElementAtIndex(index).FindPropertyRelative(BACKGROUND_PREFAB_PROPERTY_NAME));
            EditorGUI.PropertyField(secondHalfRect, backgroundDataProperty.GetArrayElementAtIndex(index).FindPropertyRelative(BACKGROUND_LEVEL_PROPERTY_NAME));
        }

        private void DrawBackgroundsTab()
        {
            EditorGUILayout.BeginVertical();
            backgroundReordableList.DoLayoutList();
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Edit Layers Tab
        private void DrawEditLayersTab()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(TEST_LEVEL, WatermelonEditor.Styles.button_01))
            {
                SetAsCurrentLevel();
            }

            GUILayout.Space(20);

            EditorGUILayout.LabelField(levelLabel, WatermelonEditor.Styles.label_medium);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(editColorsContent, GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                drawEditVariablesTabDisplayed = true;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(LAYERS_LABEL, WatermelonEditor.Styles.label_medium_bold);
            EditorGUILayout.BeginHorizontal();

            if (selectedLevelRepresentation.layersProperty.arraySize == 1)
            {
                if (GUILayout.Button("Add larger layer", WatermelonEditor.Styles.button_03))
                {
                    selectedLevelRepresentation.AddLayer(1);
                    UpdateLayersOptions();
                    selectedLayerIndex = 0;
                    selectedLevelRepresentation.ValidateLevel();
                    InitCellsForGameView();
                    previousSelectedLayerIndex = int.MinValue;
                }

                if (GUILayout.Button("Add smaller layer", WatermelonEditor.Styles.button_03))
                {
                    selectedLevelRepresentation.AddLayer(-1);
                    UpdateLayersOptions();
                    selectedLayerIndex = 0;
                    selectedLevelRepresentation.ValidateLevel();
                    InitCellsForGameView();
                    previousSelectedLayerIndex = int.MinValue;
                }
            }
            else if (GUILayout.Button("Add Layer", WatermelonEditor.Styles.button_03))
            {
                selectedLevelRepresentation.AddLayer();
                UpdateLayersOptions();
                selectedLayerIndex = 0;
                selectedLevelRepresentation.ValidateLevel();
                InitCellsForGameView();
                previousSelectedLayerIndex = int.MinValue;
            }

            EditorGUI.BeginDisabledGroup(selectedLevelRepresentation.layersProperty.arraySize == 0);

            if (GUILayout.Button("Remove top layer", WatermelonEditor.Styles.button_04))
            {
                if (EditorUtility.DisplayDialog(WARNING_LABEL, LAYER_REMOVE_WARNING, OK_LABEL, CANCEL_LABEL))
                {
                    selectedLevelRepresentation.layersProperty.DeleteArrayElementAtIndex(0);
                    UpdateLayersOptions();
                    selectedLevelRepresentation.ValidateLevel();
                    InitCellsForGameView();
                    previousSelectedLayerIndex = int.MinValue;
                }
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (IsPropertyChanged(selectedLevelRepresentation.bottomLayerWidthProperty))
            {
                selectedLevelRepresentation.HandleSizePropertyChange();
                UpdateLayersOptions();
                selectedLevelRepresentation.ValidateLevel();
                InitCellsForGameView();
            }

            if (IsPropertyChanged(selectedLevelRepresentation.bottomLayerHeightProperty))
            {
                selectedLevelRepresentation.HandleSizePropertyChange();
                UpdateLayersOptions();
                selectedLevelRepresentation.ValidateLevel();
                InitCellsForGameView();
            }

            if (IsPropertyChanged(selectedLevelRepresentation.elementsPerLevelProperty))
            {
                InitCellsForGameView();
            }

            EditorGUILayout.PropertyField(selectedLevelRepresentation.coinsRewardProperty);
            EditorGUILayout.PropertyField(selectedLevelRepresentation.useInRandomizerProperty);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Clear effects", WatermelonEditor.Styles.button_01, GUILayout.Width(EditorGUIUtility.labelWidth)))
            {
                selectedLevelRepresentation.ClearEffects();
            }

            if (GUILayout.Button("Trim Level", WatermelonEditor.Styles.button_01, GUILayout.Width(EditorGUIUtility.labelWidth)))
            {
                selectedLevelRepresentation.TrimLevel();
                UpdateLayersOptions();
                InitCellsForGameView();
                previousSelectedLayerIndex = int.MinValue;
            }

            EditorGUILayout.EndHorizontal();
            //layer selection 
            selectedLayerIndex = EditorGUILayout.Popup(CURRENT_LAYER_LABEL, selectedLayerIndex, layersOptions);
            HandleScrollEvent();

            EditorGUILayout.BeginHorizontal();

            if (selectedLayerIndex != -1)
            {
                DrawLayer();
            }
            else
            {
                GUILayout.FlexibleSpace();
            }

            HandleChangingButtonWidth();
            selectedLayerIndex = GUILayout.SelectionGrid(selectedLayerIndex, layersOptions, 1, GUILayout.Width(currentButtonsWidth), GUILayout.ExpandHeight(true));

            EditorGUILayout.EndHorizontal();

            selectedLevelRepresentation.ApplyChanges();
            levelsHandler.UpdateCurrentLevelLabel(selectedLevelRepresentation.GetLevelLabel(levelsHandler.SelectedLevelIndex, stringBuilder));

            DrawTipsAndWarnings();

            EditorGUILayout.EndVertical();
        }



        private void HandleScrollEvent()
        {
            currentEvent = Event.current;

            if (currentEvent.type == EventType.ScrollWheel)
            {

                if (currentEvent.delta.y != 0)
                {
                    if (currentEvent.delta.y < 0) // scroll up
                    {
                        if (selectedLayerIndex > 0)
                        {
                            selectedLayerIndex--;
                        }
                    }
                    else
                    {
                        if (selectedLayerIndex < layersOptions.Length - 1)
                        {
                            selectedLayerIndex++;
                        }
                    }

                    currentEvent.Use();
                }
            }
        }

        private void DrawLayer()
        {
            drawRect = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            currentOffset = selectedLevelRepresentation.GetOffset();
            maxLayerHeight = Mathf.Max(selectedLevelRepresentation.bottomLayerHeightProperty.intValue, selectedLevelRepresentation.bottomLayerHeightProperty.intValue + currentOffset);
            maxLayerWidth = Mathf.Max(selectedLevelRepresentation.bottomLayerWidthProperty.intValue, selectedLevelRepresentation.bottomLayerWidthProperty.intValue + currentOffset);
            xSize = Mathf.Floor(drawRect.width / maxLayerWidth);
            ySize = Mathf.Floor(drawRect.height / maxLayerHeight);
            elementSize = Mathf.Min(xSize, ySize);
            selectedLayerWidth = selectedLevelRepresentation.GetLayerWidth(selectedLayerIndex);
            selectedLayerHeight = selectedLevelRepresentation.GetLayerHeight(selectedLayerIndex);
            halfElementSize = elementSize / 2f;

            if (selectedLayerHeight == maxLayerHeight)
            {
                elementOffset = 0;
            }
            else
            {
                elementOffset = halfElementSize;
            }


            currentEvent = Event.current;

            if (!buttonSeparatorIsDragged)
            {
                //Handle drag
                if ((currentEvent.type == EventType.MouseDrag) && (currentEvent.button == 0))
                {
                    if (!validationNeededAfterDrag)
                    {
                        dragArray = new bool[selectedLayerWidth, selectedLayerHeight];
                        levelsHandler.IgnoreDragEvents = true;
                    }

                    elementUnderMouseIndex = (currentEvent.mousePosition - drawRect.position - (Vector2.one * elementOffset)) / (elementSize);

                    elementPosition = new Vector2Int(Mathf.FloorToInt(elementUnderMouseIndex.x), Mathf.FloorToInt(elementUnderMouseIndex.y));

                    if ((elementPosition.x >= 0) && (elementPosition.x < selectedLayerWidth) && (elementPosition.y >= 0) && (elementPosition.y < selectedLayerHeight) && (!dragArray[elementPosition.x, elementPosition.y]))
                    {
                        tempBoolValue = selectedLevelRepresentation.ToggleCellValue(selectedLayerIndex, elementPosition.x, selectedLayerHeight - elementPosition.y - 1);
                        editorSceneController.UpdateCells(selectedLayerIndex, elementPosition.x, selectedLayerHeight - elementPosition.y - 1, tempBoolValue);
                        currentEvent.Use();
                        validationNeededAfterDrag = true;
                        dragArray[elementPosition.x, elementPosition.y] = true;
                    }
                }
                else if ((currentEvent.type == EventType.MouseUp) && validationNeededAfterDrag)
                {
                    selectedLevelRepresentation.ValidateLevel();
                    validationNeededAfterDrag = false;
                    levelsHandler.IgnoreDragEvents = false;
                }

            }


            //draw  background
            GUI.color = backgroundColorSerializedProperty.colorValue;
            GUI.DrawTexture(new Rect(drawRect.position.x, drawRect.position.y, elementSize * maxLayerWidth, elementSize * maxLayerHeight), Texture2D.whiteTexture);

            //draw current layer
            for (int rowIndex = selectedLayerHeight - 1; rowIndex >= 0; rowIndex--)
            {
                for (int columnIndex = 0; columnIndex < selectedLayerWidth; columnIndex++)
                {
                    buttonRectX = drawRect.position.x + columnIndex * elementSize + elementOffset;
                    buttonRectY = drawRect.position.y + (selectedLayerHeight - rowIndex - 1) * elementSize + elementOffset;
                    buttonRect = new Rect(buttonRectX, buttonRectY, elementSize, elementSize);

                    if (selectedLevelRepresentation.GetCellValue(selectedLayerIndex, columnIndex, rowIndex))
                    {
                        GUI.color = selectedColorSerializedProperty.colorValue;
                    }
                    else
                    {
                        GUI.color = unselectedColorSerializedProperty.colorValue;
                    }

                    GUI.DrawTexture(buttonRect, Texture2D.whiteTexture);
                    tempExtraProp = gridHandler.GetExtraProp(selectedLevelRepresentation.GetCellEffect(selectedLayerIndex, columnIndex, rowIndex));


                    if (GUI.Button(buttonRect, GUIContent.none, GUIStyle.none))
                    {
                        if (currentEvent.button == 0)
                        {
                            tempBoolValue = selectedLevelRepresentation.ToggleCellValue(selectedLayerIndex, columnIndex, rowIndex);
                            editorSceneController.UpdateCells(selectedLayerIndex, columnIndex, rowIndex, tempBoolValue);
                            selectedLevelRepresentation.ValidateLevel();
                        }
                        else if ((currentEvent.button == 1) && (selectedLevelRepresentation.GetCellValue(selectedLayerIndex, columnIndex, rowIndex)))
                        {
                            GenericMenu menu = new GenericMenu();

                            menuIndex1 = selectedLayerIndex;
                            menuIndex2 = columnIndex;
                            menuIndex3 = rowIndex;

                            foreach (CellTypesHandler.ExtraProp el in gridHandler.extraProps)
                            {
                                menu.AddItem(new GUIContent(el.label), el.value == tempExtraProp.value, delegate
                                {
                                    selectedLevelRepresentation.SetCellEffect(menuIndex1, menuIndex2, menuIndex3, el.value);
                                    selectedLevelRepresentation.ValidateLevel();
                                });
                            }

                            menu.ShowAsContext();
                        }
                    }

                    if (tempExtraProp.value != 0)
                    {
                        GUI.color = defaultColor;
                        GUI.Label(buttonRect, tempExtraProp.label, gridHandler.GetLabelStyle(GUI.color));
                    }
                }
            }

            HandleTint();

            // draw grid
            GUI.color = gridColorSerializedProperty.colorValue;


            for (int i = 0; i <= selectedLayerWidth; i++)
            {
                GUI.DrawTexture(new Rect(drawRect.position.x + elementOffset + elementSize * i, drawRect.position.y + elementOffset, 1, elementSize * selectedLayerHeight), Texture2D.whiteTexture);
            }

            for (int i = 0; i <= selectedLayerHeight; i++)
            {
                GUI.DrawTexture(new Rect(drawRect.position.x + elementOffset, drawRect.position.y + elementOffset + elementSize * i, elementSize * selectedLayerWidth, 1), Texture2D.whiteTexture);
            }

            GUI.color = defaultColor;
            EditorGUILayout.EndVertical();

            GUILayout.Space(BOTTOM_PADDING);
        }

        private void HandleTint()
        {
            if (previousSelectedLayerIndex != selectedLayerIndex)
            {
                tintMap = new TintMap(maxLayerWidth, maxLayerHeight);

                int layerWidth;
                int layerHeight;

                for (int layerIndex = selectedLevelRepresentation.layersProperty.arraySize - 1; layerIndex > selectedLayerIndex; layerIndex--)
                {
                    layerWidth = selectedLevelRepresentation.GetLayerWidth(layerIndex);
                    layerHeight = selectedLevelRepresentation.GetLayerHeight(layerIndex);

                    for (int x = 0; x < layerWidth; x++)
                    {
                        for (int y = 0; y < layerHeight; y++)
                        {
                            if (selectedLevelRepresentation.GetCellValue(layerIndex, x, y))
                            {
                                if (layerHeight == maxLayerHeight)
                                {
                                    tintMap.MarkPositionInOuterLayer(x, y);
                                }
                                else
                                {
                                    tintMap.MarkPositionInInnerLayer(x, y);
                                }
                            }
                        }
                    }
                }

                tintMap.FillMapList();
                previousSelectedLayerIndex = selectedLayerIndex;
                editorSceneController.UpdateSelectedLayerIndex(selectedLayerIndex);
            }


            GUI.color = tintColorSerializedProperty.colorValue;

            foreach (Vector2Int el in tintMap.mapList)
            {
                GUI.DrawTexture(new Rect(drawRect.position + Vector2.one * halfElementSize * el, Vector2.one * halfElementSize), Texture2D.whiteTexture);
            }

        }

        private void HandleChangingButtonWidth()
        {
            separatorRect = EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.MinWidth(8), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.AddCursorRect(separatorRect, MouseCursor.ResizeHorizontal);


            if (separatorRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    buttonSeparatorIsDragged = true;
                    Event.current.Use();
                }
            }

            if (buttonSeparatorIsDragged)
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    buttonSeparatorIsDragged = false;
                    PlayerPrefs.SetInt(PREFS_BUTTONS_WIDTH, currentButtonsWidth);
                    PlayerPrefs.Save();
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseDrag)
                {
                    currentButtonsWidth = currentButtonsWidth - Mathf.RoundToInt(Event.current.delta.x);
                    Event.current.Use();
                }
            }
        }

        private void DrawTipsAndWarnings()
        {
            infoRect = EditorGUILayout.BeginVertical(GUILayout.MinHeight(INFO_HEIGH));

            if (selectedLevelRepresentation.IsLevelCorrect)
            {
                EditorGUILayout.HelpBox(LEVEL_PASSED_VALIDATION, MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(selectedLevelRepresentation.errorLabels[0], MessageType.Error);
            }

            EditorGUILayout.HelpBox($"Tiles amount: {selectedLevelRepresentation.elementCounter}. Sets amount: {selectedLevelRepresentation.SetsAmount}. {selectedLevelRepresentation.effectsLabel}\nDifficulty score: {selectedLevelRepresentation.DifficultyScore} (sets / unique items)\n\n{MOUSE_BUTTONS_INSTRUCTION}", MessageType.Info);
            EditorGUILayout.EndVertical();

            //Debug.Log(infoRect.height);
        }

        #endregion

        private void DrawEditVariablesTab()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(RETURN_LABEL, WatermelonEditor.Styles.button_01))
            {
                drawEditVariablesTabDisplayed = false;
            }

            EditorGUILayout.LabelField("Colors used during level editing", WatermelonEditor.Styles.label_medium_bold);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(selectedColorSerializedProperty);
            EditorGUILayout.PropertyField(unselectedColorSerializedProperty);
            EditorGUILayout.PropertyField(tintColorSerializedProperty);
            EditorGUILayout.PropertyField(backgroundColorSerializedProperty);
            EditorGUILayout.PropertyField(gridColorSerializedProperty);
            EditorGUILayout.EndVertical();
        }

        private void DrawSpecialEffectsTab()
        {
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(550));
            EditorGUILayout.PropertyField(tileEffectsSerializedProperty);
            EditorGUILayout.EndVertical();
        }

        private void OnDestroy()
        {
            if (editorSceneController != null)
            {
                editorSceneController.Destroy();
                editorSceneController = null;
            }
        }

        public override void OnBeforeAssemblyReload()
        {
            ClearSelection();
            lastActiveLevelOpened = false;
        }

        public override bool WindowClosedInPlaymode()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (editorSceneController != null)
                {
                    ClearSelection();
                    lastActiveLevelOpened = false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        private void ClearSelection()
        {
            levelsHandler.ClearSelection();
            selectedLevelRepresentation = null;

            if (editorSceneController != null)
            {
                editorSceneController.Destroy();
                editorSceneController = null;
            }
        }


        private class TintMap
        {
            public int maxLayerWidth;
            public int maxLayerHeight;
            public bool[,] map;
            public List<Vector2Int> mapList;

            public TintMap(int maxLayerWidth, int maxLayerHeight)
            {
                this.maxLayerWidth = maxLayerWidth;
                this.maxLayerHeight = maxLayerHeight;
                this.map = new bool[maxLayerWidth * 2, maxLayerHeight * 2];
                mapList = new List<Vector2Int>();
            }

            public void MarkPositionInOuterLayer(int x, int y) // if firstLayerSize = 5 this function is for every layer with size 6
            {
                int topLeftX = x * 2;
                int topLeftY = (maxLayerHeight - y - 1) * 2;

                try
                {
                    map[topLeftX, topLeftY] = true;
                    map[topLeftX + 1, topLeftY] = true;
                    map[topLeftX, topLeftY + 1] = true;
                    map[topLeftX + 1, topLeftY + 1] = true;
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.LogError("Incorrect layers size.");
                    Debug.LogError(e);
                }
            }

            public void MarkPositionInInnerLayer(int x, int y) // if firstLayerSize = 5 this function is for every layer with size 5
            {
                int topLeftX = x * 2 + 1;
                int topLeftY = (maxLayerHeight - y - 2) * 2 + 1;

                try
                {
                    map[topLeftX, topLeftY] = true;
                    map[topLeftX + 1, topLeftY] = true;
                    map[topLeftX, topLeftY + 1] = true;
                    map[topLeftX + 1, topLeftY + 1] = true;
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.LogError("Incorrect layers size");
                    Debug.LogError(e);
                }
            }

            public void FillMapList()
            {
                for (int i = 0; i < maxLayerWidth * 2; i++)
                {
                    for (int j = 0; j < maxLayerHeight * 2; j++)
                    {
                        if (map[i, j])
                        {
                            mapList.Add(new Vector2Int(i, j));
                        }
                    }
                }
            }
        }

        protected class LevelRepresentation : LevelRepresentationBase
        {
            private const string LAYERS_PROPERTY_NAME = "layers";
            private const string BOTTOM_LAYER_WIDTH_PROPERTY_NAME = "bottomLayerWidth";
            private const string BOTTOM_LAYER_HEIGHT_PROPERTY_NAME = "bottomLayerHeight";
            private const string USER_IN_RANDOMIZER_PROPERTY_NAME = "useInRandomizer";
            private const string ELEMENTS_PER_LEVEL_PROPERTY_NAME = "elementsPerLevel";
            private const string COINS_REWARD_PROPERTY_NAME = "coinsReward";
            private const string EDITOR_NOTE_PROPERTY_NAME = "editorNote";


            private const string ROWS_PROPERTY_NAME = "rows";
            private const string CELLS_PROPERTY_NAME = "cells";
            private const string CELL_IS_FILLED_PROPERTY_NAME = "IsFilled";
            private const string EFFECT_PROPERTY_NAME = "Effect";


            public SerializedProperty layersProperty;
            public SerializedProperty bottomLayerWidthProperty;
            public SerializedProperty bottomLayerHeightProperty;
            public SerializedProperty useInRandomizerProperty;
            public SerializedProperty elementsPerLevelProperty;
            public SerializedProperty coinsRewardProperty;
            public SerializedProperty editorNoteProperty;

            private SerializedProperty tempProperty;

            public int elementCounter;
            public int SetsAmount => (elementCounter - (elementCounter % 3)) / 3;
            public string effectsLabel;
            private int[] effectsCounter;


            public float DifficultyScore => Mathf.Round(Mathf.Clamp(SetsAmount / (float)elementsPerLevelProperty.intValue, 1, float.MaxValue) * 10.0f) * 0.1f;

            protected override bool LEVEL_CHECK_ENABLED => true;

            public LevelRepresentation(UnityEngine.Object levelObject) : base(levelObject)
            {
            }

            protected override void ReadFields()
            {
                layersProperty = serializedLevelObject.FindProperty(LAYERS_PROPERTY_NAME);
                bottomLayerWidthProperty = serializedLevelObject.FindProperty(BOTTOM_LAYER_WIDTH_PROPERTY_NAME);
                bottomLayerHeightProperty = serializedLevelObject.FindProperty(BOTTOM_LAYER_HEIGHT_PROPERTY_NAME);
                useInRandomizerProperty = serializedLevelObject.FindProperty(USER_IN_RANDOMIZER_PROPERTY_NAME);
                elementsPerLevelProperty = serializedLevelObject.FindProperty(ELEMENTS_PER_LEVEL_PROPERTY_NAME);
                coinsRewardProperty = serializedLevelObject.FindProperty(COINS_REWARD_PROPERTY_NAME);
                editorNoteProperty = serializedLevelObject.FindProperty(EDITOR_NOTE_PROPERTY_NAME);

                effectsCounter = new int[Enum.GetValues(typeof(TileEffectType)).Length];
            }

            public override void Clear()
            {
                layersProperty.arraySize = 0;
                bottomLayerHeightProperty.intValue = 10;
                bottomLayerWidthProperty.intValue = 10;
                useInRandomizerProperty.boolValue = true;
                elementsPerLevelProperty.intValue = 8;
                coinsRewardProperty.intValue = 20;
                ApplyChanges();
            }

            public bool GetCellValue(int layer, int xIndex, int yIndex)
            {
                return layersProperty.GetArrayElementAtIndex(layer).FindPropertyRelative(ROWS_PROPERTY_NAME).GetArrayElementAtIndex(yIndex).FindPropertyRelative(CELLS_PROPERTY_NAME).GetArrayElementAtIndex(xIndex).FindPropertyRelative(CELL_IS_FILLED_PROPERTY_NAME).boolValue;
            }

            public void SetCellValue(int layer, int xIndex, int yIndex, bool newValue)
            {
                layersProperty.GetArrayElementAtIndex(layer).FindPropertyRelative(ROWS_PROPERTY_NAME).GetArrayElementAtIndex(yIndex).FindPropertyRelative(CELLS_PROPERTY_NAME).GetArrayElementAtIndex(xIndex).FindPropertyRelative(CELL_IS_FILLED_PROPERTY_NAME).boolValue = newValue;
            }

            public bool ToggleCellValue(int layer, int xIndex, int yIndex)
            {
                tempProperty = layersProperty.GetArrayElementAtIndex(layer).FindPropertyRelative(ROWS_PROPERTY_NAME).GetArrayElementAtIndex(yIndex).FindPropertyRelative(CELLS_PROPERTY_NAME).GetArrayElementAtIndex(xIndex).FindPropertyRelative(CELL_IS_FILLED_PROPERTY_NAME);
                tempProperty.boolValue = (!tempProperty.boolValue);
                return tempProperty.boolValue;
            }

            public int GetCellEffect(int layer, int xIndex, int yIndex)
            {
                return layersProperty.GetArrayElementAtIndex(layer).FindPropertyRelative(ROWS_PROPERTY_NAME).GetArrayElementAtIndex(yIndex).FindPropertyRelative(CELLS_PROPERTY_NAME).GetArrayElementAtIndex(xIndex).FindPropertyRelative(EFFECT_PROPERTY_NAME).enumValueIndex;
            }

            public void SetCellEffect(int layer, int xIndex, int yIndex, int newValue)
            {
                layersProperty.GetArrayElementAtIndex(layer).FindPropertyRelative(ROWS_PROPERTY_NAME).GetArrayElementAtIndex(yIndex).FindPropertyRelative(CELLS_PROPERTY_NAME).GetArrayElementAtIndex(xIndex).FindPropertyRelative(EFFECT_PROPERTY_NAME).enumValueIndex = newValue;
            }

            public void HandleSizePropertyChange()
            {
                if (bottomLayerHeightProperty.intValue < 2)
                {
                    bottomLayerHeightProperty.intValue = 2;
                }

                if (bottomLayerWidthProperty.intValue < 2)
                {
                    bottomLayerWidthProperty.intValue = 2;
                }

                for (int layerIndex = 0; layerIndex < layersProperty.arraySize; layerIndex++)
                {
                    SetCorrectLevelSize(layerIndex);
                }
            }

            private void SetCorrectLevelSize(int layerIndex)
            {
                int correctWidth;
                int correctHeight;

                if ((layersProperty.arraySize + 1) % 2 == 0)
                {
                    correctWidth = bottomLayerWidthProperty.intValue;
                    correctHeight = bottomLayerHeightProperty.intValue;
                }
                else
                {
                    int offset = GetOffset();
                    correctWidth = bottomLayerWidthProperty.intValue + offset;
                    correctHeight = bottomLayerHeightProperty.intValue + offset;
                }

                for (int i = 0; i < correctHeight; i++)
                {
                    layersProperty.GetArrayElementAtIndex(layerIndex).FindPropertyRelative(ROWS_PROPERTY_NAME).arraySize = correctHeight;
                    layersProperty.GetArrayElementAtIndex(layerIndex).FindPropertyRelative(ROWS_PROPERTY_NAME).GetArrayElementAtIndex(i).FindPropertyRelative(CELLS_PROPERTY_NAME).arraySize = correctWidth;
                }
            }

            public int GetOffset()
            {
                if (layersProperty.arraySize >= 2)
                {
                    return GetLayerHeight(layersProperty.arraySize - 2) - GetLayerHeight(layersProperty.arraySize - 1);
                }
                else
                {
                    return 1;
                }
            }

            public void ClearEffects()
            {
                int layerWidth;
                int layerHeight;

                for (int i = 0; i < layersProperty.arraySize; i++)
                {
                    layerWidth = GetLayerWidth(i);
                    layerHeight = GetLayerHeight(i);

                    for (int y = 0; y < layerHeight; y++)
                    {
                        for (int x = 0; x < layerWidth; x++)
                        {
                            SetCellEffect(i, x, y, 0);
                        }
                    }
                }
            }

            public override string GetLevelLabel(int index, StringBuilder stringBuilder)
            {
                if(NullLevel || (!IsLevelCorrect))
                {
                    return base.GetLevelLabel(index, stringBuilder);
                }
                else
                {
                    return base.GetLevelLabel(index, stringBuilder) + editorNoteProperty.stringValue;
                }
            }

            public override void ValidateLevel() // validation from old editor
            {
                errorLabels.Clear();
                //check layers size

                for (int i = 0; i < effectsCounter.Length; i++)
                {
                    effectsCounter[i] = 0;
                }

                if(layersProperty.arraySize < 2)
                {
                    errorLabels.Add("Level requires at least 2 layers.");
                }

                bool haveOffset = false;
                int offset = GetOffset();

                for (int i = layersProperty.arraySize - 1; i >= 0; i--)
                {
                    if (haveOffset)
                    {
                        if (GetLayerHeight(i) != bottomLayerHeightProperty.intValue + offset)
                        {
                            errorLabels.Add(GetLayerName(i) + " have incorrect height. Correct height: " + (bottomLayerHeightProperty.intValue + offset));
                        }

                        if (GetLayerWidth(i) != bottomLayerWidthProperty.intValue + offset)
                        {
                            errorLabels.Add(GetLayerName(i) + " have incorrect width. Correct width: " + (bottomLayerWidthProperty.intValue + offset));
                        }
                    }
                    else
                    {
                        if (GetLayerHeight(i) != bottomLayerHeightProperty.intValue)
                        {
                            errorLabels.Add(GetLayerName(i) + " have incorrect height. Correct height: " + (bottomLayerHeightProperty.intValue));
                        }

                        if (GetLayerWidth(i) != bottomLayerWidthProperty.intValue)
                        {
                            errorLabels.Add(GetLayerName(i) + " have incorrect width. Correct width: " + (bottomLayerWidthProperty.intValue));
                        }
                    }

                    haveOffset = (!haveOffset);
                }

                //check for element
                elementCounter = 0;
                int layerWidth;
                int layerHeight;
                int effectValue;
                bool haveEffects = false;
                bool layerIsEmpty;

                for (int i = 0; i < layersProperty.arraySize; i++)
                {
                    layerWidth = GetLayerWidth(i);
                    layerHeight = GetLayerHeight(i);
                    layerIsEmpty = true;

                    for (int x = 0; x < layerWidth; x++)
                    {
                        for (int y = 0; y < layerHeight; y++)
                        {
                            if (GetCellValue(i, x, y))
                            {
                                elementCounter++;
                                layerIsEmpty = false;
                            }

                            effectValue = GetCellEffect(i, x, y);

                            if (effectValue > 0)
                            {
                                effectsCounter[effectValue]++;
                                haveEffects = true;
                            }
                        }
                    }

                    if (layerIsEmpty)
                    {
                        errorLabels.Add($"Layer {i + 1} is empty.");
                    }
                }

                if (elementCounter % 3 != 0)
                {
                    if (elementCounter % 3 == 1)
                    {
                        errorLabels.Add("Incorrect tiles amount.Need 2 more to create a full set.");
                    }
                    else
                    {
                        errorLabels.Add("Incorrect tiles amount.Need 1 more to create a full set.");
                    }
                }

                editorNoteProperty.stringValue = " sets: " + SetsAmount;

                if (haveEffects)
                {
                    effectsLabel = "Effects: ";
                    bool firstElement = true;

                    for (int i = 0; i < effectsCounter.Length; i++)
                    {
                        if (effectsCounter[i] > 0)
                        {
                            if (firstElement)
                            {
                                editorNoteProperty.stringValue += ",effects: " + Enum.GetName(typeof(TileEffectType), i).ToLower();
                                effectsLabel += Enum.GetName(typeof(TileEffectType), i) + " x" + effectsCounter[i];
                                firstElement = false;
                            }
                            else
                            {
                                editorNoteProperty.stringValue += ", " + Enum.GetName(typeof(TileEffectType), i).ToLower();
                                effectsLabel += ", " + Enum.GetName(typeof(TileEffectType), i) + " x" + effectsCounter[i];
                            }

                        }
                    }

                    effectsLabel += ".";

                }
                else
                {
                    effectsLabel = string.Empty;
                }

                ApplyChanges(); // Save editorNoteProperty

            }

            public int GetLayerHeight(int layerIndex)
            {
                return layersProperty.GetArrayElementAtIndex(layerIndex).FindPropertyRelative(ROWS_PROPERTY_NAME).arraySize;
            }

            public int GetLayerWidth(int layerIndex)
            {
                if (layersProperty.GetArrayElementAtIndex(layerIndex).FindPropertyRelative(ROWS_PROPERTY_NAME).arraySize > 0)
                {
                    return layersProperty.GetArrayElementAtIndex(layerIndex).FindPropertyRelative(ROWS_PROPERTY_NAME).GetArrayElementAtIndex(0).FindPropertyRelative(CELLS_PROPERTY_NAME).arraySize;
                }
                else
                {
                    return 0;
                }
            }

            public string GetLayerName(int layerIndex)
            {
                return String.Format("Layer # {0} ({1}x{2})", layerIndex + 1, GetLayerWidth(layerIndex), GetLayerHeight(layerIndex));
            }

            public void AddLayer()
            {
                layersProperty.InsertArrayElementAtIndex(0);

                SetCorrectLevelSize(0);
                int layerHeight = GetLayerHeight(0);
                int layerWidth = GetLayerWidth(0);

                for (int i = 0; i < layerWidth; i++)
                {
                    for (int j = 0; j < layerHeight; j++)
                    {
                        SetCellValue(0, i, j, false);
                    }
                }
            }

            public void AddLayer(int offset)
            {
                layersProperty.InsertArrayElementAtIndex(0);

                int correctWidth = bottomLayerWidthProperty.intValue + offset;
                int correctHeight = bottomLayerHeightProperty.intValue + offset;

                for (int i = 0; i < correctHeight; i++)
                {
                    layersProperty.GetArrayElementAtIndex(0).FindPropertyRelative(ROWS_PROPERTY_NAME).arraySize = correctHeight;
                    layersProperty.GetArrayElementAtIndex(0).FindPropertyRelative(ROWS_PROPERTY_NAME).GetArrayElementAtIndex(i).FindPropertyRelative(CELLS_PROPERTY_NAME).arraySize = correctWidth;
                }

                for (int i = 0; i < correctWidth; i++)
                {
                    for (int j = 0; j < correctHeight; j++)
                    {
                        SetCellValue(0, i, j, false);
                    }
                }
            }

            public void TrimLevel()
            {
                if(layersProperty.arraySize == 0)
                {
                    Debug.LogError("Attempt to trim level with no layers");
                    return;
                }


                int layer0Width = GetLayerWidth(0);
                int layer0Height = GetLayerHeight(0);

                int layer1Width = 0;
                int layer1Height = 0;

                if(layersProperty.arraySize > 1)
                {
                    layer1Width = GetLayerWidth(1);
                    layer1Height = GetLayerHeight(1);
                }


                int maxLayerWidth = Mathf.Max(layer0Width, layer1Width);
                int maxLayerHeight = Mathf.Max(layer0Height, layer1Height);

                TintMap tintMap = new TintMap(maxLayerWidth, maxLayerHeight);

                int layerWidth;
                int layerHeight;

                for (int layerIndex = layersProperty.arraySize - 1; layerIndex >= 0; layerIndex--)
                {
                    layerWidth = GetLayerWidth(layerIndex);
                    layerHeight = GetLayerHeight(layerIndex);

                    for (int x = 0; x < layerWidth; x++)
                    {
                        for (int y = 0; y < layerHeight; y++)
                        {
                            if (GetCellValue(layerIndex, x, y))
                            {
                                if (layerHeight == maxLayerHeight)
                                {
                                    tintMap.MarkPositionInOuterLayer(x, y);
                                }
                                else
                                {
                                    tintMap.MarkPositionInInnerLayer(x, y);
                                }
                            }
                        }
                    }
                }

                int emptyMapRowsTop = 0;
                int emptyMapColumsLeft = 0;
                int emptyMapRowsBottom = 0;
                int emptyMapColumsRight = 0;

                bool isEmpty = true;

                for (int x = 0; x < (tintMap.maxLayerWidth * 2) && isEmpty; x++)
                {
                    for (int y = 0; y < (tintMap.maxLayerHeight * 2) && isEmpty; y++)
                    {
                        if (tintMap.map[x, y])
                        {
                            isEmpty = false;
                        }
                    }

                    if (isEmpty)
                    {
                        emptyMapColumsLeft++;
                    }
                }


                isEmpty = true;

                for (int y = 0; y < (tintMap.maxLayerHeight * 2) && isEmpty; y++)
                {
                    for (int x = 0; x < (tintMap.maxLayerWidth * 2) && isEmpty; x++)
                    {
                        if (tintMap.map[x, y])
                        {
                            isEmpty = false;
                        }
                    }

                    if (isEmpty)
                    {
                        emptyMapRowsTop++;
                    }
                }

                isEmpty = true;

                for (int x = (tintMap.maxLayerWidth * 2) - 1; (x >= 0) && isEmpty; x--)
                {
                    for (int y = 0; y < (tintMap.maxLayerHeight * 2) && isEmpty; y++)
                    {
                        if (tintMap.map[x, y])
                        {
                            isEmpty = false;
                        }
                    }


                    if (isEmpty)
                    {
                        emptyMapColumsRight++;
                    }
                }

                isEmpty = true;

                for (int y = (tintMap.maxLayerHeight * 2) - 1; (y >= 0) && isEmpty; y--)
                {
                    for (int x = 0; x < (tintMap.maxLayerWidth * 2) && isEmpty; x++)
                    {
                        if (tintMap.map[x, y])
                        {
                            isEmpty = false;
                        }
                    }


                    if (isEmpty)
                    {
                        emptyMapRowsBottom++;
                    }
                }

                int widthOffset;
                int heightOffset;
                int xOffset;
                int yOffset;

                bool needToChangeHeight = (emptyMapRowsTop > 1) || (emptyMapRowsBottom > 1);
                bool needToChangedWidth = (emptyMapColumsRight > 1) || (emptyMapColumsLeft > 1);

                if ((!needToChangedWidth) && (!needToChangeHeight))
                {
                    Debug.LogWarning("Nothing to trim");
                    return;
                }

                xOffset = emptyMapColumsLeft / 2;
                yOffset = emptyMapRowsBottom / 2;
                widthOffset = xOffset + emptyMapColumsRight / 2;
                heightOffset = yOffset + emptyMapRowsTop / 2;

                if((widthOffset >= maxLayerWidth) || (heightOffset >= maxLayerHeight))
                {
                    Debug.LogError("Attempt to trim empty level");
                    return;
                }

                bool cellValue;
                int effectValue;

                for (int layerIndex = 0; layerIndex < layersProperty.arraySize; layerIndex++)
                {
                    layerWidth = GetLayerWidth(layerIndex);
                    layerHeight = GetLayerHeight(layerIndex);

                    for (int y = 0; y + yOffset < layerHeight; y++)
                    {
                        for (int x = 0; x + xOffset < layerWidth; x++)
                        {
                            cellValue = GetCellValue(layerIndex, x + xOffset, y + yOffset);
                            effectValue = GetCellEffect(layerIndex, x + xOffset, y + yOffset);
                            SetCellValue(layerIndex, x + xOffset, y + yOffset, false);
                            SetCellEffect(layerIndex, x + xOffset, y + yOffset, 0);
                            SetCellValue(layerIndex, x, y, cellValue);
                            SetCellEffect(layerIndex, x, y, effectValue);
                        }

                        if (needToChangedWidth)
                        {
                            layersProperty.GetArrayElementAtIndex(layerIndex).FindPropertyRelative(ROWS_PROPERTY_NAME).GetArrayElementAtIndex(y).FindPropertyRelative(CELLS_PROPERTY_NAME).arraySize = layerWidth - widthOffset;
                        }
                    }

                    if (needToChangeHeight)
                    {
                        layersProperty.GetArrayElementAtIndex(layerIndex).FindPropertyRelative(ROWS_PROPERTY_NAME).arraySize = layerHeight - heightOffset;
                    }
                }

                bottomLayerWidthProperty.intValue = GetLayerWidth(layersProperty.arraySize - 1);
                bottomLayerHeightProperty.intValue = GetLayerHeight(layersProperty.arraySize - 1);
            }
        }
    }
}

// -----------------
// 2d grid level editor
// -----------------

// Changelog
// v 1.2
// • Reordered some methods
// v 1.1
// • Added global validation
// • Added validation example
// • Fixed mouse click bug
// v 1 basic version works
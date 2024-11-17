using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UltimateSlider
{
    
public class SliderManager : MonoBehaviour
{
    #region Public Variables

    public bool dontDestroyOnLoadFlag = false; // set true if u want the slider to not be destroyed while changing the scenes
    public bool dontDestroyOnLoadCameraFlag = false; // set true if u want the Camera to not be destroyed while changing the scenes
    public MovementType movementType = MovementType.easeInOutSine; // the type of the slider mainmovement
    public EndPageChangeType endSliderChangeType = EndPageChangeType.continuous; // how should slider do when it finish the slider
    public ControlType controlType = ControlType.Automatically; // how to control the slider
    public CameraType cameraType = CameraType.Orthographic_2D; // the type of the camera
    public ShowOnTextTypes showOnTextTypes = ShowOnTextTypes.Type1; // should the position of the slider be show on a text
    public AutomaticType automaticType = AutomaticType.Ping_Pong; // type of the automatic movement of the slider
    public animationSelection animationSelection = animationSelection.None; // the animation selection for the childs
    public animationSelection preAnimationSelection = animationSelection.None; // the animation selection for pre slider
    public animationSelection afterAnimationSelection = animationSelection.None; // the animation selection for after slider
    public AnimationType animationType = AnimationType.linear; // animation type for the childs to be animated with
    public Vector3 animationDirection = new Vector3(0.5f , 0.5f , 0.5f); // the animation direction for the slides
    public animationTime animationTime = animationTime.Specified; // when the animation on the slides should happen
    public animationAlwaysTimeFormat animationAlwaysTimeFormat = animationAlwaysTimeFormat.pingPong; // the format of the animation when its always animating
    public float alwaysanimationTimeDelay = 0f; // the delay between playing animations in always mode
    public float animationEffectSpeed = 2; // the speed effect of the animation on the slides
    public Camera cam; // the camera that shows the slider
    public bool ignoreTimeScale; // should the slider ignore time scale or not
    public float delay = 0; // delay between each slider movement
    public float speed = 50; // the speed of the main slider movement
    public bool disableInActiveSlides; // should the in-active slidedes be disabled or not
    public Image positiveArrowObject; // the ui image for positive movement
    public Image negativeArrowObject; // the ui image for negative movement
    public string positiveKey = "d"; // the key for positive movement
    public string negativeKey = "a";// the key for negative movement
    public List<Image> buttonList = new List<Image>();// buttons for slider movement
    public Vector3 sliderMoveRange = new Vector3(15, 0 , 0);// the range of the slider movement
    public bool swipeEnabled;// should the swaip be enabled or not
    public float dragDistanceNeed = 250;// how much we need to drag in order to do a swipe
    public float sliderMoveTimeNeed = 0.3f;//show the time we need to do a swipe
    public int currentSliderNumber;// the current number of the slider 
    public int preSliderNumber;// the pre number of the slider
    public int sliderPagesCount;//toatl amount of slider childs
    public Text showPageNumberText;// the text to show the slider number
    public Scrollbar scrollBar;// the scroll bar to controll the slider
    public Transform sliderParent;// slider parent to move
    public bool saveSliderPosition;// should the slider position be saved 
    public bool fastLoading;// load fast if we are loding slider ? 
    public Vector2 automaticTimeToChangeSlide = new Vector2(1, 3); // times the slider should move automatcially
    public bool sliding = false; // is slider moving or not ?
    public int startSliderNumber; // the number that sldier started with
    public int movemetnCount;// total amount of slider movements
    public float totalTimeSliding;// total time passed sliding 
    public bool showOnHandles; // should the slider position show on handles
    public bool handleCanMoveSlider; // can handles move slider or not
    public List<Image> HandlesList = new List<Image>();// handles images to show the slider position
    public Sprite activeHandelSprite; // sprite for when handle is active 
    public Sprite disttActiveHandelSprite;// sprite for when handle is in-active 

    [System.Serializable]
    public class ExampleEvent : UnityEvent { } // a sample unity event class
    public ExampleEvent OnStartSliding = new ExampleEvent(); // events when the slider start moving
    public ExampleEvent OnUpdateSliding = new ExampleEvent(); // events when the slider is moving 
    public ExampleEvent OnEndSliding = new ExampleEvent(); // events when the slider end moving
    public ExampleEvent OnSliderFirstSlide = new ExampleEvent(); // events when the slider do the first slide moving
    public ExampleEvent OnSliderLastSlide = new ExampleEvent(); // events when the slider do the last slide moving
    public List<SlideEventClass> slideEventClass = new List<SlideEventClass>();//Class for events specialy for each slid
    public List<Transform> parentsofSlides = new List<Transform>(); // all the childs parents slides holded here
    public List<Transform> objectsofSlides = new List<Transform>();// all the objects slides holded here
    public float resetSpeedScaleFactor = 1; // while reseting the slider how much the speed should go up
    public bool reseting = false; // is slider reseting or not
    public bool lockCameraOnSlider;// should the camera be locked on slider or not
    public Vector3 cameraOffset = new Vector3(0, 0, -10); // the offset between camera and slider
    public bool continueseModActive; // is the slider in continue more or not
    public bool premovingDirectionWasRight;// what direction was our last direction movement ( right or left )
    [Range(0,1)]
    public float selectedCurrentValueOfSliding = 1; // when the animation should happen while sliding 
    #endregion

    #region Private Variables
    Vector3 startClickOrTouchPos; // the position of the start touch or click
    Vector3 endClickOrTouchPos;// the position of the end touch or click
    float sliderMoveTimeNeedOnClickOrTouchTemp; 
    int currentParentChildAmount;
    bool selectedAutomaticSlideChangeTime = false;
    float selectedAutomaticSlideChangeTimeValue;
    float automaticTimerTemp;
    bool automaticPingPongIncreasing = true;
    float tempPreResetSpeed;
    Vector3 sliderParentStartPosition;
    bool specifiedAnimationActive;
    float currentValueOfSliding;
    bool createdspecifeActivation;
    #endregion

    #region Unity Functions
    
    void Start()
    {

        // check if slider parent is null or dont have enough childs
        if (sliderParent == null || sliderParent.childCount < 2) 
        {
            Debug.LogError("Slider Error : Please Assing a parent to the slider" , this);
            enabled = false;
            return;
        }

        // check if we need to set dont destroy on load
        if (dontDestroyOnLoadFlag)
        {
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(sliderParent.gameObject);

            if (cam != null && dontDestroyOnLoadCameraFlag)
            {
                DontDestroyOnLoad(cam.gameObject); 
            }
        }
        
        //assign the slider total counts 
        sliderPagesCount = sliderParent.childCount;

        // change slider position
        sliderParentStartPosition = sliderParent.position;
        
        //calculate the speed to be modified based on the slider move range
        speed = ((Mathf.Abs(sliderMoveRange.x) + Mathf.Abs(sliderMoveRange.y) + Mathf.Abs(sliderMoveRange.z))) * speed * Time.deltaTime;

        //hold start speed value for later use
        tempPreResetSpeed = speed;

        // set slider childs positions
        for (int i = 0; i < sliderParent.childCount; i++)
        {

            if (sliderParent.GetChild(i).GetComponent<RectTransform>() != null)
            {

                sliderParent.GetChild(i).GetComponent<RectTransform>().anchorMin = sliderParent.GetComponent<RectTransform>().anchorMin;
                sliderParent.GetChild(i).GetComponent<RectTransform>().anchorMax = sliderParent.GetComponent<RectTransform>().anchorMax;
                sliderParent.GetChild(i).GetComponent<RectTransform>().anchoredPosition = sliderParent.GetComponent<RectTransform>().anchoredPosition;
                sliderParent.GetChild(i).GetComponent<RectTransform>().sizeDelta = sliderParent.GetComponent<RectTransform>().sizeDelta;
                sliderParent.GetChild(i).GetComponent<RectTransform>().offsetMin = sliderParent.GetComponent<RectTransform>().offsetMin;
                sliderParent.GetChild(i).GetComponent<RectTransform>().offsetMax = sliderParent.GetComponent<RectTransform>().offsetMax;

            }

            sliderParent.GetChild(i).position =
                new Vector3(sliderParent.position.x + (sliderMoveRange.x * i)
                    , sliderParent.position.y + (sliderMoveRange.y * i), sliderParent.position.z + (sliderMoveRange.z * i));
        }

        // create slider object for each child 
        CheckChildsParents();

        // set slider childs positions again after making slider for them
        for (int i = 0; i < sliderParent.childCount; i++)
        {
            if (sliderParent.GetChild(i).GetComponent<RectTransform>() != null)
            {

                sliderParent.GetChild(i).GetComponent<RectTransform>().anchorMin = sliderParent.GetComponent<RectTransform>().anchorMin;
                sliderParent.GetChild(i).GetComponent<RectTransform>().anchorMax = sliderParent.GetComponent<RectTransform>().anchorMax;
                sliderParent.GetChild(i).GetComponent<RectTransform>().anchoredPosition = sliderParent.GetComponent<RectTransform>().anchoredPosition;
                sliderParent.GetChild(i).GetComponent<RectTransform>().sizeDelta = sliderParent.GetComponent<RectTransform>().sizeDelta;
                sliderParent.GetChild(i).GetComponent<RectTransform>().offsetMin = sliderParent.GetComponent<RectTransform>().offsetMin;
                sliderParent.GetChild(i).GetComponent<RectTransform>().offsetMax = sliderParent.GetComponent<RectTransform>().offsetMax;

            }

            sliderParent.GetChild(i).position =
                              new Vector3(sliderParent.position.x + (sliderMoveRange.x * i)
                              , sliderParent.position.y + (sliderMoveRange.y * i), sliderParent.position.z + (sliderMoveRange.z * i));
        }

        //disable the sliders out side of the focuse if needed
        if ( disableInActiveSlides)
         CheckToDisableOutsidePages();
     
        // set camera type 
        if (cam != null)
        { 
            if (cameraType == CameraType.Orthographic_2D)
        {
             cam.orthographic = true;
        }
        else
        {
             cam.orthographic = false;
        }
        }

        // if we selected the arrow control type
        if (controlType == ControlType.Arrows)  
        {

            if (positiveArrowObject == null || negativeArrowObject == null)
            {
                Debug.LogError(" #Slider Error# :Please add two ui images for arrow controllers !");
                enabled = false;
                return;
            }

            // assign buttons for the slider  arrows

            Button PositiveButton;


            if (positiveArrowObject.GetComponent<Button>() == null)
            {
                PositiveButton = positiveArrowObject.gameObject.AddComponent<Button>();
            }
            else
            {
                PositiveButton = positiveArrowObject.GetComponent<Button>();
            }

            PositiveButton.onClick.AddListener(() => MoveSliderToRightOrLeft(true));


            Button NegativeButton;


            if (negativeArrowObject.GetComponent<Button>() == null)
            {
                NegativeButton = negativeArrowObject.gameObject.AddComponent<Button>();
            }
            else
            {
                NegativeButton = negativeArrowObject.GetComponent<Button>();
            }

            NegativeButton.onClick.AddListener(() => MoveSliderToRightOrLeft(false));


            // check if arrows need to be diss active
            if (endSliderChangeType == EndPageChangeType.LockAtTheEnd)
                CheckSliderArrows();
        }  
        // if we selected the keyboard control type
        else if (controlType == ControlType.KeyBoard)  
        {
            if (positiveKey == System.String.Empty || negativeKey == System.String.Empty)
            {
                Debug.LogError(" #Slider Error# :Please add two keyboard key for slider controllers !");
                enabled = false;
                return;
            }
        }
        // if we selected the Buttons control type
        else if (controlType == ControlType.Buttons)  
        {
            for (int i = 0; i < buttonList.Count; i++)
            {
                if (buttonList[i] == null)
                {
                    Debug.LogError(" #Slider Error# :Please add all images as needed for Button controllers !");
                    enabled = false;
                    break;
                }

                // assign button component for the slider  Buttons
                Button ThisButton = null;

                if (buttonList[i].GetComponent<Button>() == null)
                {
                    ThisButton = buttonList[i].gameObject.AddComponent<Button>();

                }
                else
                {
                    ThisButton = buttonList[i].gameObject.GetComponent<Button>();
                }
                int Number = i;
                ThisButton.onClick.AddListener(() => MoveSliderToSelectedPage(Number));
            }

        }
        // if we selected the scroll control type
        else if (controlType == ControlType.Scroll)
        {

            if (scrollBar == null)
            {
                Debug.LogError(" #Slider Error# :Please assign a Scroll Bar !");
                enabled = false;
            }

            scrollBar.onValueChanged.AddListener(delegate { OnScrollValueChange(); });
        }
         //try to load slider position if we have save enabled
        if (saveSliderPosition && PlayerPrefs.HasKey("Slider_Current_Position_Value" + gameObject.GetInstanceID()))
        {
            // check if we have a value and load it
            preSliderNumber = currentSliderNumber;
            currentSliderNumber = PlayerPrefs.GetInt("Slider_Current_Position_Value" + gameObject.GetInstanceID());
            if(fastLoading)
            {
                sliderParent.position = new Vector3(sliderParent.position.x + (currentSliderNumber * -sliderMoveRange.x)
                    , sliderParent.position.y + ( currentSliderNumber * -sliderMoveRange.y), sliderParent.position.z + (currentSliderNumber * -sliderMoveRange.z));
            }
            
        }
        // load default slider position selected in the main part
        else
        { 
            preSliderNumber = currentSliderNumber;
            currentSliderNumber = startSliderNumber;
            sliderParent.position = new Vector3(sliderParent.position.x + (currentSliderNumber * -sliderMoveRange.x)
                , sliderParent.position.y + (currentSliderNumber * -sliderMoveRange.y), sliderParent.position.z + (currentSliderNumber * -sliderMoveRange.z));

        }

        // check if we need handles
        if(showOnHandles)
        {
            bool allHandlesAreSet = true;
            for (int i = 0; i < HandlesList.Count ; i++)
            {
                if ( HandlesList[i] == null)
                {
                    Debug.LogError("Slider Error : please assing UI images as your slider Handles");
                    allHandlesAreSet = false;
                }
            }

            if(!allHandlesAreSet)
            {
                this.enabled = false;
                return;
            }
        }
        // check if handles can mvoe the slider
        if(showOnHandles && handleCanMoveSlider)
        {
            for (int i = 0; i < HandlesList.Count; i++)
            {
                // assign button component for the slider  Buttons
                Button ThisButton = null;

                if (HandlesList[i].GetComponent<Button>() == null)
                {
                    ThisButton = HandlesList[i].gameObject.AddComponent<Button>();
                }
                else
                {
                    ThisButton = HandlesList[i].gameObject.GetComponent<Button>();
                }
                int Number = i;
                ThisButton.onClick.AddListener(() => MoveSliderToSelectedPage(Number));
            }
        }

        //save total amount of slider childs to use later
        currentParentChildAmount = sliderParent.childCount;
        //check to see if need to show the slider number on a text
        CheckShowOnText();
        // check if we are showing slider handles
        CheckSliderHandles();
        //move slider for the first time
        MoveSlider();
    }

    void Update()
    {
    
        // check if parent is not null
        if (sliderParent == null || sliderParent.childCount < 2)
        {
            return;
        }

        // check if we need to lock the camera
        if (lockCameraOnSlider)
        {
            cam.transform.position = sliderParentStartPosition + cameraOffset;
        }

        //things we need to do while the slider is moving goes here
        if (sliding)
        {
            // add total time sliding
            totalTimeSliding += Time.deltaTime;

            // check if we have to animate the slides
            if((movemetnCount > 1 || ( saveSliderPosition && !fastLoading)) && animationTime == animationTime.Specified && !specifiedAnimationActive && !createdspecifeActivation)
            {
                createdspecifeActivation = true;
                specifiedAnimationActive = true;
            }

            // detect distance needed to reach the final point of the movement
            float FullDistance = Vector3.Distance(
                new Vector3(
                    sliderParentStartPosition.x + (preSliderNumber * -sliderMoveRange.x),
                    sliderParentStartPosition.y + (preSliderNumber * -sliderMoveRange.y),
                    sliderParentStartPosition.z + (preSliderNumber * -sliderMoveRange.z)),
                new Vector3(
                    sliderParentStartPosition.x + (currentSliderNumber * -sliderMoveRange.x),
                    sliderParentStartPosition.y + (currentSliderNumber * -sliderMoveRange.y),
                    sliderParentStartPosition.z + (currentSliderNumber * -sliderMoveRange.z)));

            float ThisDistance = Vector3.Distance(sliderParent.position,
                new Vector3(
                    sliderParentStartPosition.x + (currentSliderNumber * -sliderMoveRange.x),
                    sliderParentStartPosition.y + (currentSliderNumber * -sliderMoveRange.y),
                    sliderParentStartPosition.z + (currentSliderNumber * -sliderMoveRange.z)));

            if(FullDistance != 0)
            { 
                currentValueOfSliding = 1 - (ThisDistance / FullDistance);
            } 
        }
        else
        {
            // disable slider animation effect
            createdspecifeActivation = false;
        }

         // check to run the animation effects on the sldie
        if (animationTime == animationTime.Specified && currentValueOfSliding >= selectedCurrentValueOfSliding)
        {
             if (specifiedAnimationActive)
            {
                 specifiedAnimationActive = false;
                RunAnimationEffectOnce(parentsofSlides[currentSliderNumber]);
            }
        }

        // if the slider is moving automatically
        if (controlType == ControlType.Automatically)
        {
            // select a time to move the slider automatically
            if (!selectedAutomaticSlideChangeTime)
            {
                selectedAutomaticSlideChangeTime = true;
                selectedAutomaticSlideChangeTimeValue = Random.Range(automaticTimeToChangeSlide.x, automaticTimeToChangeSlide.y);
            }

            // get ready to move the slider on the right time
            if (!sliding)
                automaticTimerTemp += Time.deltaTime;
            if (automaticTimerTemp >= selectedAutomaticSlideChangeTimeValue && !sliding)
            {
                automaticTimerTemp = 0;
                selectedAutomaticSlideChangeTime = false;
                // change slider automatically here
                switch (automaticType) // check the slider movement type
                {
                    case AutomaticType.Increase:

                        MoveSliderToRightOrLeft(true);

                        ; break;
                    case AutomaticType.Deacrease:

                        MoveSliderToRightOrLeft(false);

                        ; break;
                    case AutomaticType.Ping_Pong:
                        if (automaticPingPongIncreasing)
                        {
                            if ((currentSliderNumber + 2) == sliderPagesCount)
                            {
                                automaticPingPongIncreasing = false;
                            }
                            MoveSliderToRightOrLeft(true);
                        }
                        else
                        {
                            if ((currentSliderNumber - 2) == -1)
                            {
                                automaticPingPongIncreasing = true;
                            }
                            MoveSliderToRightOrLeft(false);

                        }
                        ; break;
                    case AutomaticType.Random:
                        preSliderNumber = currentSliderNumber;
                        currentSliderNumber = Random.Range(0, sliderPagesCount);
                        MoveSlider();
                        ; break;
                }

            }

        }
        // if the slider is moving with keys
        else if (controlType == ControlType.KeyBoard)
        {
           // check the keys pressed this frame
            foreach (char c in Input.inputString)
            {
       
                if (c.ToString() == positiveKey)  
                {
                    MoveSliderToRightOrLeft(true);
                    break;
                }
                else if (c.ToString() == negativeKey) 
                {
                    MoveSliderToRightOrLeft(false);
                    break;
                }
            }
        }

        // check if new slides has been added slider  
        if (currentParentChildAmount != sliderParent.childCount)
        {
            // get the new slide object ready for the slider
            currentParentChildAmount = sliderParent.childCount;
            sliderPagesCount = sliderParent.childCount;
            CheckChildsParents();
            CheckShowOnText();
            CheckSliderArrows();

         
            for (int i = 0; i < parentsofSlides.Count; i++)
            {
                parentsofSlides[i].position = new Vector3(
                    sliderParent.position.x + (sliderMoveRange.x * i)
                    , sliderParent.position.y + (sliderMoveRange.y * i)
                    , sliderParent.position.z + (sliderMoveRange.z * i));
            }

        }

        // check if swiping is enabled
        if (swipeEnabled)
        {
            if (sliding && endSliderChangeType == EndPageChangeType.continuous && controlType == ControlType.Arrows)
            {
                return;
            }

            // get mouse or touch position for the swup
            if (Input.GetMouseButtonDown(0))
            {
                startClickOrTouchPos = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                sliderMoveTimeNeedOnClickOrTouchTemp += Time.deltaTime;
                endClickOrTouchPos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            { 
                if (sliderMoveTimeNeedOnClickOrTouchTemp < sliderMoveTimeNeed)
                {
                    endClickOrTouchPos = Input.mousePosition;

                    if (Mathf.Abs(startClickOrTouchPos.x - endClickOrTouchPos.x) > dragDistanceNeed)
                    {
                        // move page 
                        if (startClickOrTouchPos.x > endClickOrTouchPos.x)
                        {
                            // move left
                            MoveSliderToRightOrLeft(false);
                        }
                        else
                        {
                            // move right
                            MoveSliderToRightOrLeft(true);
                        }
                    }

                }

                sliderMoveTimeNeedOnClickOrTouchTemp = 0;
            }
        }


    }
    
    #endregion

    #region Main Slider Functions
     // check to see if any childs of the slider need to have sliders
    public void CheckChildsParents()
    {

        // check wrong parent
        for (int i = 0; i < parentsofSlides.Count; i++)
        {
            if (parentsofSlides[i].parent != sliderParent)
            {
                parentsofSlides.RemoveAt(i);
                i = -1;
            }
        }

       // add the slider for the child
        for (int i = 0; i < sliderParent.childCount; i++)
        {
            if (!objectsofSlides.Contains(sliderParent.GetChild(i)) && !parentsofSlides.Contains(sliderParent.GetChild(i)))
            {
                objectsofSlides.Add(sliderParent.GetChild(i));
            }
        }
      
        //change amount of the parent slider list based on the childs

        while (parentsofSlides.Count < objectsofSlides.Count)
        {
            parentsofSlides.Add(null);
        }
       
        while (parentsofSlides.Count > objectsofSlides.Count)
        {
            parentsofSlides.RemoveAt(parentsofSlides.Count - 1);
        }
     
         // create sliders for the child that dont have it 
        for (int i = 0; i < parentsofSlides.Count; i++)
        {
            if (parentsofSlides[i] == null)
            {

                GameObject newParentObject = new GameObject();

                if (objectsofSlides[i].GetComponent<RectTransform>() != null)
                {
                    newParentObject.AddComponent<RectTransform>();
                    newParentObject.GetComponent<RectTransform>().anchorMin = objectsofSlides[i].GetComponent<RectTransform>().anchorMin;
                    newParentObject.GetComponent<RectTransform>().anchorMax = objectsofSlides[i].GetComponent<RectTransform>().anchorMax;
                    newParentObject.GetComponent<RectTransform>().anchoredPosition = objectsofSlides[i].GetComponent<RectTransform>().anchoredPosition;
                    newParentObject.GetComponent<RectTransform>().sizeDelta = objectsofSlides[i].GetComponent<RectTransform>().sizeDelta;
                    newParentObject.GetComponent<RectTransform>().offsetMin = objectsofSlides[i].GetComponent<RectTransform>().offsetMin;
                    newParentObject.GetComponent<RectTransform>().offsetMax = objectsofSlides[i].GetComponent<RectTransform>().offsetMax;

                }
                else
                {
                    newParentObject.transform.position = objectsofSlides[i].position;
                    
                }
                newParentObject.transform.SetParent(sliderParent, false);
                objectsofSlides[i].SetParent(newParentObject.transform, true);

                if (objectsofSlides[i].GetComponent<RectTransform>() != null)
                {

                    objectsofSlides[i].GetComponent<RectTransform>().anchorMin = newParentObject.GetComponent<RectTransform>().anchorMin;
                    objectsofSlides[i].GetComponent<RectTransform>().anchorMax = newParentObject.GetComponent<RectTransform>().anchorMax;
                    objectsofSlides[i].GetComponent<RectTransform>().anchoredPosition = newParentObject.GetComponent<RectTransform>().anchoredPosition;
                    objectsofSlides[i].GetComponent<RectTransform>().sizeDelta = newParentObject.GetComponent<RectTransform>().sizeDelta;
                    objectsofSlides[i].GetComponent<RectTransform>().offsetMin = Vector2.zero;
                    objectsofSlides[i].GetComponent<RectTransform>().offsetMax = Vector2.zero;

                }
                else
                {
                    objectsofSlides[i].localPosition = Vector3.zero;
                }


                newParentObject.name = "Slide " + i;
                parentsofSlides[i] = newParentObject.transform;
            }
        }
         
        //remove the un un used childs
        for (int i = 0; i < objectsofSlides.Count; i++)
        {
            if (objectsofSlides[i].parent.parent != sliderParent)
            {
                objectsofSlides.RemoveAt(i);
                i = -1;
            }
        }

        // check for always animation effects to be done here
        if (animationSelection != animationSelection.None)
        {
            if (animationTime == animationTime.Always)
            {
                for (int i = 0; i < parentsofSlides.Count; i++)
                {
                    if(parentsofSlides[i].GetComponent<SliderTween>() == null)
                    {
                        RunAnimationEffectforAlways(parentsofSlides[i]);
                    }
                }
            }
        }
    }
    // we move the slider here 
    public void MoveSliderToRightOrLeft(bool IsRight)
    {
        // move slider to positive value
        if (IsRight)
        {
            preSliderNumber = currentSliderNumber;
            currentSliderNumber++;
        }
        // move slider to negative value
        else
        {
            preSliderNumber = currentSliderNumber;
            currentSliderNumber--;
        }

        // check to luck the slider
        if (endSliderChangeType == EndPageChangeType.LockAtTheEnd)
        {
            if (IsRight)
            {
                if (currentSliderNumber == sliderPagesCount)
                {
                    preSliderNumber = currentSliderNumber;
                    currentSliderNumber = sliderPagesCount - 1;
                }
            }
            else
            {
                if (currentSliderNumber == -1)
                {
                    preSliderNumber = currentSliderNumber;
                    currentSliderNumber = 0;

                }
            }
            CheckSliderArrows();
        }
        // check to reset the slider
        else if (endSliderChangeType == EndPageChangeType.Reset)
        {
            if (IsRight)
            {
                if (currentSliderNumber == sliderPagesCount)
                {
                    preSliderNumber = currentSliderNumber;
                    currentSliderNumber = 0;

                    reseting = true;

                    speed = tempPreResetSpeed * resetSpeedScaleFactor;
                }
                else if (reseting)
                {

                    reseting = false;
                    speed = tempPreResetSpeed;

                }
            }
            else
            {
                if (currentSliderNumber == -1)
                {
                    preSliderNumber = currentSliderNumber;
                    currentSliderNumber = sliderPagesCount - 1;

                    reseting = true;

                    speed = tempPreResetSpeed * resetSpeedScaleFactor;
                }
                else if (reseting)
                {
                    speed = tempPreResetSpeed;
                    reseting = false;
                }
            }

        }
        // check to continue the slider
        else if (endSliderChangeType == EndPageChangeType.continuous)
        {
            // move right
            if (IsRight)  
            {
                if (continueseModActive)
                {
                    if (premovingDirectionWasRight == true)
                    {
 
                        continueseModActive = false;
                      
                        parentsofSlides[parentsofSlides.Count - 1].transform.position = new Vector3(
                            sliderParent.position.x + ((-sliderPagesCount + 1) * -sliderMoveRange.x)
                          , sliderParent.position.y + ((-sliderPagesCount + 1) * -sliderMoveRange.y)
                            , sliderParent.position.z + ((-sliderPagesCount + 1) * -sliderMoveRange.z));
                    }
                    else
                    {
                       
                        parentsofSlides[0].transform.position = new Vector3(sliderParent.position.x + ((sliderPagesCount - 1) * -sliderMoveRange.x)
                    , sliderParent.position.y + ((sliderPagesCount - 1) * -sliderMoveRange.y), sliderParent.position.z + ((sliderPagesCount - 1) * -sliderMoveRange.z));
                    }
                }

                if (currentSliderNumber == sliderPagesCount)
                {
                
                    preSliderNumber = currentSliderNumber;
                    currentSliderNumber = 0;


                    continueseModActive = true;
                  
                  //  sliding = false;
                    if (sliderParent.GetComponent<SliderTween>())
                    {
                        sliderParent.GetComponent<SliderTween>().isRunning = false;
                        Destroy(sliderParent.GetComponent<SliderTween>());
                    }
                    // Slider returns to first position 
                    Vector3 preContinuePos = new Vector3(
                        sliderParentStartPosition.x + (-1 * -sliderMoveRange.x)
                        , sliderParentStartPosition.y + (-1 * -sliderMoveRange.y),
                        sliderParentStartPosition.z + (-1 * -sliderMoveRange.z));
                  
                    sliderParent.position = preContinuePos;
                     
                    parentsofSlides[parentsofSlides.Count - 1].position = sliderParentStartPosition;

                }
            }
            else // moving left
            {
                if (continueseModActive)
                {
                    if (premovingDirectionWasRight == false)
                    {
                     
                        continueseModActive = false;
                     
                        parentsofSlides[0].transform.position = new Vector3(sliderParent.position.x 
               , sliderParent.position.y , sliderParent.position.z);
                          
                    }
                    else
                    {
                
                        parentsofSlides[parentsofSlides.Count - 1].transform.position = new Vector3(
                            sliderParentStartPosition.x + ((-sliderPagesCount + 1) * -sliderMoveRange.x)
                          , sliderParentStartPosition.y + ((-sliderPagesCount + 1) * -sliderMoveRange.y)
                            , sliderParentStartPosition.z + ((-sliderPagesCount + 1) * -sliderMoveRange.z));

                    }
                }

                if (currentSliderNumber == -1)
                {
                 

                    preSliderNumber = currentSliderNumber;
                    currentSliderNumber = sliderPagesCount - 1;
 
                    continueseModActive = true; 

                    if (sliderParent.GetComponent<SliderTween>())
                    {
                    sliderParent.GetComponent<SliderTween>().isRunning = false;
                    Destroy(sliderParent.GetComponent<SliderTween>());
                    }
                    // Slider goes to one point to the las 
                    Vector3 preContinuePos = new Vector3(
                        sliderParentStartPosition.x + ((sliderPagesCount) * -sliderMoveRange.x)
                        , sliderParentStartPosition.y + ((sliderPagesCount) * -sliderMoveRange.y)
                        , sliderParentStartPosition.z + ((sliderPagesCount) * -sliderMoveRange.z));
                  
                    sliderParent.position = preContinuePos;
                  
                 
                    parentsofSlides[0].transform.position = sliderParentStartPosition;
                 
                }
            }

        }


        // set last movement direction
        if (IsRight)
            premovingDirectionWasRight = true;
        else
            premovingDirectionWasRight = false;
         
        // move the slider
        MoveSlider();
    }
    // move the slider based on scroll value change
    public void OnScrollValueChange()
    {
        preSliderNumber = currentSliderNumber;
        currentSliderNumber = (int)(Mathf.Lerp(0, sliderPagesCount - 1, scrollBar.value));
        MoveSlider();
    }
    // disable the slides that are in-actve
    public void CheckToDisableOutsidePages()
    {
        for (int i = 0; i < sliderParent.childCount; i++)
        {
            if (i == currentSliderNumber)
            {
                parentsofSlides[i].gameObject.SetActive(true);
            }
            else
            {
                parentsofSlides[i].gameObject.SetActive(false);
            }
        }
    }
    //save the slider value if needed
    public void CheckSavingSliderValue()
    {
        if (saveSliderPosition)
        {
            PlayerPrefs.SetInt("Slider_Current_Position_Value" + gameObject.GetInstanceID(), currentSliderNumber);
        }
    }
    //active or diss-acrtive slider arrows
    public void CheckSliderArrows()
    {
        if (controlType != ControlType.Arrows)
            return;


        // arrow left
        if (currentSliderNumber == 0)
        {
            negativeArrowObject.gameObject.SetActive(false);
        }
        else
        {
            negativeArrowObject.gameObject.SetActive(true);
        }


        //arrow right
        if (currentSliderNumber == sliderPagesCount - 1)
        {
            positiveArrowObject.gameObject.SetActive(false);
        }
        else
        {
            positiveArrowObject.gameObject.SetActive(true);
        }

    }
     //this function move the slider
    public void MoveSlider() 
    {
        movemetnCount++;

        if (disableInActiveSlides)
        {
            parentsofSlides[currentSliderNumber].gameObject.SetActive(true);
        }
    
        CheckShowOnText();
        CheckSliderHandles();
        Vector3 TargetPos = new Vector3(
            sliderParentStartPosition.x + (currentSliderNumber * -sliderMoveRange.x),
            sliderParentStartPosition.y + (currentSliderNumber * -sliderMoveRange.y)
            , sliderParentStartPosition.z + (currentSliderNumber * -sliderMoveRange.z));
    

        
        if(movementType == MovementType.Immediate)
        {
            sliderParent.position = TargetPos;
        }
        else
        {  
            SliderTween.MoveTo(sliderParent.gameObject, SliderTween.Set("position", TargetPos, "easeType", movementType.ToString(), "loopType", "none", "delay", delay,
           "speed", speed, "onstart", "OnStartEventStarter", "onstarttarget", gameObject, "oncomplete", "OnEndEventStarter", "oncompletetarget", gameObject,
           "onupdate", "OnUpdateventStarter", "onupdatetarget", gameObject));
        }
    }
    //this function aimate the slides for ever
    public void RunAnimationEffectforAlways(Transform objectToAnimate)
    { 
        // check selected animation
        switch (animationSelection)
        {
            case animationSelection.ShortScaleUpdate:
                SliderTween.ScaleAdd(objectToAnimate.gameObject, SliderTween.Set("amount", animationDirection, "easeType", animationType.ToString(), "loopType",
                    animationAlwaysTimeFormat.ToString(), "delay", alwaysanimationTimeDelay, "speed", animationEffectSpeed));
                break;
            case animationSelection.ShortRotateUpdate:
                SliderTween.RotateAdd(objectToAnimate.gameObject, SliderTween.Set("amount", animationDirection, "easeType", animationType.ToString(), "loopType",
                    animationAlwaysTimeFormat.ToString(), "delay", alwaysanimationTimeDelay, "speed", animationEffectSpeed));
                break;
            case animationSelection.ShortPositionUpdate:
                SliderTween.MoveAdd(objectToAnimate.gameObject, SliderTween.Set("amount", animationDirection, "easeType", animationType.ToString(), "loopType",
                    animationAlwaysTimeFormat.ToString(), "delay", alwaysanimationTimeDelay, "speed", animationEffectSpeed));
                break;
            case animationSelection.ImpulseScale:
                SliderTween.PunchScale(objectToAnimate.gameObject, SliderTween.Set("amount", animationDirection, "easeType", animationType.ToString(), "loopType",
                    animationAlwaysTimeFormat.ToString(), "delay", alwaysanimationTimeDelay, "speed", animationEffectSpeed));
                break;
            case animationSelection.ImpulseRotate:
                SliderTween.PunchRotation(objectToAnimate.gameObject, SliderTween.Set("amount", animationDirection, "easeType", animationType.ToString(), "loopType",
                    animationAlwaysTimeFormat.ToString(), "delay", alwaysanimationTimeDelay, "speed", animationEffectSpeed));
                break;
            case animationSelection.ImpulsePosition:
                SliderTween.PunchPosition(objectToAnimate.gameObject, SliderTween.Set("amount", animationDirection, "easeType", animationType.ToString(), "loopType",
                    animationAlwaysTimeFormat.ToString(), "delay", alwaysanimationTimeDelay, "speed", animationEffectSpeed));
                break;
        }
 
    }
    //this function aimate the slides in Specified times
    public void RunAnimationEffectOnce(Transform objectToAnimate)
    {

        if (objectToAnimate.GetComponent<SliderTween>())
        {
            objectToAnimate.GetComponent<SliderTween>().isRunning = false;
            Destroy(objectToAnimate.GetComponent<SliderTween>());
        }
           
        // check selected animation
        switch (animationSelection)
        {
            case animationSelection.ShortScaleUpdate:
                SliderTween.ScaleAdd(objectToAnimate.gameObject, SliderTween.Set("amount", animationDirection, "easeType", animationType.ToString(), "loopType",
                   SliderTween.LoopType.none, "delay", alwaysanimationTimeDelay, "speed", animationEffectSpeed));
                break;
            case animationSelection.ShortRotateUpdate:
                SliderTween.RotateAdd(objectToAnimate.gameObject, SliderTween.Set("amount", animationDirection, "easeType", animationType.ToString(), "loopType",
                    SliderTween.LoopType.none, "delay", alwaysanimationTimeDelay, "speed", animationEffectSpeed));
                break;
            case animationSelection.ShortPositionUpdate:
                SliderTween.MoveAdd(objectToAnimate.gameObject, SliderTween.Set("amount", animationDirection, "easeType", animationType.ToString(), "loopType",
                     SliderTween.LoopType.none, "delay", alwaysanimationTimeDelay, "speed", animationEffectSpeed));
                break;
            case animationSelection.ImpulseScale:
                SliderTween.PunchScale(objectToAnimate.gameObject, SliderTween.Set("amount", animationDirection, "easeType", animationType.ToString(), "loopType",
                   SliderTween.LoopType.none, "delay", alwaysanimationTimeDelay, "speed", animationEffectSpeed));
                break;
            case animationSelection.ImpulseRotate:
                SliderTween.PunchRotation(objectToAnimate.gameObject, SliderTween.Set("amount", animationDirection, "easeType", animationType.ToString(), "loopType",
                    SliderTween.LoopType.none, "delay", alwaysanimationTimeDelay, "speed", animationEffectSpeed));
                break;
            case animationSelection.ImpulsePosition:
                SliderTween.PunchPosition(objectToAnimate.gameObject, SliderTween.Set("amount", animationDirection, "easeType", animationType.ToString(), "loopType",
                     SliderTween.LoopType.none, "delay", alwaysanimationTimeDelay, "speed", animationEffectSpeed));
                break;
        }

    }
    // check if we are using handles to show the slider position on the screen
    public void CheckSliderHandles()
    {
        if(showOnHandles)
        {
            for (int i = 0; i < HandlesList.Count; i++)
            {
                if(i == currentSliderNumber)
                {
                    HandlesList[i].sprite = activeHandelSprite;
                }
                else
                {
                    HandlesList[i].sprite = disttActiveHandelSprite;
                }
            }
        }
    }

    #endregion

    #region Event Functions
    // starter event starter
    public void OnStartEventStarter()
    {
        currentValueOfSliding = 0;

        OnStartSliding.Invoke();

        CheckSavingSliderValue();

        sliding = true;

        SpecialSlideEventStarter(currentSliderNumber);
    }
    // end event starter
    public void OnEndEventStarter()
    {
        currentValueOfSliding = 1;

        OnEndSliding.Invoke();

        if (disableInActiveSlides)
            CheckToDisableOutsidePages();

        if (currentSliderNumber == 0)
        {
            OnSliderFirstSlide.Invoke();
        }

        if ((currentSliderNumber + 1) == sliderPagesCount)
        {
            OnSliderLastSlide.Invoke();
        }

        sliding = false;

        SpecialSlideEventEnder(currentSliderNumber);
    }
    // update event starter
    public void OnUpdateventStarter()
    {
        OnUpdateSliding.Invoke();

        SpecialSlideEventUpdater(currentSliderNumber);
    }
    //Special starter event starter
    public void SpecialSlideEventStarter(int SpecialSlideNumber)
    {
        if (slideEventClass.Count > SpecialSlideNumber && slideEventClass[SpecialSlideNumber] != null)
        {
            slideEventClass[SpecialSlideNumber].OnStartSliding.Invoke();
        }
    }
    //Special end event starter
    public void SpecialSlideEventEnder(int SpecialSlideNumber)
    {
        if (slideEventClass.Count > SpecialSlideNumber && slideEventClass[SpecialSlideNumber] != null)
        {
            slideEventClass[SpecialSlideNumber].OnEndSliding.Invoke();
        }
    }
    //Special update event starter
    public void SpecialSlideEventUpdater(int SpecialSlideNumber)
    {
        if (slideEventClass.Count > SpecialSlideNumber && slideEventClass[SpecialSlideNumber] != null)
        {
            slideEventClass[SpecialSlideNumber].OnUpdateSliding.Invoke();
        }
    }
    #endregion

    #region Help and Utility Functions
    //Add a sllider to the obejct with a game obejct
    public void AddSlideToSlilder(GameObject ObjectToAdd)
    {
        ObjectToAdd.transform.SetParent(sliderParent, true);
    }

    // moving the slider to a selected page
    public void MoveSliderToSelectedPage(int SelectedNumber)
    {
        preSliderNumber = currentSliderNumber;
        currentSliderNumber = SelectedNumber;
        MoveSlider();
    }
     
    // swap two slider position together
    public void SwapSlides(int firstSlideNumber , int secondSlideNumber , MovementType thisMovementType = MovementType.Immediate , float thisSpeed = 50)
    {
         
        Vector3 TargetPos = Vector3.zero;

        objectsofSlides[firstSlideNumber].SetParent(parentsofSlides[secondSlideNumber], true);


        objectsofSlides[secondSlideNumber].SetParent(parentsofSlides[firstSlideNumber], true);

        if (thisMovementType == MovementType.Immediate)
        {
            objectsofSlides[firstSlideNumber].localPosition = Vector3.zero;
            objectsofSlides[secondSlideNumber].localPosition = Vector3.zero;

            Transform temp = objectsofSlides[firstSlideNumber];
            objectsofSlides[firstSlideNumber] = objectsofSlides[secondSlideNumber];
            objectsofSlides[secondSlideNumber] = temp;
        }
        else
        {
 
            Transform temp = objectsofSlides[firstSlideNumber];
            objectsofSlides[firstSlideNumber] = objectsofSlides[secondSlideNumber];
            objectsofSlides[secondSlideNumber] = temp;


            SliderTween.MoveTo(objectsofSlides[firstSlideNumber].gameObject, SliderTween.Set("position", TargetPos, "easeType", thisMovementType.ToString(), "islocal", true, "speed", thisSpeed));

            SliderTween.MoveTo(objectsofSlides[secondSlideNumber].gameObject, SliderTween.Set("position", TargetPos, "easeType", thisMovementType.ToString(), "islocal", true, "speed", thisSpeed));
        }

    }

    // Remove a Child from the slider
    public GameObject RemoveSlide(int sliderNumber , bool DestorySlideGameObject)
    {
        if(sliderPagesCount > 2)
        {
        GameObject sliderObject = parentsofSlides[sliderNumber].gameObject;
        parentsofSlides.RemoveAt(sliderNumber);
        objectsofSlides.RemoveAt(sliderNumber);
        sliderPagesCount--;
        //change other sliders poses
        if(DestorySlideGameObject)
        {
            Destroy(sliderObject);
            return null;
        }
        else
        {
            return sliderObject;
        }
        }
        else
        {
            Debug.LogError("Slider Error: the slider childs cant be removed any more ! ( slider must have at least 2 childs inside)");
            return null;
        }

    }
     
    // start the slider to move automatically
    public void StartAutomaticSlider(  Vector2 WaitingDelay , AutomaticType thisAutomaticType = AutomaticType.Increase)
    {
        automaticTimeToChangeSlide = WaitingDelay;
        automaticType = thisAutomaticType;
        controlType = ControlType.Automatically;
    }
    
    // Dis-active the slider ( the game object SetActive would become false )
    public void DisActiveSlider()
    {
        sliderParent.gameObject.SetActive(false); 
        enabled = false; 
    }
    
    // Active the slider ( the game object SetActive would become true )
    // u can set resetSlider to "true" its like starting the slider again
    public void ActiveSlider(bool resetSlider = false, bool fastReset = false)
    {
        enabled = true;
        sliderParent.gameObject.SetActive(true);
        if (resetSlider)
        {
            LoadSliderPosition(fastReset);
        }
    }
    
    // save this slider currently position 
    public void SaveSliderPositionManually()
    {
        PlayerPrefs.SetInt("Slider_Current_Position_Value" + gameObject.GetInstanceID(), currentSliderNumber);
    }

    // load the slider based on the position we saved before
    public void LoadSliderPosition(bool thisFastLoading = true)
    {
        if (PlayerPrefs.HasKey("Slider_Current_Position_Value" + gameObject.GetInstanceID()))
        {
            preSliderNumber = currentSliderNumber;
            currentSliderNumber = PlayerPrefs.GetInt("Slider_Current_Position_Value" + gameObject.GetInstanceID());
            if (thisFastLoading)
            {
                sliding = false;
                if (sliderParent.GetComponent<SliderTween>())
                { 
                    sliderParent.GetComponent<SliderTween>().isRunning = false;
                    Destroy(sliderParent.GetComponent<SliderTween>());
                }

                sliderParent.position = new Vector3(
                    sliderParentStartPosition.x + (currentSliderNumber * -sliderMoveRange.x)
                    , sliderParentStartPosition.y + (currentSliderNumber * -sliderMoveRange.y)
                    , sliderParentStartPosition.z + (currentSliderNumber * -sliderMoveRange.z));


            }
            else
            {
                MoveSlider();
            }

        }
        else
        {
            preSliderNumber = 0;
            currentSliderNumber = 0;
            if (thisFastLoading)
            {
                sliding = false;
                if (sliderParent.GetComponent<SliderTween>())
                { 
                    sliderParent.GetComponent<SliderTween>().isRunning = false;
                    Destroy(sliderParent.GetComponent<SliderTween>());
                }

                sliderParent.position = new Vector3(
                    sliderParentStartPosition.x + (currentSliderNumber * -sliderMoveRange.x)
                    , sliderParentStartPosition.y + (currentSliderNumber * -sliderMoveRange.y)
                    , sliderParentStartPosition.z + (currentSliderNumber * -sliderMoveRange.z));


            }
            else
            {
                MoveSlider();
            }
        }
    }

    // load the show the slider position on a text
    public void CheckShowOnText()
    {
        if (showPageNumberText != null)
        {
            switch (showOnTextTypes)
            {
                case ShowOnTextTypes.Type1:
                    showPageNumberText.text = (currentSliderNumber + 1).ToString() + " of " + sliderPagesCount;
                    break;
                case ShowOnTextTypes.Type2:
                    showPageNumberText.text = (currentSliderNumber + 1).ToString() + " / " + sliderPagesCount;
                    break;
                case ShowOnTextTypes.Type3:
                    showPageNumberText.text = (currentSliderNumber + 1).ToString();
                    break;
                case ShowOnTextTypes.Type4:
                    showPageNumberText.text = (((float)(currentSliderNumber + 1) / (float)(sliderPagesCount)) * 100) + " %";
                    break;
                case ShowOnTextTypes.Type5:
                    showPageNumberText.text = sliderUtility.SliderUtility.ConvertWholeNumber((currentSliderNumber + 1).ToString());
                    break;
                case ShowOnTextTypes.Type6:
                    showPageNumberText.text = sliderUtility.SliderUtility.ConvertWholeNumber((currentSliderNumber + 1).ToString()) + " of " + sliderUtility.SliderUtility.ConvertWholeNumber(sliderPagesCount.ToString());
                    break;
            }
        }
    }

    #endregion
}

#region Enums and Classes

//Different slider main movement Types
public enum MovementType
{
    Immediate,
    easeInQuad,
    easeOutQuad,
    easeInOutQuad,
    easeInCubic,
    easeOutCubic,
    easeInOutCubic,
    easeInQuart,
    easeOutQuart,
    easeInOutQuart,
    easeInQuint,
    easeOutQuint,
    easeInOutQuint,
    easeInSine,
    easeOutSine,
    easeInOutSine,
    easeInExpo,
    easeOutExpo,
    easeInOutExpo,
    easeInCirc,
    easeOutCirc,
    easeInOutCirc,
    linear,
    spring,
    easeInBounce,
    easeOutBounce,
    easeInOutBounce,
    easeInBack,
    easeOutBack,
    easeInOutBack,
    easeInElastic,
    easeOutElastic,
    easeInOutElastic,


}
//Different slides animation Effects
public enum AnimationType
{
    easeInQuad,
    easeOutQuad,
    easeInOutQuad,
    easeInCubic,
    easeOutCubic,
    easeInOutCubic,
    easeInQuart,
    easeOutQuart,
    easeInOutQuart,
    easeInQuint,
    easeOutQuint,
    easeInOutQuint,
    easeInSine,
    easeOutSine,
    easeInOutSine,
    easeInExpo,
    easeOutExpo,
    easeInOutExpo,
    easeInCirc,
    easeOutCirc,
    easeInOutCirc,
    linear,
    spring,
    easeInBounce,
    easeOutBounce,
    easeInOutBounce,  
    easeInBack,
    easeOutBack,
    easeInOutBack,
    easeInElastic,
    easeOutElastic,
    easeInOutElastic
}
//Different slides animation Style
public enum animationSelection
{
    None,
    ImpulseRotate,
    ImpulseScale,
    ImpulsePosition,
    ShortRotateUpdate,
    ShortScaleUpdate,
    ShortPositionUpdate,
    ConstantRotate,
    ConstantScale,
    ConstantPosition,
    RotateTo,
    ScaleTo,
    PositionTo,
    

}
//time the animation should happen
public enum animationTime
{
    Always,
    Specified
}
//the format of the animaton playing always
public enum animationAlwaysTimeFormat
{
    loop,
    pingPong
}
// the type of reseting the slider when it reaches the end
public enum EndPageChangeType
{
      continuous,
      LockAtTheEnd,
      Reset
}
//different type of controlling the slider
public enum ControlType
{
    Automatically,
    KeyBoard,
    Arrows,
    Buttons,
    Scroll,
    No_Controllers,
}
//different type for the automatic slider
public enum AutomaticType
{
        Increase,
        Deacrease,
        Ping_Pong,
        Random
 }
//different type for camera
public enum CameraType
{
    Orthographic_2D,
    Perspective_3D
}
//different type for the text to show
public enum ShowOnTextTypes
{
    Type1,
    Type2,
    Type3,
    Type4,
    Type5,
    Type6
}

//class for unity event on the slider
[System.Serializable]
public class SlideEventClass
{
    public SliderManager.ExampleEvent OnStartSliding = new SliderManager.ExampleEvent();
    public SliderManager.ExampleEvent OnUpdateSliding = new SliderManager.ExampleEvent();
    public SliderManager.ExampleEvent OnEndSliding = new SliderManager.ExampleEvent();
}
#endregion



}

using System.Collections;
using UnityEngine;

namespace NINESOFT.TUTORIAL_SYSTEM
{

    public delegate void StandardAction();
    public delegate void StringAction(string value);
    public delegate void FloatAction(float value);
    public delegate void IntAction(int value);
    public delegate void ObjectAction(object value);

    public enum DebugType { None, Normal, Info, Successful, Error }

    public enum TriggerType { None, AutomaticRun, Collision, ButtonClick, Event, Distance, ManualCall }
    public enum EventType { None, OnEnable, OnDisable, OnDestroy, OnClick, ManualCall }

    public enum PopUpType { None, Scale, Fade, SlideToBottom, SlideToTop }
    public enum HandEventType { Normal, Holding, Click,DoubleClick }
    public enum ArrowMovementType { Follow, Static }


    public enum TransformSpaceType {[InspectorName("3D")] ThreeD, UI }


}
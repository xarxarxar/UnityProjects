namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public abstract class ButtonBase : MonoBehaviour
    {
        public Button Button => GetComponent<Button>();
    }
    
}

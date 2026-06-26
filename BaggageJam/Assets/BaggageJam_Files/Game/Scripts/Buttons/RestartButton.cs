namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    
    public class RestartButton : ButtonBase
    {
        void Start()
        {
            Button.onClick.AddListener(OnRestartButtonClicked);
        }
    
        void OnRestartButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
}

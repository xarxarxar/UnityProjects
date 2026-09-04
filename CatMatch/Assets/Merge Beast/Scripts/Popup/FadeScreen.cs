using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast
{
    public class FadeScreen : BaseScreen
    {

       public void FadeBG()
        {
            GetComponent<Animator>().Play("Fade-Out");
        }

        private void DeActiveScr()
        {
            ScreenManager.Instance.DeActiveScreen();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Lives Data", menuName = "Content/Data/Lives")]
    public class LivesData : ScriptableObject
    {
        public int maxLivesCount = 5;
        public int customedMaxLivesCount = 5;
        [Tooltip("In seconds")]public int oneLifeRestorationDuration = 1200;
        public int defaultLifeRestorationDuration = 1200;
        [Space]
        public string fullText = "����!";
        public string timespanFormat = "{0:mm\\:ss}";
        public string longTimespanFormat = "{0:hh\\:mm\\:ss}";
    }
}
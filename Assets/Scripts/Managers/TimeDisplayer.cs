using Level;
using UnityEngine;

namespace Managers
{
    public class TimeDisplayer : MonoBehaviour
    {
        internal TimeCounter TimeCounter = new();

        public void BeginCounting(float selectedLevelTimeRemaining, LevelSettings.CountdownType selectedLevelTimerType)
        {
        }
    }

    public class TimeCounter
    {
        public bool HasRanOutOfTime()
        {
            return false;
        }
    }
}
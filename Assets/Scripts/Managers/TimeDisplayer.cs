using System;
using System.Collections;
using Level;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class TimeDisplayer : MonoBehaviour
    {
        public TimeCounter TimeCounter;
        private TMP_Text _display;

        public void BeginCounting(float selectedLevelTimeRemaining)
        {
            TimeCounter = new TimeCounter(selectedLevelTimeRemaining, this);
        }

        public void Setup(Canvas timeDisplay)
        {
            Canvas canvas = Instantiate(timeDisplay);
            _display = canvas.GetComponentInChildren<TMP_Text>();
        }


        private void Update()
        {
            if (TimeCounter != null)
                if (TimeCounter.isNotOutOfTime && TimeCounter.CurrentTime <= 60)
                {
                    _display.gameObject.SetActive(true);
                    _display.text = TimeCounter.FormatToMinutes();
                }
                else if (_display.gameObject.activeSelf)
                    _display.gameObject.SetActive(false);
        }

        public void Reset()
        {
            TimeCounter = null;
        }
    }

    public class TimeCounter
    {
        private float _currentTime;

        public float CurrentTime => _currentTime;
        public bool isNotOutOfTime => _currentTime > 0;

        public TimeCounter(float designatedTime, MonoBehaviour coroutineInitiator)
        {
            _currentTime = designatedTime;
            coroutineInitiator.StartCoroutine(CountTime());
        }

        private IEnumerator CountTime()
        {
            while (isNotOutOfTime)
            {
                _currentTime -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }


        public string FormatToMinutes()
        {
            float minute = Mathf.FloorToInt(_currentTime / 60);
            float seconds = Mathf.FloorToInt(_currentTime % 60);

            return $"{minute:00}:{seconds:00}";
        }

        public bool HasRanOutOfTime(bool extraCondition = true)
        {
            return _currentTime <= 0 && extraCondition;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Michsky.UI.Shift
{
    public class TimedEvent : MonoBehaviour
    {
        [Header("TIMING (SECONDS)")]
        public float timer = 10;
        public bool enableAtStart;

        [Header("TIMER EVENT")]
        public UnityEvent timerAction;

        [Header("RECONNECT EVENT")]
        public UnityEvent reconnectedAction;

        void Start()
        {
            if(enableAtStart == true)
            {
                StartCoroutine("TimedEventStart");
            }
        }

        IEnumerator TimedEventStart()
        {
            yield return new WaitForSeconds(timer);
            Launcher.instance.disconnectedWindow.ModalWindowIn();
        }

        public void StartIEnumerator ()
        {
            StartCoroutine("TimedEventStart");
        }

        public void StopIEnumerator ()
        {
            StopCoroutine("TimedEventStart");
        }
    }
}

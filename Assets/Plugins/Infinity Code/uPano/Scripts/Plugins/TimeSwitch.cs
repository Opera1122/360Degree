/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System.Collections.Generic;
using InfinityCode.uPano.Controls;
using InfinityCode.uPano.Transitions;
using UnityEngine;

namespace InfinityCode.uPano.Plugins
{
    /// <summary>
    /// Automatically switches panoramas by timer.
    /// </summary>
    [AddComponentMenu("uPano/Plugins/TimeSwitch")]
    public class TimeSwitch : MonoBehaviour
    {
        /// <summary>
        /// Panoramas to be switched.
        /// </summary>
        public Pano[] panoramas;

        /// <summary>
        /// Index of first panorama.
        /// </summary>
        public int startIndex = 0;

        /// <summary>
        /// Duration of display of each panorama.
        /// </summary>
        public float duration = 3;

        /// <summary>
        /// If enabled, the next panorama will be randomly selected (except for the current one). If disabled, panoramas will be displayed in a row.
        /// </summary>
        public bool random = false;

        /// <summary>
        /// If random is off, after the end of the list, start from the first panorama.
        /// </summary>
        public bool loop = true;

        /// <summary>
        /// Use transitions between panoramas.
        /// </summary>
        public bool useTransition = true;

        /// <summary>
        /// Prefab which contains the transition that is played before the panorama is closed.
        /// </summary>
        public GameObject beforeTransitionPrefab;

        /// <summary>
        /// Prefab which contains the transition that is played after the panorama is closed.
        /// </summary>
        public GameObject afterTransitionPrefab;

        /// <summary>
        /// Set timer to pause on user interaction.
        /// </summary>
        public bool pauseOnInput = true;

        /// <summary>
        /// Continue timer after seconds of inactivity.
        /// </summary>
        public float restoreAfter = 10;

        private int currentIndex = -1;
        private bool isInputPause;
        private bool isSwitch = false;
        private bool pause = false;
        private List<int> prevIndices;
        private float progress = 0;
        private float restoreProgress = 0;

        private void Awake()
        {
            PanoControl.OnControlStarted += OnControlStarted;
        }

        private void OnControlStarted(PanoControl control)
        {
            control.OnInput += OnControlInput;
        }

        private void OnControlInput(PanoControl control)
        {
            if (!pauseOnInput) return;

            isInputPause = true;
            restoreProgress = 0;
        }

        /// <summary>
        /// Get the progress of the current panorama.
        /// </summary>
        /// <returns>Progress (0-1).</returns>
        public float GetProgress()
        {
            return progress;
        }

        protected Transition GetTransition(GameObject prefab)
        {
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab);
                return go.GetComponent<Transition>();
            }

            return null;
        }

        /// <summary>
        /// Switch to next panorama.
        /// </summary>
        public void Next()
        {
            int nextIndex = -1;
            if (random)
            {
                if (panoramas.Length > 1)
                {
                    do
                    {
                        nextIndex = Random.Range(0, panoramas.Length);
                    } while (nextIndex == currentIndex);
                }
            }
            else
            {
                nextIndex = currentIndex + 1;
                if (nextIndex >= panoramas.Length)
                {
                    nextIndex = loop ? 0 : -1;
                }
            }

            if (nextIndex != -1) Set(nextIndex);
            else
            {
                pause = true;
            }
        }

        /// <summary>
        /// Pause timer.
        /// </summary>
        public void Pause()
        {
            pause = true;
        }

        /// <summary>
        /// Switch to the previous panorama.
        /// </summary>
        public void Prev()
        {
            if (prevIndices.Count == 0) return;
            int prevIndex = prevIndices.Count - 1;
            Set(prevIndices[prevIndex], false);
            prevIndices.RemoveAt(prevIndex);
        }

        /// <summary>
        /// Resume timer.
        /// </summary>
        public void Resume()
        {
            pause = false;
        }

        /// <summary>
        /// Switch to panorama by index.
        /// </summary>
        /// <param name="index">Index of panorama (0 to count - 1).</param>
        public void Set(int index)
        {
            Set(index, true);
        }

        /// <summary>
        /// Switch to panorama by index.
        /// </summary>
        /// <param name="index">Index of panorama (0 to count - 1).</param>
        /// <param name="recordPrevIndex">Add the previous panorama to the history.</param>
        public void Set(int index, bool recordPrevIndex)
        {
            if (index < 0 || index >= panoramas.Length)
            {
                Debug.LogError("Index must be in the range from 0 to the number of panoramas - 1.");
                pause = true;
                return;
            }

            progress = 0;
            isSwitch = true;
            if (recordPrevIndex) prevIndices.Add(currentIndex);
            StartSwap(index);
        }

        private void StartSwap(int index)
        {
            if (!useTransition)
            {
                SwapPanoActive(index);
                isSwitch = false;
                return;
            }

            Transition beforeTransition = GetTransition(beforeTransitionPrefab);
            if (beforeTransition == null) beforeTransition = GlobalSettings.GetBeforeTransition();

            if (beforeTransition != null)
            {
                Pano pano = panoramas[currentIndex];
                pano.locked = true;
                beforeTransition.OnFinish += t =>
                {
                    pano.locked = false;
                    SwapPanoActive(index);
                    Destroy(beforeTransition.gameObject);

                    Transition afterTransition = GetTransition(afterTransitionPrefab);
                    if (afterTransition == null) afterTransition = GlobalSettings.GetAfterTransition();

                    if (afterTransition != null)
                    {
                        afterTransition.OnFinish += t2 =>
                        {
                            Destroy(afterTransition.gameObject);
                            isSwitch = false;
                        };
                        afterTransition.Execute(null);
                    }
                    else isSwitch = false;
                };
                beforeTransition.Execute(null);
            }
            else
            {
                SwapPanoActive(index);
                isSwitch = false;
            }
        }

        private void SwapPanoActive(int index)
        {
            if (currentIndex == index) return;

            if (currentIndex != -1)
            {
                panoramas[currentIndex].gameObject.SetActive(false);
            }

            currentIndex = index;

            panoramas[index].gameObject.SetActive(true);
        }

        private void Start()
        {
            prevIndices = new List<int>();
            if (panoramas.Length == 0)
            {
                Debug.LogError("The panorama list is empty.");
                return;
            }

            if (startIndex < 0 || startIndex >= panoramas.Length)
            {
                Debug.LogError("Start Index must be in the range from 0 to the number of panoramas - 1.");
                return;
            }

            for (int i = 0; i < panoramas.Length; i++)
            {
                Pano pano = panoramas[i];
                pano.gameObject.SetActive(i == startIndex);
            }

            currentIndex = startIndex;
        }

        private void Update()
        {
            if (isSwitch || pause) return;

            if (isInputPause)
            {
                restoreProgress += Time.deltaTime / restoreAfter;
                if (restoreProgress >= 1) isInputPause = false;
                return;
            }

            progress += Time.deltaTime / duration;
            if (progress >= 1)
            {
                progress = 1;
                Next();
            }
        }
    }
}
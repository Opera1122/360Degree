/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.InteractiveElements;
using UnityEngine;

namespace InfinityCode.uPano.Actions
{
    /// <summary>
    /// Plays AudioClip or AudioSource
    /// </summary>
    [AddComponentMenu("uPano/Actions/For Everything/Play Sound")]
    public class PlaySound : InteractiveElementAction
    {
        /// <summary>
        /// AudioClip. If AudioSource is set and has a clip, this value will be ignored
        /// </summary>
        public AudioClip audioClip;

        /// <summary>
        /// AudioSource. If set and has a clip, AudioClip will be ignored
        /// </summary>
        public AudioSource audioSource;

        /// <summary>
        /// Play sound once?
        /// </summary>
        public bool oneShot = true;

        /// <summary>
        /// Do not start if playing
        /// </summary>
        public bool ignoreIfPlayed = false;

        protected AudioClip currentClip;
        protected AudioSource currentSource;

        public override void Invoke(InteractiveElement element)
        {
            if (audioSource != null)
            {
                currentSource = audioSource;
                if (audioSource.clip != null) currentClip = audioSource.clip;
            }

            if (currentClip == null) currentClip = audioClip;
            if (currentClip == null) return;

            if (currentSource == null)
            {
                currentSource = gameObject.AddComponent<AudioSource>();
                currentSource.clip = currentClip;
            }

            if (!ignoreIfPlayed || !currentSource.isPlaying)
            {
                if (oneShot) currentSource.PlayOneShot(currentClip);
                else currentSource.Play();
            }
        }

        /// <summary>
        /// Stops all sounds started by all instances of PlaySound except this, and play a sound of this instance
        /// </summary>
        /// <param name="element">Interactive Element which called the action</param>
        public void PlaySolo(InteractiveElement element)
        {
            PlaySound[] playSounds = FindObjectsOfType<PlaySound>();
            foreach (PlaySound playSound in playSounds)
            {
                if (playSound != this) playSound.Stop(element);
            }
            Invoke(element);
        }

        /// <summary>
        /// Stops sound started by current instance of PlaySound
        /// </summary>
        /// <param name="element">Interactive Element which called the action</param>
        public void Stop(InteractiveElement element)
        {
            if (audioSource != null) audioSource.Stop();
        }

        /// <summary>
        /// Stops all sounds started by all instances of PlaySound
        /// </summary>
        /// <param name="element">Interactive Element which called the action</param>
        public void StopAllSounds(InteractiveElement element)
        {
            PlaySound[] playSounds = FindObjectsOfType<PlaySound>();
            foreach (PlaySound playSound in playSounds) playSound.Stop(element);
        }
    }
}
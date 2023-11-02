using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace BotanLib
{
    /// <summary>
    /// Requires TMP of course
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class TypeWriterTMP : MonoBehaviour
    {

        [SerializeField] private TMP_Text textUI = null;


        private string parsedText;
        private Action onComplete;
        private Tween tween;

        public void Reset()
        {
            textUI = GetComponent<TMP_Text>();
        }

        /// <summary>
        /// kills the tween if it is running
        /// </summary>
        private void OnDestroy()
        {
            tween?.Kill();

            tween = null;
            onComplete = null;
        }

        /// <param name="text"> Text to display (rich text supported) </param>
        /// <param name="speed">Display speed (if speed == 1 it takes 1 second to display one character, if speed == 2 it takes 0.5 seconds)</param>
        /// <param name="onUpdate">runs on tweens update, on every new letter</param>
        /// <param name="onComplete">runs on tweens end</param>

        public void Play(string text, float speed, Action onComplete, Action onUpdate)
        {
            textUI.text = text;
            this.onComplete = onComplete;

            textUI.ForceMeshUpdate();

            parsedText = textUI.GetParsedText();

            var length = parsedText.Length;
            Debug.Log(length);
            var duration = 1 / speed * length;

            OnUpdate(0);

            tween?.Kill();
            tween = DOTween
                .To(value => OnUpdate(value, onUpdate), 0, 1, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => OnComplete())
            ;
        }
        /// <param name="text"> Text to display (rich text supported) </param>
        /// <param name="speed">Display speed (if speed == 1 it takes 1 second to display one character, if speed == 2 it takes 0.5 seconds)</param>
        /// <param name="onComplete">runs on tweens end</param>
        public void Play(string text, float speed, Action onComplete)
        {
            textUI.text = text;
            this.onComplete = onComplete;

            textUI.ForceMeshUpdate();

            parsedText = textUI.GetParsedText();

            var length = parsedText.Length;
            Debug.Log(length);
            var duration = 1 / speed * length;

            OnUpdate(0);

            tween?.Kill();
            tween = DOTween
                .To(value => OnUpdate(value), 0, 1, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => OnComplete())
            ;
        }
        /// <summary>
        /// await for the animations end
        /// </summary>

        /// <param name="text"> Text to display (rich text supported) </param>
        /// <param name="speed">Display speed (if speed == 1 it takes 1 second to display one character, if speed == 2 it takes 0.5 seconds)</param>
        /// <param name="onUpdate">runs on tweens update, on every new letter</param>
        /// <param name="onComplete">runs on tweens end</param>
        public async UniTask PlayAsync(string text, float speed, Action onComplete, Action onUpdate)
        {
            textUI.text = text;

            this.onComplete = onComplete;

            textUI.ForceMeshUpdate(true);

            parsedText = textUI.GetParsedText();

            var length = parsedText.Length;
            float duration = 1f / speed * (float)length;

            OnUpdate(0, onUpdate);

            tween?.Kill();
            tween = DOTween
                .To(value => OnUpdate(value, onUpdate), 0f, 1f, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => OnComplete())
            ;
            await tween;
        }

        public void ClearText()
        {
            Play(string.Empty, 1000, null);
        }
        public void Skip(bool useCallBacks = true)
        {
            tween?.Kill();
            tween = null;

            OnUpdate(1);

            if (!useCallBacks) return;

            onComplete?.Invoke();
            onComplete = null;
        }

        public void Pause()
        {
            tween?.Pause();
        }


        public void Resume()
        {
            tween?.Play();
        }

        int cacheCount;
        private void OnUpdate(float value, Action onNewCase)
        {
            var current = Mathf.Lerp(0, parsedText.Length, value);
            var count = Mathf.FloorToInt(current);

            if (cacheCount != count)
            {
                cacheCount = count;
                onNewCase?.Invoke();
            }
            textUI.maxVisibleCharacters = count;

        }
        private void OnUpdate(float value)
        {
            var current = Mathf.Lerp(0, parsedText.Length, value);
            var count = Mathf.FloorToInt(current);

            textUI.maxVisibleCharacters = count;


        }

        private void OnComplete()
        {
            tween = null;
            onComplete?.Invoke();
            onComplete = null;
        }
    }
}
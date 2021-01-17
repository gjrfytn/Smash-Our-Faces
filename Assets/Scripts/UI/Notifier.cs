using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gjrfytn.Dim.Object;
using UnityEngine;
using UnityEngine.UI;

namespace Sof.UI
{
    public class Notifier : SofMonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Text _NotificationText;
#pragma warning restore 0649

        private readonly Queue<string> _NotificationQueue = new Queue<string>();

        private void Update()
        {
            if (_NotificationQueue.Any() && !_NotificationText.gameObject.activeSelf)
            {
                _NotificationText.text = _NotificationQueue.Dequeue();
                _NotificationText.gameObject.SetActive(true);

                StartCoroutine(DisableAfter(3));
            }
        }

        public void ShowNotification(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new System.ArgumentException("Notification text cannot be null or whitespace.", nameof(text));

            _NotificationQueue.Enqueue(text);
        }

        private IEnumerator DisableAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            _NotificationText.gameObject.SetActive(false);
        }
    }
}
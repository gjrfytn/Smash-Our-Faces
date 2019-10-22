﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Sof.UI
{
    public class Notifier : MonoBehaviour
    {
        [SerializeField]
        private Text _NotificationText;

        private Queue<string> _NotificationQueue = new Queue<string>();

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
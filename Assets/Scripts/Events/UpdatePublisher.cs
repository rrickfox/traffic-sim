using System;
using UnityEngine;

namespace Events
{
    public class UpdatePublisher : MonoBehaviour
    {
        private static WeakVoidPublisher _PRE_FIXED_UPDATE { get; } = new WeakVoidPublisher();
        private static WeakVoidPublisher _NORMAL_FIXED_UPDATE { get; } = new WeakVoidPublisher();
        private static WeakVoidPublisher _POST_FIXED_UPDATE { get; } = new WeakVoidPublisher();

        private void FixedUpdate()
        {
            _PRE_FIXED_UPDATE.Publish();
            _NORMAL_FIXED_UPDATE.Publish();
            _POST_FIXED_UPDATE.Publish();
        }
        
        public static void SubscribePreFixedUpdate(Action subscriber) => _PRE_FIXED_UPDATE.Subscribe(subscriber);
        public static void SubscribeNormalFixedUpdate(Action subscriber) => _NORMAL_FIXED_UPDATE.Subscribe(subscriber);
        public static void SubscribePostFixedUpdate(Action subscriber) => _POST_FIXED_UPDATE.Subscribe(subscriber);

        public static void UnsubscribePreFixedUpdate(Action subscriber) => _PRE_FIXED_UPDATE.Subscribe(subscriber);
        public static void UnsubscribeNormalFixedUpdate(Action subscriber) => _NORMAL_FIXED_UPDATE.Subscribe(subscriber);
        public static void UnsubscribePostFixedUpdate(Action subscriber) => _POST_FIXED_UPDATE.Subscribe(subscriber);
    }
}

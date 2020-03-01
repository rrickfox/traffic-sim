using System;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class UpdatePublisher : MonoBehaviour
    {
        private static Dictionary<Type, TypePublisher> _publishers { get; } = new Dictionary<Type, TypePublisher>();

        private void FixedUpdate()
        {
            foreach (var publisher in _publishers.Values)
                publisher.Publish();
            foreach (var publisher in _publishers.Values)
                publisher.ResetState();
        }

        public static void RegisterTypePublisher<T>(TypePublisher typePublisher)
        {
            try
            {
                _publishers.Add(typeof(T), typePublisher);
            }
            catch (ArgumentException e)
            {
                throw new Exception("There can only be one TypePublisher per type", e);
            }
        }
    }
}

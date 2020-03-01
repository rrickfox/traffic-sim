using System;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    // Manages the publishing of FixedUpdates
    public class UpdatePublisher : MonoBehaviour
    {
        private static Dictionary<Type, TypePublisher> _publishers { get; } = new Dictionary<Type, TypePublisher>();

        private void FixedUpdate()
        {
            foreach (var publisher in _publishers.Values)
                publisher.Publish();
            // reset the publishers' states so they know that
            // they haven't been invoked in the next update yet
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

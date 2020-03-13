using System.Collections.Generic;
using Interface;
using UnityEngine;
using Utility;

namespace Events
{
    // Manages the publishing of FixedUpdates
    public class UpdatePublisher : MonoBehaviour
    {
        private static HashSet<TypePublisher> _publishers { get; } = new HashSet<TypePublisher>();

        private void FixedUpdate()
        {
            if (Manager.pause || Manager.menu)
                return;
            foreach (var publisher in _publishers)
                publisher.Publish();
            // reset the publishers' states so they know that
            // they haven't been invoked in the next update yet
            foreach (var publisher in _publishers)
                publisher.ResetState();
        }

        public static void RegisterTypePublisher(TypePublisher typePublisher) => _publishers.Add(typePublisher);

        public static void ResetPublisher()
        {
            foreach (var publisher in _publishers)
            {
                publisher.ResetObjectPublishers();
            }
        }
    }
}

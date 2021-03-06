using System.Collections.Generic;
using Events;
using UnityEngine;

namespace DataTypes
{
    public abstract class GameObjectData
    {
        public abstract GameObject prefab { get; }
        public GameObject gameObject { get; }
        public Transform transform { get; }
        public LinkedBehaviour behaviour { get; }
        protected HashSet<ObjectPublisher> _allPublishers { get; } = new HashSet<ObjectPublisher>();

        protected GameObjectData()
        {
            gameObject = Object.Instantiate(prefab, Saves.SaveLoader.simulation.transform);
            // set default name
            gameObject.name = $"{prefab.name} ({gameObject.GetInstanceID()})";
            transform = gameObject.transform;
            behaviour = gameObject.AddComponent<LinkedBehaviour>();
            behaviour.data = this;
        }

        public void Dispose()
        {
            Object.Destroy(gameObject);
            // unsubscribe from all events to make sure there are no more references to this object
            foreach (var publisher in _allPublishers)
                publisher?.Dispose();
        }
    }
    
    public class LinkedBehaviour : MonoBehaviour
    {
        // You may have to manually cast this to the GameObjectData subclass you need.
        // This is not really fixable since we don't have a reference to the specific type.
        public GameObjectData data { get; set; }
    }
}
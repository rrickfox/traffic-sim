using Events;
using UnityEngine;
using static Utility.CONSTANTS;

namespace DataTypes
{
    public abstract class GameObjectData
    {
        public GameObject prefab { get; }
        public GameObject gameObject { get; }
        public Transform transform { get; }
        public LinkedBehaviour behaviour { get; }
        protected ObjectPublisher _publisher { get; set; }

        protected GameObjectData(GameObject prefab)
        {
            this.prefab = prefab;
            gameObject = Object.Instantiate(this.prefab);
            // TODO: find way to generate useful default name (own type is no longer accessible)
            // // set default name
            // gameObject.name = $"{typeof(TThis)} ({gameObject.GetInstanceID()})";
            transform = gameObject.transform;
            behaviour = gameObject.AddComponent<LinkedBehaviour>();
            behaviour.data = this;
        }

        // construct using an empty prefab
        protected GameObjectData() : this(EMPTY_PREFAB) { }

        public void Dispose()
        {
            Object.Destroy(gameObject);
            // unsubscribe from all events to make sure there are no more references to this object
            _publisher?.UnsubscribeAll();
        }
    }
    
    public class LinkedBehaviour : MonoBehaviour
    {
        // You may have to manually cast this to the GameObjectData subclass you need.
        // This is not really fixable since we don't have a reference to the specific type.
        public GameObjectData data { get; set; }
    }
}
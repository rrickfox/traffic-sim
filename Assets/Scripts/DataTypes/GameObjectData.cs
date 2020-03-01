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
        protected ObjectPublisher _publisher { get; set; }

        protected GameObjectData(GameObject prefab)
        {
            this.prefab = prefab;
            gameObject = Object.Instantiate(this.prefab);
            // TODO: find way to generate useful default name (own type is no longer accessible)
            // // set default name
            // gameObject.name = $"{typeof(TThis)} ({gameObject.GetInstanceID()})";
            transform = gameObject.transform;
        }
        
        // construct using an empty prefab
        protected GameObjectData() : this(EMPTY_PREFAB) { }

        public void Dispose()
        {
            Object.Destroy(gameObject);
            _publisher?.UnsubscribeAll();
        }
    }
}
using System;
using UnityEngine;
using Utility;

namespace DataTypes
{
    public class GameObjectData<TThis, TBehaviour> : IDisposable
        where TBehaviour : LinkedBehaviour<TThis>
        where TThis : GameObjectData<TThis, TBehaviour>
    {
        public GameObject prefab { get; }
        public GameObject gameObject { get; }
        public Transform transform { get; private set; }
        public TBehaviour behaviour { get; private set; }

        protected GameObjectData(GameObject prefab)
        {
            if (!(this is TThis))
                throw new InvalidCastException("The first type parameter of a subclass GameObjectData has to be itself");
            this.prefab = prefab;
            gameObject = UnityEngine.Object.Instantiate(this.prefab);
            // set default name
            gameObject.name = $"{typeof(TThis)} ({gameObject.GetInstanceID()})";
            transform = gameObject.transform;
            behaviour = gameObject.AddComponent<TBehaviour>();
            behaviour.Initialize((TThis)this);
        }
        
        // construct using an empty prefab
        protected GameObjectData() : this(CONSTANTS.EMPTY_PREFAB) { }
        
        public void Dispose() => UnityEngine.Object.Destroy(gameObject);
    }
}
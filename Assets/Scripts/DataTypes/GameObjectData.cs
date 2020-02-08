using System;
using UnityEngine;
using static Utility.CONSTANTS;

namespace DataTypes
{
    public interface IGameObjectData : IDisposable
    {
        GameObject prefab { get; }
        GameObject gameObject { get; }
        Transform transform { get; }
    }
    
    public class GameObjectData<TThis, TBehaviour> : IGameObjectData
        where TBehaviour : LinkedBehaviour<TThis>
        where TThis : GameObjectData<TThis, TBehaviour>
    {
        public GameObject prefab { get; }
        public GameObject gameObject { get; }
        public Transform transform { get; }
        public TBehaviour behaviour { get; }

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
        protected GameObjectData() : this(EMPTY_PREFAB) { }
        
        public void Dispose() => UnityEngine.Object.Destroy(gameObject);
    }
}
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
    
    public class GameObjectData : IGameObjectData
    {
        public GameObject prefab { get; }
        public GameObject gameObject { get; }
        public Transform transform { get; }

        protected GameObjectData(GameObject prefab)
        {
            this.prefab = prefab;
            gameObject = UnityEngine.Object.Instantiate(this.prefab);
            // TODO: find way to generate useful default name (own type is no longer accessible)
            // // set default name
            // gameObject.name = $"{typeof(TThis)} ({gameObject.GetInstanceID()})";
            transform = gameObject.transform;
        }
        
        // construct using an empty prefab
        protected GameObjectData() : this(EMPTY_PREFAB) { }
        
        public void Dispose() => UnityEngine.Object.Destroy(gameObject);
    }
}
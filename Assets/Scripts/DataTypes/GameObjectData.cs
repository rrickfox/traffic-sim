using System;
using UnityEngine;

namespace DataTypes
{
    public class GameObjectData<TThis, TBehaviour> : IDisposable
        where TBehaviour : LinkedBehaviour<TThis>
        where TThis : GameObjectData<TThis, TBehaviour>
    {
        public GameObject gameObject { get; }
        public Transform transform { get; private set; }
        public TBehaviour behaviour { get; private set; }

        protected GameObjectData()
        {
            gameObject = new GameObject();
            Initialize();
        }

        protected GameObjectData(GameObject prefab)
        {
            gameObject = UnityEngine.Object.Instantiate(prefab);
            Initialize();
        }

        private void Initialize()
        {
            // set default name
            gameObject.name = $"{typeof(TThis)} ({gameObject.GetInstanceID()})";
            transform = gameObject.transform;
            behaviour = gameObject.AddComponent<TBehaviour>();
            behaviour.Initialize((TThis)this);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
using System;
using UnityEngine;

namespace DataTypes
{
    public class GameObjectData<TThis, TBehaviour> : IDisposable
        where TBehaviour : LinkedBehaviour<TThis>
        where TThis : GameObjectData<TThis, TBehaviour>
    {
        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }
        public TBehaviour behaviour { get; private set; }

        protected void CreateGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            gameObject = UnityEngine.Object.Instantiate(prefab, position, rotation);
            PostGameObjectCreation();
        }

        protected void CreateGameObject()
        {
            gameObject = new GameObject();
            PostGameObjectCreation();
        }

        private void PostGameObjectCreation()
        {
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
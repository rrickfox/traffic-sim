using System;
using UnityEngine;

namespace DataTypes
{
    public class GameObjectData<TBehaviour> : IDisposable where TBehaviour : MonoBehaviour
    {
        public GameObject gameObject { get; private set; }
        public TBehaviour behaviour { get; private set; }

        protected void CreateGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            gameObject = UnityEngine.Object.Instantiate(prefab, position, rotation);
            behaviour = gameObject.AddComponent<TBehaviour>();
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
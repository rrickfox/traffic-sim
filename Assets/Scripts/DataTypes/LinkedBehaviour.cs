using UnityEngine;

namespace DataTypes
{
    public class LinkedBehaviour<T> : MonoBehaviour
    {
        protected T _data { get; private set; }

        public void Initialize(T data)
        {
            _data = data;
        }
    }
}
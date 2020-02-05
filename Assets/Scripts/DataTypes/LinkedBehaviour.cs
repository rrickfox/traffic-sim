using UnityEngine;

namespace DataTypes
{
    public class LinkedBehaviour<TData> : MonoBehaviour
    {
        protected TData _data { get; private set; }

        public void Initialize(TData data)
        {
            _data = data;
        }
    }
}
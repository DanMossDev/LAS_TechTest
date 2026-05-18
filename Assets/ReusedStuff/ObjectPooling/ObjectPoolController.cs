using System.Collections.Generic;
using UnityEngine;

namespace MossUtils
{
    public class ObjectPoolController : MonoBehaviour
    {
        private ObjectPoolModel _model;
        private Dictionary<string, ObjectPool> _pools = new Dictionary<string, ObjectPool>();

        public static ObjectPoolController Instance { get; private set; }
        
        private void OnEnable()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        private void OnDisable()
        {
            if (Instance == this)
                Instance = null;
        }

        private void Awake()
        {
            _model = Resources.Load<ObjectPoolModel>("ObjectPoolModel");
            Debug.Assert(_model != null, "Object pool model is null");
        }

        public GameObject Get(string key)
        {
            if (!_pools.ContainsKey(key))
            {
                var parent = new GameObject($"{key} pool");
                parent.transform.parent = transform;
                _pools.Add(key, new ObjectPool(_model.PrefabLookup[key], parent.transform));
            }

            return _pools[key].Get();
        }

        public GameObject Get(string key, Transform owner)
        {
            if (!_pools.ContainsKey(key))
            {
                var parent = new GameObject($"{key} pool");
                parent.transform.parent = transform;
                _pools.Add(key, new ObjectPool(_model.PrefabLookup[key], parent.transform));
            }

            var obj = _pools[key].Get();
            obj.transform.position = owner.position;

            return obj;
        }

        public void Return(GameObject returned, string key)
        {
            _pools[key].Return(returned);
        }
    }
}

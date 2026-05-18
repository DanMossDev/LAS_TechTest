using System.Collections.Generic;
using UnityEngine;

namespace MossUtils
{
    public class ObjectPool
    {
        private GameObject _prefab;
        private Transform _parent;
        private List<GameObject> _objects = new List<GameObject>();


        public ObjectPool(GameObject prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public GameObject Get()
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                if (!_objects[i].activeSelf)
                {
                    _objects[i].SetActive(true);
                    return _objects[i];
                }
            }

            return CreateNew();
        }

        private GameObject CreateNew()
        {
            var go = GameObject.Instantiate(_prefab, _parent);
            _objects.Add(go);
            return go;
        }

        public void Return(GameObject returned)
        {
            returned.transform.parent = _parent;
            returned.gameObject.SetActive(false);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace MossUtils
{
    [CreateAssetMenu(menuName= "ObjectPool/ObjectPoolModel", fileName="ObjectPoolModel")]
    public class ObjectPoolModel : ScriptableObject
    {
        [SerializeField] private GameObject[] _prefabList;

        [System.NonSerialized] private Dictionary<string, GameObject> _prefabLookup;
        public Dictionary<string, GameObject> PrefabLookup
        {
            get
            {
                if (_prefabLookup == null)
                {
                    _prefabLookup = new Dictionary<string, GameObject>(); 
                    
                    foreach (var prefab in _prefabList)
                        _prefabLookup.Add(prefab.name, prefab);
                }
                
                return _prefabLookup;
            }
        }
    }
}

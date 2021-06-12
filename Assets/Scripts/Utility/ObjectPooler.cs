using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class ObjectPooler
    {
        private static Dictionary<int, List<GameObject>> dictionaryOfPooledObjects = new();

        private static void PoolGameObject(GameObject objectToPool, int amm, Transform parent)
        {
            parent = !parent ? new GameObject($"{objectToPool.name}'s Pool").transform : parent;

            List<GameObject> pool = new List<GameObject>();

            for (int i = 0; i < amm; i++)
            {
                GameObject clone = Object.Instantiate(objectToPool, parent);
                clone.SetActive(false);
                pool.Add(clone);
            }

            if (dictionaryOfPooledObjects.ContainsKey(objectToPool.GetInstanceID()))
            {
                dictionaryOfPooledObjects[objectToPool.GetInstanceID()].AddRange(pool);
            }
            else
            {
                dictionaryOfPooledObjects.Add(objectToPool.GetInstanceID(), pool);
            }
        }


        public static GameObject DynamicInstantiate(GameObject gameObject, Transform parent, int amm = 300)
        {
            if (gameObject == null) return null;
            foreach (var pool in dictionaryOfPooledObjects)
            {
                if (pool.Key.Equals(gameObject.GetInstanceID()))
                {
                    foreach (var pooledObject in pool.Value)
                    {
                        if (pooledObject && !pooledObject.activeSelf)
                        {
                            pooledObject.SetActive(true);
                            return pooledObject;
                        }
                    }
                }
            }

            PoolGameObject(gameObject, amm, parent);
            return DynamicInstantiate(gameObject, parent, amm);
        }

        public static GameObject DynamicInstantiate(GameObject gameObject, Vector3 position, Quaternion rotation,
            int amm = 300)
        {
            GameObject foundObj = DynamicInstantiate(gameObject, null, amm);
            if (!foundObj) return null;
            foundObj.transform.position = position;
            foundObj.transform.rotation = rotation;
            return foundObj;
        }

        public static T DynamicInstantiate<T>(T gameObject, Transform parent, int amm = 300)
        {
            MonoBehaviour monoBehaviour = (gameObject as MonoBehaviour);
            Component component = (gameObject as Component);
            return DynamicInstantiate(
                    monoBehaviour ? monoBehaviour.gameObject : component ? component.gameObject : null, parent, amm)
                .GetComponent<T>();
        }

        public static T DynamicInstantiate<T>(T gameObject, Vector3 position, Quaternion rotation, int amm = 300)
        {
            MonoBehaviour monoBehaviour = (gameObject as MonoBehaviour);
            Component component = (gameObject as Component);
            return DynamicInstantiate(
                monoBehaviour ? monoBehaviour.gameObject : component ? component.gameObject : null, position,
                rotation, amm).GetComponent<T>();
        }

        public static void RemoveObjectFromPool(GameObject gameObject)
        {
            GameObject objToRemove = null;
            foreach (var pooledObjects in dictionaryOfPooledObjects)
            {
                foreach (var pooledObject in pooledObjects.Value)
                {
                    if (pooledObject.GetInstanceID() == gameObject.GetInstanceID())
                        objToRemove = pooledObject;
                }

                if (objToRemove)
                {
                    pooledObjects.Value.Remove(objToRemove);
                    break;
                }
            }
        }
    }
}
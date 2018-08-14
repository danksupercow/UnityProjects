using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    #region Singleton
    public static ObjectPooler instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region PoolClasses
    [Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    public class PoolObject
    {
        public GameObject Object;
        public IPooledObject PooledObject;

        public PoolObject(GameObject obj, IPooledObject pooledObject)
        {
            Object = obj;
            PooledObject = pooledObject;
        }
    }
    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<PoolObject>> poolDictionary;
    
    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<PoolObject>>();

        foreach (Pool pool in pools)
        {
            Console.Log("Creating Pool: " + pool.tag + "...");
            Queue<PoolObject> objectPool = new Queue<PoolObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                IPooledObject pooledObject = obj.GetComponent<IPooledObject>();
                obj.SetActive(false);
                objectPool.Enqueue(new PoolObject(obj, pooledObject));
            }
            poolDictionary.Add(pool.tag, objectPool);
            Console.Log("Successfully Created Pool: " + pool.tag + " with " + pool.size + " GameObjects.");
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot)
    {
        if(!poolDictionary.ContainsKey(tag))
        {
            Console.LogError("ObjectPooler: Pool With Tag " + tag + " does not exist.");
            Debug.LogError("ObjectPooler: Pool With Tag" + tag + "does not exist.");
            return null;
        }

        PoolObject poolObj = poolDictionary[tag].Dequeue();
        poolObj.Object.SetActive(true);
        poolObj.Object.transform.position = pos;
        poolObj.Object.transform.rotation = rot;
        
        if(poolObj.PooledObject != null)
        {
            poolObj.PooledObject.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(poolObj);

        return poolObj.Object;
    }
}

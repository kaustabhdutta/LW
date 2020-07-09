using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton
    public static objectPooler Instance;
    private void Awake() {
        Instance = this;
    }
    #endregion
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools) {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) {
           // Debug.LogWarning("Pool with tag" + tag "doesn't exsist.");
            return null;
        }


       GameObject objSpawn= poolDictionary[tag].Dequeue();

        objSpawn.SetActive(true);
        objSpawn.transform.position = position;
        objSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objSpawn);

        return objSpawn;
    }
}

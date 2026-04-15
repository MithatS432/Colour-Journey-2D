using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDict;

    void Awake()
    {
        Instance = this;

        poolDict = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            Queue<GameObject> objPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                objPool.Enqueue(obj);
            }

            poolDict.Add(pool.tag, objPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot)
    {
        if (!poolDict.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool tag '{tag}' bulunamadı!");
            return null;
        }

        if (poolDict[tag].Count == 0)
        {
            Debug.LogWarning($"Pool '{tag}' boş! Daha fazla obje ekleyin veya return edilmemiş objeler var.");
            return null;
        }

        GameObject obj = poolDict[tag].Dequeue();

        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.rotation = rot;

        poolDict[tag].Enqueue(obj);

        return obj;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!poolDict.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool tag '{tag}' bulunamadı!");
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);

        if (!poolDict[tag].Contains(obj))
        {
            poolDict[tag].Enqueue(obj);
        }
    }
}
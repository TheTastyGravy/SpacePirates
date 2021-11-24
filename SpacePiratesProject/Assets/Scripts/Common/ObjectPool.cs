using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Tooltip("Name used to get this pool")]
    public string poolName = "New Pool";
    public GameObject prefab;
    [Tooltip("The maximum number of objects that will be stored as disabled")]
    public uint maxStoredCount = 10;

    public List<GameObject> pool = new List<GameObject>();
    // Object to be returned to the pool after a delay
    private struct ReturnEntry
    {
        public GameObject obj;
        public float timeRemaining;
    }
    private List<ReturnEntry> toReturn = new List<ReturnEntry>();

    // Dictionary indexed by the hash of poolName
    private static Dictionary<int, ObjectPool> allPools = new Dictionary<int, ObjectPool>();



    void Awake()
    {
        allPools.Add(poolName.GetHashCode(), this);
    }

    void OnDestroy()
    {
        allPools.Remove(poolName.GetHashCode().GetHashCode());
        foreach (GameObject obj in pool)
        {
            Destroy(obj);
        }
    }

    void Update()
    {
        for (int i = 0; i < toReturn.Count; i++)
        {
            ReturnEntry entry = toReturn[i];
            entry.timeRemaining -= Time.deltaTime;
            if (entry.timeRemaining <= 0)
            {
                if (!pool.Contains(entry.obj))
                    Return(entry.obj);
                toReturn.RemoveAt(i);
                i--;
            }
            else
            {
                toReturn[i] = entry;
            }
        }
    }

    public GameObject GetInstance()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool[0];

            if (obj == null)
            {
                Debug.LogError("<size=15>OBJECT POOL INSTANCE IS NULL</size>");
            }

            obj.SetActive(true);
            pool.RemoveAt(0);
            return obj;
        }
        else
        {
            // Pool is empty, create a new one
            return Instantiate(prefab, transform);
        }
    }

    public void Return(GameObject instance)
    {
        if (instance == null)
        {
            Debug.LogError("<size=15>OBJECT POOL INSTANCE IS NULL</size>");
        }

        if (pool.Count < maxStoredCount)
        {
            instance.SetActive(false);
            pool.Add(instance);
        }
        else
        {
            // Too many stored objects, destroy it
            Destroy(instance);
        }
    }

    public void Return(GameObject instance, float time)
    {
        if (instance == null)
        {
            Debug.LogError("<size=15>OBJECT POOL INSTANCE IS NULL</size>");
        }

        toReturn.Add(new ReturnEntry() { obj = instance, timeRemaining = time });
    }

    public static ObjectPool GetPool(string poolName)
    {
        if (allPools.ContainsKey(poolName.GetHashCode()))
        {
            return allPools[poolName.GetHashCode()];
        }
        else
        {
            throw new System.Exception("Requested object pool does not exist");
        }
    }
}

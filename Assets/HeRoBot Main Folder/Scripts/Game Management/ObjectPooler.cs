using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[DefaultExecutionOrder ( -116 )]
public class ObjectPooler : MonoBehaviour
{
    // this script is on the Object Pooler object which is an empty game object in the Folder GameManagerEtc.
    [System.Serializable]
    public class Pool
    {
        public string tag;

        public GameObject projectile;
        public int size;
    }

    #region Singleton
    public static ObjectPooler Instance;

    private void Awake( )
    {
        Instance = this;
    }
    #endregion

    public List<Pool> pools;

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    IPooledObject pooledObject;

    void Start( )
    {

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for(int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.projectile);

                obj.SetActive(false);
                objectPool.Enqueue(obj); // add to the queue
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool( string tag, Vector3 position )
    {
        if(!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + "doesn't exist");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[ tag ].Dequeue(); // remove from the queue

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;

        pooledObject = objectToSpawn.GetComponent<IPooledObject>();

        if(pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        poolDictionary[ tag ].Enqueue(objectToSpawn); // put back in the queue

        return objectToSpawn;
    }
}

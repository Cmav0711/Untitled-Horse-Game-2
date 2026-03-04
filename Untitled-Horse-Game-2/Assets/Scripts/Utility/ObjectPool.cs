using System.Collections.Generic;
using UnityEngine;

/*
 *      Object Pool: An optimization strategy where, instead of creating reusable objects at runtime (which is expensive and laggy), 
 *      you create a "pool" of objects at the start of the game and then grab one from the pool when you need it.
 *
 *      To return an object to the pool, simply set it to inactive. The next time you need an object, the pool will check for an inactive one and return it.
 */

public class ObjectPool : MonoBehaviour
{
    private List<GameObject> pooledObjects; //The list of objects in the pool
    public GameObject objectToPool; //The object that makes up the pool
    public int amountToPool; //The amount of objects to pool, also the limit for how many can be active

    //Instantiate all objects to the pool in awake and deactivates them
    private void Awake()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.transform.parent = transform;
            pooledObjects.Add(tmp);
            tmp.SetActive(false);
        }
    }

    //Return the next object available in the pool but does nothing with it. might be null if none available
    public GameObject GetNextObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    //Returns the next object available in the pool then activates it and places it in the world. might be null if none available
    public GameObject PlaceNextObject(Vector3 pos, Quaternion rot)
    {
        GameObject obj = GetNextObject();
        if (obj != null)
        {
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.SetActive(true);
            return obj;
        }
        else
            return null;
    }

    //Deactivates all pooled objects
    public void DeactivateAllObjects()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            pooledObjects[i].SetActive(false);
        }
    }
}

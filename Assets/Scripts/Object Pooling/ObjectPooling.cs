using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling 
{
    private static Dictionary<string, Queue<Component>> objectPool = new Dictionary<string, Queue<Component>>();
    private static Dictionary<string, Component> referencedObjects = new Dictionary<string, Component>();

    static int defaultPoolSize = 10;

    static int DefaultPoolSize 
    { 
        get
        { 
            return defaultPoolSize; 
        }
        set
        {
            defaultPoolSize = value; 
        }
    }

    public static void EnqueuePool<T>(string poolName, T objectToEnque) where T : Component
    {
        if (!objectToEnque.gameObject.activeSelf)
        {
            return;
        }
 
        objectToEnque.transform.position = Vector2.zero;
        objectPool[poolName].Enqueue(objectToEnque);
        objectToEnque.gameObject.SetActive(false);
    }

    public static T DequeuePool<T>(string poolName) where T : Component
    {
        if (objectPool[poolName].TryDequeue(out var value))
        {
            return (T)value;
        }

        T newObject = ExtendPool<T>(poolName);

        return newObject;
    }

    public static T ExtendPool<T>(string poolName) where T: Component
    {
        T newObject = Object.Instantiate((T)referencedObjects[poolName]); 
        newObject.transform.SetParent(GameManager.instance.transform);
        newObject.transform.position = Vector3.zero;
        newObject.gameObject.SetActive(false);
        return newObject;
        
    }
    
    public static void SetupPool<T>(string poolName,T objectToBeInstantiated) where T : Component
    {
        objectPool[poolName] = new Queue<Component>();
        referencedObjects[poolName] = objectToBeInstantiated;

        for (int i = 0; i < defaultPoolSize; i++)
        {
            T instantiatedObject = Object.Instantiate(objectToBeInstantiated);
            instantiatedObject.transform.SetParent(GameManager.instance.transform);
            instantiatedObject.transform.position = Vector2.zero;
            instantiatedObject.gameObject.SetActive(false);
            objectPool[poolName].Enqueue(instantiatedObject);
        }
       
    }

}

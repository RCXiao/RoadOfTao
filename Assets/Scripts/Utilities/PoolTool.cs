using UnityEngine;
using UnityEngine.Pool;


public class PoolTool : MonoBehaviour
{
    public GameObject ObjPrefab;
    private ObjectPool<GameObject> pool;
    private void Awake()
    {
        //初始化对象池
        pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(ObjPrefab, transform),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 100
            );

        PreFillPool(10);
    }

    private void PreFillPool(int count)
    {
        var preFillArray = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            preFillArray[i] = pool.Get();
        }

        foreach (var item in preFillArray)
        {
            pool.Release(item);
        }
    }

    public GameObject GetObjectFromPool()
    {
        return pool.Get();
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        pool.Release(obj);
    }
}

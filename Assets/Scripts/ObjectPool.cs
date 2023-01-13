using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Hit Notes")]
    public GameObject prefab;
    public int quantity;

    int index;
    List<GameObject> pool;

    private Vector2 HiddenPosition { get { return Vector2.up * 100; } }

    private void Awake() 
    {
        if (!prefab || quantity < 1)
            return;

        MakePool();
    }

    private void MakePool()
    {
        index = 0;
        pool = new List<GameObject>();
        
        for (int i = 0; i < quantity; i++) 
        {
            GameObject go = Instantiate (prefab, HiddenPosition, Quaternion.identity, transform);
            go.SetActive(false);
            pool.Add(go);
        }
    }


    public GameObject GetFromPool() 
    {
        if (pool == null)
            return null;

        GameObject go = pool[index];
        index = (index + 1) % pool.Count;

        return go;
    }

    public void ReturnToPool(GameObject go) 
    {
        go.transform.position = HiddenPosition;
        go.SetActive(false);
    }
}

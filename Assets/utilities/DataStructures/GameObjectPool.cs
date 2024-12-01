using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
  private readonly Queue<GameObject> available;
  private readonly HashSet<GameObject> inUse;
  private readonly GameObject prefab;
  private readonly Transform parent;

  public GameObjectPool(GameObject prefab, int initialSize = 10, Transform parent = null)
  {
    this.prefab = prefab;
    this.parent = parent;
    available = new Queue<GameObject>();
    inUse = new HashSet<GameObject>();

    // Pre-instantiate GameObjects and add to the available queue
    for (int i = 0; i < initialSize; i++)
    {
      GameObject instance = Object.Instantiate(prefab, parent);
      instance.SetActive(false);
      available.Enqueue(instance);
    }
  }

  public GameObject GetItem()
  {
    GameObject item;
    if (available.Count > 0)
    {
      item = available.Dequeue();
    }
    else
    {
      item = Object.Instantiate(prefab, parent);
    }

    inUse.Add(item);
    item.SetActive(true);
    return item;
  }

  public void FinishedUsingItem(GameObject item)
  {
    item.SetActive(false);
    available.Enqueue(item);
    inUse.Remove(item);
  }

  public void FinishedUsingAllItems()
  {
    foreach (GameObject item in inUse)
    {
      item.SetActive(false);
      available.Enqueue(item);
    }
    inUse.Clear();
  }
}
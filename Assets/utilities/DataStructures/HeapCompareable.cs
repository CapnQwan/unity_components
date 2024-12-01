using System;
using System.Collections;
using System.Collections.Generic;

public class HeapCompareable<T> : IEnumerable<T> where T : IComparable<T>
{
  private List<T> elements = new List<T>();
  public List<T> Elements => elements;

  public int Size => elements.Count;

  public bool IsEmpty => elements.Count == 0;

  public void Clear()
  {
    elements.Clear();
  }

  public void Insert(T item)
  {
    elements.Add(item);
    HeapifyUp(elements.Count - 1);
  }

  public T Peek()
  {
    if (IsEmpty)
      throw new InvalidOperationException("Heap is empty.");

    return elements[0];
  }

  public T ExtractMin()
  {
    if (IsEmpty)
      throw new InvalidOperationException("Heap is empty.");

    T min = elements[0];
    elements[0] = elements[elements.Count - 1];
    elements.RemoveAt(elements.Count - 1);
    HeapifyDown(0);

    return min;
  }

  public bool Contains(T item)
  {
    foreach (T element in elements)
    {
      if (element.Equals(item))
      {
        return true;
      }
    }
    return false;
  }

  private void HeapifyUp(int index)
  {
    while (index > 0)
    {
      int parentIndex = (index - 1) / 2;

      if (elements[index].CompareTo(elements[parentIndex]) >= 0)
      {
        break;
      }

      Swap(index, parentIndex);
      index = parentIndex;
    }
  }

  private void HeapifyDown(int index)
  {
    int lastIndex = elements.Count - 1;
    while (index < lastIndex)
    {
      int leftChildIndex = 2 * index + 1;
      int rightChildIndex = 2 * index + 2;
      int smallestChildIndex = index;

      if (leftChildIndex <= lastIndex && elements[leftChildIndex].CompareTo(elements[smallestChildIndex]) < 0)
      {
        smallestChildIndex = leftChildIndex;
      }

      if (rightChildIndex <= lastIndex && elements[rightChildIndex].CompareTo(elements[smallestChildIndex]) < 0)
      {
        smallestChildIndex = rightChildIndex;
      }

      if (smallestChildIndex == index)
      {
        break;
      }

      Swap(index, smallestChildIndex);
      index = smallestChildIndex;
    }
  }

  private void Swap(int index1, int index2)
  {
    T temp = elements[index1];
    elements[index1] = elements[index2];
    elements[index2] = temp;
  }

  public IEnumerator<T> GetEnumerator()
  {
    return elements.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}

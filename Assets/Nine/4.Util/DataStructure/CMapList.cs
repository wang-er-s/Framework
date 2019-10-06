using System;
using System.Collections.Generic;

public class CMapList <T> where T : class, new()
{
    
    private DoubleLinkedList<T> linkedList = new DoubleLinkedList<T>();
    
    Dictionary<T,DoubleLinkedListNode<T>> findMap = new Dictionary<T,DoubleLinkedListNode<T>>();

    ~CMapList()
    {
        Clear();
    }
    
    public void InsertToHead(T t)
    {
        DoubleLinkedListNode<T> node = null;
        if (findMap.TryGetValue(t, out node) && node != null)
        {
            linkedList.AddToHeader(node);
            return;
        }

        linkedList.AddToHeader(t);
        findMap.Add(t,linkedList.Head);
    }

    public void Clear()
    {
        while (linkedList.Tail != null)
        {
            Remove(linkedList.Tail.data);
        }
    }

    public void Pop()
    {
        if (linkedList.Tail == null) return;
        Remove(linkedList.Tail.data);
    }

    private void Remove(T t)
    {
        DoubleLinkedListNode<T> node = null;
        if(!findMap.TryGetValue(t,out node) || node == null) return;
        linkedList.RemoveNode(node);
        findMap.Remove(t);
    }

    public T GetTail()
    {
        return linkedList.Tail?.data;
    }

    public int Count()
    {
        return linkedList.Count;
    }

    /// <summary>
    /// 查找是否存在该节点
    /// </summary>
    public bool Find(T t)
    {
        DoubleLinkedListNode<T> node = null;
        if (!findMap.TryGetValue(t, out node) || node == null)
            return false;
        return true;
    }

    public bool MoveToHead(T t)
    {
        DoubleLinkedListNode<T> node = null;
        if (!findMap.TryGetValue(t, out node) || node == null)
            return false;
        linkedList.MoveToHead(node);
        return true;
    }
    
}

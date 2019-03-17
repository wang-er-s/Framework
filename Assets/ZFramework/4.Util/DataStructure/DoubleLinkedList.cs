/*
* Create by Soso
* Time : 2019-01-24-02 下午
*/
using UnityEngine;
using System;
using ZFramework;

public class DoubleLinkedList<T> where T : class, new()
{
    public DoubleLinkedListNode<T> Head = null;

    public DoubleLinkedListNode<T> Tail = null;

    protected SimpleObjectPool<DoubleLinkedListNode<T>> doubleLinkedNodePool;

    public DoubleLinkedList()
    {
        doubleLinkedNodePool =
            new SimpleObjectPool<DoubleLinkedListNode<T>>(() => new DoubleLinkedListNode<T>(), ResetNode, 100);
    }

    public int Count { get; private set; } = 0;
    
    
    public DoubleLinkedListNode<T> AddToHeader(T t)
    {
        DoubleLinkedListNode<T> pList = doubleLinkedNodePool.Spawn();
        pList.data = t;
        return AddToHeader(pList);
    }

    public DoubleLinkedListNode<T> AddToHeader(DoubleLinkedListNode<T> node)
    {
        if(node == null) return null;
        node.preNode = null;
        if (Head == null)
        {
            Head = Tail = node;
        }
        else
        {
            node.nextNode = Head;
            Head.preNode = node;
            Head = node;
        }

        return Head;
    }

    public DoubleLinkedListNode<T> AddToTail(T t)
    {
        DoubleLinkedListNode<T> node = doubleLinkedNodePool.Spawn();
        node.data = t;
        return AddToTail(node);
    }

    public DoubleLinkedListNode<T> AddToTail(DoubleLinkedListNode<T> node)
    {
        if (node == null) return null;
        node.nextNode = null;
        if (Tail == null)
        {
            Head = Tail = node;
        }
        else
        {
            node.preNode = Tail;
            Tail.nextNode = node;
            Tail = node;
        }
        Count++;
        return Tail;
    }

    public void RemoveNode(DoubleLinkedListNode<T> node)
    {
        if(node == null || Count == 0) return;
        if (node == Head)
        {
            Head = Head.nextNode;
        }
        
        if (node == Tail)
        {
            Tail = Tail.preNode;
        }

        if (node.preNode != null)
        {
            node.preNode.nextNode = node.nextNode;
        }

        if (node.nextNode != null)
        {
            node.nextNode.preNode = node.preNode;
        }

        doubleLinkedNodePool.DeSpawn(node);
        Count--;
    }

    /// <summary>
    /// 把某个节点移动到头部
    /// </summary>
    public void MoveToHead(DoubleLinkedListNode<T> node)
    {
        if(node == null || node == Head) return;
        if(node.preNode == null && node.nextNode == null) return;
        if (node == Tail)
            Tail = node.preNode;

        if (node.nextNode != null)
            node.nextNode.preNode = node.preNode;

        if (node.preNode != null)
            node.preNode.nextNode = node.nextNode;
        
        node.nextNode = Head;
        node.preNode = null;
        Head.preNode = node;
        Head = node;
        if (Tail == null)
        {
            Tail = Head;
        }
    }
    
    private void ResetNode(DoubleLinkedListNode<T> node)
    {
        node.data = null;
        node.preNode = null;
        node.nextNode = null;
    }
}

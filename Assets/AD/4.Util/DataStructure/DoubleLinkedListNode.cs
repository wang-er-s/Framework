using System;

public class DoubleLinkedListNode<T> where T : class, new()
{
    public DoubleLinkedListNode<T> preNode = null;
    public DoubleLinkedListNode<T> nextNode = null;
    public T data = null;
}


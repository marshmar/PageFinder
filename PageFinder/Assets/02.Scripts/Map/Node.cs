using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    private Node before;
    private Node next;

    public Node Before { get => before; set => before = value; }
    public Node Next { get => next; set => next = value; }

    public Node(Node before, Node next)
    {
        this.before = before;
        this.next = next;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

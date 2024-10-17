using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    public bool isTrain;
    public LinkedList<Cart> connectedCarts; 
    //private LinkedListNode<Cart> self;
    // Start is called before the first frame update
    void Start()
    {
        if(isTrain){
            connectedCarts = new LinkedList<Cart>();
            connectedCarts.AddFirst(gameObject.GetComponent<Cart>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

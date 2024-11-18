using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class Track : MonoBehaviour
{
    public int cartLimit;
    public LinkedList<Cart> cartsInTrack;
    public Cart first,second,third,fourth,fifth;
    // Start is called before the first frame update
    void Start()
    {
        if(cartLimit>=3){
            cartsInTrack.AddFirst(first);
            cartsInTrack.AddLast(second);
            cartsInTrack.AddLast(third);
        }
        if(cartLimit==5){
            cartsInTrack.AddLast(fourth);
            cartsInTrack.AddLast(fifth);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
/*
    void FixedUpdate(){

    }
*/
    void OnCollisionEnter(Collision col){
        Debug.Log("Function Entered");
        if(col.gameObject.CompareTag("Cart"))
        {
            Debug.Log("It works!");
            col.gameObject.transform.parent = gameObject.transform;
        }
    }
}

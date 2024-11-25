using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class CartLinkBuilder : MonoBehaviour
{
    [SerializeField] public List<AbstractTrainCart> list;


    // Start is called before the first frame update
    void Start()
    {
        if (list.Count != 0)
        {
            //AbstractTrainCart current = stack.Pop();
            //current.transform.position = gameObject.transform.position;
            while (list.Count > 0)
            {
                
            }
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class CartLinkBuilder : MonoBehaviour
{
    public LENGTH length = LENGTH.THREE;
    public Transform lastRodTransform;
    //public AbstractTrainCart first, second, third, fourth, fifth;
    [SerializeField] public List<AbstractTrainCart> list;


    // Start is called before the first frame update
    void Start()
    {
        lastRodTransform = gameObject.transform;
        switch (length)
        {
            case LENGTH.THREE:
                
                break;
            case LENGTH.FIVE:
                // code block
                break;
            default:
                // code block
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public enum LENGTH : int
{
    THREE = 3,
    FIVE = 5
}
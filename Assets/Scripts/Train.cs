using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Linq;
using Unity.VisualScripting;
public class Train : MonoBehaviour
{
    public SplineContainer splineContainer;
    private UnityEngine.Vector3 trainPos;
    private UnityEngine.Vector3 nearestWorldPosition;
    private ArrayList splines;
    private Spline[] splinesArray;
    // Start is called before the first frame update
    void Start()
    {
        splinesArray = splineContainer.Splines.ToArray();
    }
/*
    // Update is called once per frame
    void Update()
    {
        
    }
    */

    void FixedUpdate(){
        
        //gameObject.transform.position = nearestWorldPosition;

    /* Vector3 localSplinePoint = _splineContainer.transform.InverseTransformPoint(trainPos);
    SplineUtility(_splineContainer, localSplinePoint, out float3 nearestPoint, out float normalizedPosition);
    Vector3 nearestWorldPosition = _splineContainer.transform.TransformPoint(nearestPoint); */

    //SplineUtility.GetNearestPoint(spline, localSplinePoint, out nearestWorldPosition,
    }

    void LateUpdate(){
        SnapTrainToTrack();
    }

    void OnCollisionEnter(Collision col){
        Debug.Log("Function Entered");
        if(col.gameObject.CompareTag("Cart"))
        {
            Debug.Log("It works!");
            col.gameObject.transform.parent = gameObject.transform;
        }
    }

/*
    void SnapTrainToTrack(){
        trainPos = gameObject.transform.position;
        UnityEngine.Vector3 splineTrainPos = splineContainer.transform.InverseTransformPoint(trainPos);
        SplineUtility.GetNearestPoint<Spline>(splineContainer.Spline, splineTrainPos, out float3 nearestPoint, out float normalizedPosition);
        nearestWorldPosition = splineContainer.transform.TransformPoint(nearestPoint);
        if(nearestWorldPosition!=null){
            gameObject.transform.position = nearestWorldPosition;
        }
    }
*/
    void SnapTrainToTrack(){
        trainPos = gameObject.transform.position;
        UnityEngine.Vector3 splineTrainPos = splineContainer.transform.InverseTransformPoint(trainPos);
        UnityEngine.Vector3 currNearestV3 = new UnityEngine.Vector3(0,0,0);
        float currNearestDist = 0;
        bool hasLooped = false;
        foreach (Spline s in splinesArray){
            SplineUtility.GetNearestPoint<Spline>(s, splineTrainPos, out float3 nearestPoint, out float normalizedPosition);
            float dist = UnityEngine.Vector3.Distance(nearestPoint, splineTrainPos);
            if(!hasLooped || dist<currNearestDist){
                currNearestV3 = nearestPoint;
                currNearestDist = dist;
                hasLooped = true;
            }
        }
        nearestWorldPosition = splineContainer.transform.TransformPoint(currNearestV3);
        nearestWorldPosition = new UnityEngine.Vector3(nearestWorldPosition.x,nearestWorldPosition.y+0.05f,nearestWorldPosition.z);
        gameObject.transform.position = nearestWorldPosition;
    }
}

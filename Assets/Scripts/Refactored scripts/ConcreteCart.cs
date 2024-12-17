using UnityEngine;
using System;
using System.Linq;
using Oculus.Interaction;
using UnityEngine.Splines;
using Unity.Mathematics;

public class ConcreteCart : AbstractTrainCart
{
    // Start is called before the first frame update
    void Start()
    {
        GetSpline();
        StartCouple();
        FindAttachTransforms();
        grabbable = GetComponent<Grabbable>();
    }

    private void StartCouple()
    {
        if (next == null) //om inte uppdaterad genom inspectorn
        {
            CoupleNext(new NullCart()); //"koppla" med NullCart
        }
        else
        {
            CoupleNext(next); //annars, koppla med nästa
        }
        if (prev == null) //om inte uppdaterad genom inspectorn
        {
            CoupleNext(new NullCart()); //"koppla" med NullCart
        }
        else
        {
            CoupleNext(next); //annars, koppla med nästa
        }
    }

    new public void SnapCartToTrack()
    {
        trainPos = gameObject.transform.position;
        Vector3 nearestWorldPosition;
        Vector3 splineTrainPos = splineContainer.transform.InverseTransformPoint(trainPos);
        Vector3 currNearestV3 = new Vector3(0, 0, 0);
        Vector3 currNearestRotation = new Vector3(0, 0, 0); //trying out rotation
        float currNearestDist = 0;
        bool hasLooped = false;

        foreach (Spline s in splinesArray)
        {
            SplineUtility.GetNearestPoint<Spline>(s, splineTrainPos, out float3 nearestPoint, out float t);
            float3 tangent = SplineUtility.EvaluateTangent<Spline>(s, t); //trying out rotation
            float dist = Vector3.Distance(nearestPoint, splineTrainPos);
            if (!hasLooped || dist < currNearestDist)
            {
                currNearestV3 = nearestPoint;
                currNearestRotation = tangent; //trying out rotation
                currNearestDist = dist;
                hasLooped = true;

            }
        }
        nearestWorldPosition = splineContainer.transform.TransformPoint(currNearestV3);
        nearestWorldPosition = new Vector3(nearestWorldPosition.x, nearestWorldPosition.y + 0.05f, nearestWorldPosition.z);
        Quaternion rot = Quaternion.LookRotation(currNearestRotation, Vector3.up); //trying out rotation
        gameObject.transform.rotation = rot; //trying out rotation
        //Debug.Log(rot.ToString());
        gameObject.transform.position = nearestWorldPosition;
    }

    new public void CouplePrev(TrainCart prev)
    {//to be implemented
    }
    void Update()
    {
        SnapCartToTrack();
        if (joints.Count > 0)
        {
            for (int i = joints.Count - 1; i >= 0; i--)
            {
                if (joints[i] != null && ShouldDetachDistance(joints[i]))
                {
                    Detach();
                    break; // Exit the loop after detaching one joint
                }
            }
        }
    }
}

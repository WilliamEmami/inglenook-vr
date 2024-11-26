using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using Oculus.Interaction;

public class AbstractTrainCart : MonoBehaviour, TrainCart
{
    private ConfigurableJoint joint;
    public float detachForceThreshhold = 500f;

    [SerializeField]
    private Grabbable grabbable;
    [SerializeField] private TrainCart prev;
    [SerializeField] private TrainCart next;
    public SplineContainer splineContainer;
    private UnityEngine.Vector3 trainPos;
    private ArrayList splines;
    private Spline[] splinesArray;
    // Start is called before the first frame update
    void Start()
    {
        splinesArray = splineContainer.Splines.ToArray();
        Debug.Log(splinesArray[0]);
        //if den har en next: CoupleNext(next)
        //if den har en prev: CouplePrev(prev);
        if (grabbable == null)
        {
            grabbable = GetComponent<Grabbable>();
        }
    }
    /* 
             if (grabbable != null)
            {
                grabbable.WhenGrabEnded += OnGrabEnded;
            } 
     */
    private void OnCollisionEnter(Collision collision)
    {
        /*
        TrainComponent otherComponent = collision.gameObject.GetComponent<TrainComponent>();
        if (otherComponent != null && !IsConnected())
        {
            CreateJoint(otherComponent.GetComponent<Rigidbody>());
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (joint != null && ShouldDetach())
        {
            Detach();
        }
        */
    }

    void LateUpdate()
    {
        SnapCartToTrack();
    }

    public void SnapCartToTrack()
    {
        trainPos = gameObject.transform.position;
        Vector3 nearestWorldPosition;
        Vector3 splineTrainPos = splineContainer.transform.InverseTransformPoint(trainPos);
        Vector3 currNearestV3 = new Vector3(0, 0, 0);
        Vector3 currNearestRotation = new Vector3(0,0,0); //trying out rotation
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
        Debug.Log(rot.ToString());
        gameObject.transform.position = nearestWorldPosition;
    }
    private void CreateJoint(Rigidbody connectedBody)
    {
        joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = connectedBody;
        SetupJointLimits();
    }
    private void SetupJointLimits()
    {
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        var spring = joint.linearLimitSpring;
        spring.spring = 1000f;
        spring.damper = 50f;
        joint.linearLimitSpring = spring;
    }
    /*
    private bool ShouldDetach()
    {
        
        return Vector3.Magnitude(joint.currentForce) > detachForceThreshold;
        
    }
    */
    public void Detach()
    {
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
            // Optionally apply a separating force
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
        }
    }
    public bool IsConnected()
    {
        return joint != null && joint.connectedBody != null;
    }
    /*
    private void OnGrabEnded(GrabbableArgs args)
    {
        // Check if the cart was thrown with enough force to detach
        Rigidbody rb = GetComponent<Rigidbody>();
        if (args.GrabData.ThrowVelocity.magnitude > detachForceThreshhold)
        {
            Detach();
        }
    }

    private void OnDestroy()
    {
        if (grabbable != null)
        {
            grabbable.WhenGrabEnded -= OnGrabEnded;
        }
    }
    */
    public void SplitTrain()
    {
        //do something
    }

    public void CoupleNext(TrainCart next)
    {
        //do something
    }

    public void CouplePrev(TrainCart prev)
    {
        //do something
    }
/*
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Function Entered");
        if (col.gameObject.CompareTag("Cart"))
        {
            Debug.Log("It works!");
            //col.gameObject.transform.parent = gameObject.transform;
        }
    }
    */
}

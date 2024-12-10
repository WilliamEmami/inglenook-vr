using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Linq;
using Oculus.Interaction;
using System.Collections.Generic;


public class AbstractTrainCart : MonoBehaviour, TrainCart, ITransformer
{
    private ConfigurableJoint joint;
    public float detachForceThreshhold = 0.1f;
    private Grabbable grabbable;
    [SerializeField] private TrainCart prev;
    [SerializeField] private TrainCart next;
    public SplineContainer splineContainer;
    private UnityEngine.Vector3 trainPos;
    private ArrayList splines;
    private Spline[] splinesArray;
    private List<ConfigurableJoint> joints = new List<ConfigurableJoint>();
    private Transform[] allChildren;
    private Transform rodTransform, handTransform;
    // Start is called before the first frame update
    public void Start()
    {
        splinesArray = splineContainer.Splines.ToArray();
        Debug.Log(splinesArray[0]);
        //if den har en next: CoupleNext(next)
        //if den har en prev: CouplePrev(prev);
        allChildren = gameObject.GetComponentsInChildren<Transform>();
        rodTransform = allChildren.FirstOrDefault(c => c.gameObject.name == "rodTransform");
        handTransform = allChildren.FirstOrDefault(c => c.gameObject.name == "HandTransform");
        if (grabbable == null)
        {
            grabbable = GetComponent<Grabbable>();
        }
    
     
     }
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.name);
        /*
        TrainComponent otherComponent = collision.gameObject.GetComponent<TrainComponent>();
        if (otherComponent != null && !IsConnected())
        {
            CreateJoint(otherComponent.GetComponent<Rigidbody>());
        }
        */
        AbstractTrainCart otherComponent = collision.gameObject.GetComponentInParent<AbstractTrainCart>();
        Transform otherTransform = collision.transform;
        float distToHand = Vector3.Distance(handTransform.position, otherTransform.position);
        float distToRod = Vector3.Distance(rodTransform.position, otherTransform.position);
        if (distToHand > distToRod){
            next = otherComponent;
        } else{
            prev = otherComponent;
        }
        Debug.Log("othercomp is: " + otherComponent);
        if (otherComponent != null)
        {
            Debug.Log("if sats för joints nådd");
            CreateJoint(otherComponent.GetComponent<Rigidbody>());
        }
        else{
            Debug.Log("if sats ej nådd. isconn: " + IsConnected());
        }
    }

    // Update is called once per frame
   public void Update()
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

    /* public void LateUpdate()
    {
        SnapCartToTrack();
    } */

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
    ConfigurableJoint newJoint = gameObject.AddComponent<ConfigurableJoint>();
    newJoint.connectedBody = connectedBody;
    SetupJointLimits(newJoint);
    joints.Add(newJoint);
    Debug.Log("Joint created with: " + connectedBody.name);
}
   private void SetupJointLimits(ConfigurableJoint joint)
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
    
  /*   private bool ShouldDetach()
    {
        
      //  return joint != null && Vector3.Magnitude(joint.currentForce) > detachForceThreshhold;
      if (joint == null) return false;
        float currentForceMagnitude = Vector3.Magnitude(joint.currentForce);
        Debug.Log("Current force: " + currentForceMagnitude);
        return currentForceMagnitude > detachForceThreshhold;
        
    } */
    private bool ShouldDetachDistance(ConfigurableJoint joint)
{
    if (joint == null || joint.connectedBody == null)
        return false;

    float distance = Vector3.Distance(transform.position, joint.connectedBody.transform.position);
    Debug.Log($"Distance for joint: {distance}");
    float distanceThreshold = 3.0f;

    return distance > distanceThreshold;
}
    
   /*  public void Detach()
    {
        if (joint != null)
        {
            Debug.Log("Detaching Joint...");
            Destroy(joint);
            joint = null;
            // Optionally apply a separating force
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
            Debug.Log("joint destroy force applied");
        }
    } */
   public void Detach()
{
    for (int i = joints.Count - 1; i >= 0; i--)
    {
        if (ShouldDetachDistance(joints[i]))
        {
            Destroy(joints[i]);
            joints.RemoveAt(i);
            Debug.Log($"Joint {i} destroyed due to distance");
        }
    }
}
   public bool IsConnected()
{
    return joints.Any(j => j != null && j.connectedBody != null);
}

    public void BeginTransform()
    {

    }

    public void UpdateTransform()
{
    foreach (var joint in joints)
    {
        if (joint != null && ShouldDetachDistance(joint))
        {
            Detach();
            break; // Exit the loop after detaching one joint
        }
    }
}

public void EndTransform()
{
    foreach (var joint in joints)
    {
        if (ShouldDetachDistance(joint))
        {
            Detach();
            break; // Exit the loop after detaching one joint
        }
    }
}
        /* Rigidbody rb = GetComponent<Rigidbody>();
        Debug.Log("rb velocity: " + rb.velocity.magnitude);
        if (rb.velocity.magnitude > detachForceThreshhold)
        {
            Detach();
        } */
    

    
   
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

    public void Initialize(IGrabbable grabbable)
    {
        throw new System.NotImplementedException();
    } 
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


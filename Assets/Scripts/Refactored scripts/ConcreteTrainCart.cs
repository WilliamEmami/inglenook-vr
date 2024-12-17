using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Linq;
using Oculus.Interaction;
using System.Collections.Generic;
using System;
using TMPro;
using System.Threading;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;


public class ConcreteTrainCart : MonoBehaviour, ITransformer
{
    protected ConfigurableJoint joint;
    public float detachForceThreshhold = 0.1f;    // Behövs endast om vi vill ha en force för redundancy
    protected Grabbable grabbable;
    [SerializeField] protected ConcreteTrainCart prev, next;
    protected SplineContainer splineContainer;
    protected UnityEngine.Vector3 trainPos;
    protected ArrayList splines;
    protected Spline[] splinesArray;
    protected List<ConfigurableJoint> joints = new List<ConfigurableJoint>();
    protected Transform[] allChildren;
    protected Transform rodTransform, handTransform;
    [SerializeField] protected bool isTrain = false;
    [SerializeField] protected bool isEndTrack = false;
    public TextMeshPro TMP;
    private NullCart nullcart = new NullCart();
    // Start is called before the first frame update
    void Start()
    {
        GetSpline();
        StartCouple();
        FindAttachTransforms();
        grabbable = GetComponent<Grabbable>();
        //Debug.Log("next: "+next.GetType()+", prev: "+prev.GetType());

    }

    private void StartCouple()
    {
        if (isTrain)
        {
            //CoupleNext(nc);
            //next = nc;
        }
        else
        {
            if (next == null) //om inte uppdaterad genom inspectorn
            {
                //CoupleNext(nc); //"koppla" med NullCart
                //next = nc;
            }
            else
            {
                CoupleNext(next); //annars, koppla med nästa
            }
        }

        if (prev == null) //om inte uppdaterad genom inspectorn
        {
            //CouplePrev(nc); //"koppla" med NullCart
            //next = nc;
        }
        else
        {
            CouplePrev(prev); //annars, koppla med nästa
        }

    }
    public void OnCollisionEnter(Collision collision)
    {

        //Debug.Log("Collision with: " + collision.gameObject.name);
        ConcreteTrainCart otherComponent = collision.gameObject.GetComponentInParent<ConcreteTrainCart>(); //få objectet vi kolliderar med
        //Debug.Log("othercomponent is:" + otherComponent);

        Transform otherTransform = collision.gameObject.transform; //få transformen för objektet vi kolliderade med
        float distToHand = Vector3.Distance(handTransform.position, otherTransform.position); //jämför för att se om det är frontalkrock eller bakom
        float distToRod = Vector3.Distance(rodTransform.position, otherTransform.position);

        if (distToHand > distToRod)
        {
            CoupleNext(otherComponent);
        }
        else
        {
            CouplePrev(otherComponent);
        }

        //CreateJoint(otherComponent.GetComponent<Rigidbody>()); //skapa joint med kolliderande cart
    }

    // Update is called once per frame
    void Update()
    {
        SnapCartToTrack();
        if (joints.Count > 0)
        {
            for (int i = joints.Count - 1; i >= 0; i--)
            {
                if (joints[i] != null && ShouldDetachDistance(joints[i]))
                {
                    Debug.Log("Detach Entered");
                    Detach();
                    break; // Exit the loop after detaching one joint
                }
            }
        }
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
        gameObject.transform.position = nearestWorldPosition;
    }
    public void CreateJoint(Rigidbody connectedBody) //skapa en joint och lägg i lista
    {
        ConfigurableJoint newJoint = gameObject.AddComponent<ConfigurableJoint>();
        newJoint.connectedBody = connectedBody;
        SetupJointLimits(newJoint);
        joints.Add(newJoint);
        // Debug.Log("Joint created with: " + connectedBody.name);
    }
    public void SetupJointLimits(ConfigurableJoint joint) //inställningar för joints, ändra för mer verklighetstrogen / bättre interaktion för användaren
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

        //  return joint != null && Vector3.Magnitude(joint.currentForce) > detachForceThreshhold; //gammal detach som kör på kraft
        if (joint == null) return false;
          float currentForceMagnitude = Vector3.Magnitude(joint.currentForce);
          Debug.Log("Current force: " + currentForceMagnitude);
          return currentForceMagnitude > detachForceThreshhold;

      } */
    protected bool ShouldDetachDistance(ConfigurableJoint joint) //kollar avståndet mellan objekt för att se om detach ska köras
    {
        if (joint == null || joint.connectedBody == null)
            return false;

        float distance = Vector3.Distance(transform.position, joint.connectedBody.transform.position);
        // Debug.Log($"Distance for joint: {distance}");
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
             // Optionally apply a separating force, gammal, använder kraft istället för avstånd, ev. viktig om snaptospline kärvar
             Rigidbody rb = GetComponent<Rigidbody>();
             rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
             Debug.Log("joint destroy force applied");
         }
     } */
    public void Detach() //förstör jointen och tar den ur listan
    {
        float distToHand;
        float distToRod;
        for (int i = joints.Count - 1; i >= 0; i--)
        {


            if (ShouldDetachDistance(joints[i]))
            {
                Transform otherTransform = joints[i].transform; //få transformen för objektet vi kolliderade med
                distToHand = Vector3.Distance(handTransform.position, otherTransform.position); //jämför för att se om det är frontalkrock eller bakom
                distToRod = Vector3.Distance(rodTransform.position, otherTransform.position);
                if (distToHand > distToRod)
                {

                    //next = nullcart;
                    prev.next = null;
                    prev = null;
                    //Debug.Log("next är null" + next);
                }
                else
                {
                    //prev = nullcart;
                    next.prev = null;
                    next = null;
                    //Debug.Log("prev är null" + next);
                }
                TMP.text = CountCarts().ToString();
                Destroy(joints[i]);
                joints.RemoveAt(i);
                //  Debug.Log($"Joint {i} destroyed due to distance");
            }
        }


    }
    public bool IsConnected() //kolla om joint finns
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

    public void CoupleNext(ConcreteTrainCart next)
    {
        //IF ALL CONDITIONS ARE MET:
        if ((this.next == null && !isTrain))
        {
            ConcreteTrainCart nextConcrete = next;
            this.next = nextConcrete;
            nextConcrete.prev = this;
            CreateJoint(nextConcrete.GetComponent<Rigidbody>()); //skapa joint med kolliderande cart
            Debug.Log("CoupleNext Successful" + nextConcrete + nextConcrete.GetComponent<Rigidbody>());
            TMP.text = CountCarts().ToString();
        }
    }

    public void CouplePrev(ConcreteTrainCart prev)
    {
        if ((this.prev == null))
        {
            ConcreteTrainCart prevConcrete = prev;
            this.prev = prevConcrete;
            prevConcrete.next = this;
            CreateJoint(prevConcrete.GetComponent<Rigidbody>()); //skapa joint med kolliderande cart
            Debug.Log("CouplePrev Successful" + prevConcrete + prevConcrete.GetComponent<Rigidbody>());

            Debug.Log("isTrain: " + isTrain);
            Debug.Log("prev typ: " + prev.GetType());
            TMP.text = CountCarts().ToString();

        }


    }

    public int CountCarts()
    {
        int count = 0;
        ConcreteTrainCart iterator = Game.INSTANCE.GetTrain();
        while (iterator.prev != null)
        {
            Debug.Log("while entered");
            iterator = iterator.prev;
            count++;
        }
        Debug.Log(count);
        return count;

    }

    public void Initialize(IGrabbable grabbable)
    {
        throw new System.NotImplementedException();
    }
    protected void FindAttachTransforms() //hittar rod/hand-transform för objekten
    {
        allChildren = gameObject.GetComponentsInChildren<Transform>();
        rodTransform = Array.Find<Transform>(allChildren, c => c.gameObject.name == "RodTransform");
        Debug.Log("rodT i findattach är" + rodTransform);
        handTransform = Array.Find<Transform>(allChildren, c => c.gameObject.name == "HandTransform");
        Debug.Log("handT i findattach är" + handTransform);
    }
    protected void GetSpline()
    {
        splinesArray = Game.INSTANCE.GetSplinesArray();
        splineContainer = Game.INSTANCE.GetSplineContainer();
    }
}

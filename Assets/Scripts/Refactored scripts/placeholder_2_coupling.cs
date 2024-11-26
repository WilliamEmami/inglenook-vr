/* using UnityEngine;
using Meta.XR;
 
public class TrainComponent : MonoBehaviour
{
    private ConfigurableJoint joint;
    public float detachForceThreshold = 500f;
 
    [SerializeField]
    private OVRGrabbable grabbable;
 
    private void Start()
    {
        if (grabbable == null)
        {
            grabbable = GetComponent<OVRGrabbable>();
        }
 
        if (grabbable != null)
        {
            grabbable.OnGrabEnd.AddListener(OnGrabEnded);
        }
    }
 
    private void OnCollisionEnter(Collision collision)
    {
        TrainComponent otherComponent = collision.gameObject.GetComponent<TrainComponent>();
        if (otherComponent != null && !IsConnected())
        {
            CreateJoint(otherComponent.GetComponent<Rigidbody>());
        }
    }
 
    private void Update()
    {
        if (joint != null && ShouldDetach())
        {
            Detach();
        }
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
 
    private bool ShouldDetach()
    {
        return Vector3.Magnitude(joint.currentForce) > detachForceThreshold;
    }
 
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
 
    private void OnGrabEnded()
    {
        // Check if the cart was thrown with enough force to detach
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb.velocity.magnitude > detachForceThreshold)
        {
            Detach();
        }
    }
 
    private void OnDestroy()
    {
        if (grabbable != null)
        {
            grabbable.OnGrabEnd.RemoveListener(OnGrabEnded);
        }
    }
} */
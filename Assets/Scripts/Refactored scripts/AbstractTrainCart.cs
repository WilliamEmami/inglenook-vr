using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

public abstract class AbstractTrainCart : MonoBehaviour, TrainCart
{
    [SerializeField] private TrainCart prev;
    [SerializeField] private TrainCart next;
    public SplineContainer splineContainer;
    private UnityEngine.Vector3 trainPos;
    private UnityEngine.Vector3 nearestWorldPosition;
    private ArrayList splines;
    private Spline[] splinesArray;
    // Start is called before the first frame update
    void Start()
    {
        splinesArray = splineContainer.Splines.ToArray();
        //if den har en next: CoupleNext(next)
        //if den har en prev: CouplePrev(prev);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        SnapCartToTrack();
    }

    public void SnapCartToTrack()
    {
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

    public void SplitTrain()
    {
        //do something
    }

    public void CoupleNext(TrainCart next)
    {
        //do something
    }

    public void CouplePrev(TrainCart prev){
        //do something
    }

    void OnCollisionEnter(Collision col){
        Debug.Log("Function Entered");
        if(col.gameObject.CompareTag("Cart"))
        {
            Debug.Log("It works!");
            //col.gameObject.transform.parent = gameObject.transform;
        }
    }
}

using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class Game : MonoBehaviour
{
    public static Game INSTANCE { get; private set; }
    public SplineContainer splineContainer;
    public ConcreteTrainCart train;
    public TextMeshPro tmp;
    private Spline[] splinesArray;
    public Spline[] GetSplinesArray()
    {
        return splinesArray;
    }

    public SplineContainer GetSplineContainer()
    {
        return splineContainer;
    }

    public ConcreteTrainCart GetTrain(){
        return train;
    }

    public TextMeshPro GetTMP(){
        return tmp;
    }

    private void Awake()
    {
        if (INSTANCE != null && INSTANCE != this)
        {
            Destroy(this);
        }
        else
        {
            INSTANCE = this;
            splinesArray = splineContainer.Splines.ToArray();
        }
    }
}

using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class Game : MonoBehaviour
{
    public static Game INSTANCE { get; private set; }
    public SplineContainer splineContainer;
    private Spline[] splinesArray;
    public Spline[] GetSplinesArray()
    {
        return splinesArray;
    }

    public SplineContainer GetSplineContainer()
    {
        return splineContainer;
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

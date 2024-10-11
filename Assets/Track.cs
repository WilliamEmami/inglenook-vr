using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class Track : MonoBehaviour
{
    public OneGrabTranslateTransformer train;
    //public Grabbable grabbable;
    public bool dirX = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonPress(){
        if(dirX){
            train.Constraints.MinX.Value = 0;
            train.Constraints.MaxX.Value = 0;
            train.Constraints.MinZ.Value = -0.4f;
            train.Constraints.MaxZ.Value = 0.4f;
            //grabbable.InjectOptionalOneGrabTransformer(train);
            //train.transform.parent.transform.rotation.
        } else{
            train.Constraints.MinX.Value = -0.4f;
            train.Constraints.MaxX.Value = 0.4f;
            train.Constraints.MinZ.Value = 0;
            train.Constraints.MaxZ.Value = 0;
            //grabbable.InjectOptionalOneGrabTransformer(train.GetComponent<OneGrabTranslateTransformer>());
        }
    }
}

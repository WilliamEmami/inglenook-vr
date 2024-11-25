using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullCart : TrainCart
{
    public void CoupleNext(TrainCart next){}
    public void CouplePrev(TrainCart prev){}

    public void SnapCartToTrack(){}

    public void SplitTrain(){}
}

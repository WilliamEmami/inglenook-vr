using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullCart : ConcreteTrainCart
{
    public void CoupleNext(ConcreteTrainCart next){}
    public void CouplePrev(ConcreteTrainCart prev){}
    public void SnapCartToTrack(){}
    public void SplitTrain(){}

    public int CountCarts(){
        return 0;
    }
}

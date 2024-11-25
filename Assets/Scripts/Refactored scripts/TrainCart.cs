using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TrainCart
{
    public void SnapCartToTrack(); //körs i LateUpdate() för att hålla carts på track
    public void SplitTrain(); //om användaren sliter sär 2 adjacent tåg ska man splitta tåget. Sätt next till nullcart på ena och prev till nullcart på andra. 
    //Iterera för att kolla vilken som är next respektive prev
    public void CoupleNext(TrainCart next); //lägg till next till nuvarande traincart
    public void CouplePrev(TrainCart prev); //lägg till prev till nuvarande traincart
}

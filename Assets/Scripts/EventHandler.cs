//using UnityEngine;
//using System.Collections;
//using UnityEngine.Events;
//using System;

//public class UnityEventHandler : MonoBehaviour {

//    public UnityEvent OnPlayerFailedDown;
//    public UnityEvent OnDiamondPicked;
//    public UnityEvent OnNewLevelLoaded;

//    // Use this for initialization
//    void Start () {
//        PlayerController.OnFailedDown += () => OnPlayerFailedDown.Invoke();
//        PlayerController.OnDiamondPicked += path => OnDiamondPicked.Invoke();
//        MapGenerator.OnNewLevelLoaded += d => OnNewLevelLoaded.Invoke();
//    }
//}

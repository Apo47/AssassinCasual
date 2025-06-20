using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    // Start is called before the first frame update
    
        void Start()
        {
            // Google AdMob'u baþlat
            MobileAds.Initialize(initStatus => { });
        }
    
}

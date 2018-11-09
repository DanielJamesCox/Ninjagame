using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightswitchBehavior : MonoBehaviour {

    public bool destructible = false;
    LightSource lightSprite;


    public void Start()
    {
        lightSprite = transform.parent.GetComponent<LightSource>();
    }

    public void ToggleLight() {
        if (destructible) Destroy(lightSprite);
        else lightSprite.ToggleLights();
    }
}

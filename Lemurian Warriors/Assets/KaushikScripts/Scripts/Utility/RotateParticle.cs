using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateParticle : MonoBehaviour
{
    //didn't work too well
    public ParticleSystem pSys;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var main = pSys.main;
        main.startRotation = gameObject.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
    }
}

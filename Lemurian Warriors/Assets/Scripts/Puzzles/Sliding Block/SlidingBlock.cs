using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingBlock : MonoBehaviour
{
    //public GameObject playerObject;

    //public Transform pedestal;
    public GameObject pedestal;

    bool isActive = false;


    public float moveSpeed = 3.0f;

    //Vector3 moveDirection;

    // Update is called once per frame
    void Update()
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, pedestal.transform.position, step);

        /*if(Vector3.Distance(transform.position, pedestal.transform.position) < 0.001f)
        {
            pedestal.transform.position = new Vector3(5.0f, -0.08f, -6.0f);
            
        }*/

    }


    /*private void OnMouseDown()
    {
        moveDirection = new Vector3(0f, 0f, transform.position * moveSpeed * Time.deltaTime);
    }*/



}

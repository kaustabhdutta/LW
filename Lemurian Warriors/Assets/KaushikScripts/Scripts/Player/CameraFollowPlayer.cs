using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public NewPlayerController ToFollow;
    public Vector3 vectorToPlayer = new Vector3(1, -1, 0);
    [Range(0, 1)]
    public float FollowMovementScale;
    public float maxDistanceToPlayer;
    public float minDistanceToPlayer;
    float distanceToPlayer;
    private Vector3 Displacement;
    [Range(0.1f, 1)]
    public float smoothFactor;

    private void Start()
    {
        Physics.IgnoreLayerCollision(12, 13);
        Physics.IgnoreLayerCollision(9, 11);
        distanceToPlayer = maxDistanceToPlayer;
        transform.rotation = Quaternion.LookRotation(vectorToPlayer);
        transform.position = ToFollow.transform.position - vectorToPlayer / vectorToPlayer.magnitude * distanceToPlayer;
    }
    private void LateUpdate()
    {
        Vector3 newPos = ToFollow.transform.position - vectorToPlayer.normalized * distanceToPlayer;
        transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
        //transform.rotation = Quaternion.LookRotation(ToFollow.transform.position - transform.position, Vector3.Cross(Vector3.Cross(ToFollow.transform.position - transform.position, Vector3.up), ToFollow.transform.position - transform.position));
    }

    // Start is called before the first frame update
    /*
    void Start()
    {
        distanceToPlayer = maxDistanceToPlayer;
        transform.rotation = Quaternion.LookRotation(vectorToPlayer);
        transform.position = ToFollow.transform.position - vectorToPlayer / vectorToPlayer.magnitude * distanceToPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (ToFollow.canAct)
        {
            if (ToFollow.NavAgent.velocity != null)
            {
                Displacement = ToFollow.NavAgent.velocity * FollowMovementScale - VDistanceInDirection((ToFollow.NavAgent.velocity * FollowMovementScale), Vector3.up) * Vector3.up;
                transform.position = ToFollow.transform.position + Displacement - vectorToPlayer / vectorToPlayer.magnitude * distanceToPlayer;
            }
            //Debug.Log((ToFollow.NavAgent.velocity).magnitude);
        }
        else
        {
            transform.position = ToFollow.transform.position - vectorToPlayer.normalized * distanceToPlayer;
        }
    }
    public float VDistanceInDirection(Vector3 vector, Vector3 direction)
    {
        return (Vector3.Dot(vector, direction) / direction.magnitude);
    }*/
}
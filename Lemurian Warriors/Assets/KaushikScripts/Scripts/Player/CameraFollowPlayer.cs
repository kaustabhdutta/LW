using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public NewPlayerController toFollow;
    public Vector3 vectorToPlayer = new Vector3(1, -1, 0);
    [Range(0, 1)]
    public float FollowMovementScale;
    public float maxDistanceToPlayer;
    public float minDistanceToPlayer;
    float distanceToPlayer;
    private Vector3 Displacement;
    [Range(0.1f, 1)]
    public float smoothFactor;

    private float currentX;
    private float currentY;
    [SerializeField]
    private float camRateX;
    [SerializeField]
    private float camRateY;
    [SerializeField]
    private float yAngleMin;
    [SerializeField]
    private float yAngleMax;

    private void Start()
    {
        Physics.IgnoreLayerCollision(12, 13);
        Physics.IgnoreLayerCollision(9, 11);
        distanceToPlayer = maxDistanceToPlayer;
        transform.rotation = Quaternion.LookRotation(vectorToPlayer);
        transform.position = toFollow.transform.position - vectorToPlayer / vectorToPlayer.magnitude * distanceToPlayer;
        InputController3rdP.current.mouseMovement = UpdateCam;

    }
    private void LateUpdate()
    {
        if (InputController.current.enabled)
        {
            Vector3 newPos = toFollow.transform.position - vectorToPlayer.normalized * distanceToPlayer;
            transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
        }
        //transform.rotation = Quaternion.LookRotation(ToFollow.transform.position - transform.position, Vector3.Cross(Vector3.Cross(ToFollow.transform.position - transform.position, Vector3.up), ToFollow.transform.position - transform.position));
    }
    private void UpdateCam(float inX, float inY)
    {
        currentX += inX;
        currentY -= inY;

        currentY = Mathf.Clamp(currentY, yAngleMin, yAngleMax);
        Vector3 dir = new Vector3(0, 0, -distanceToPlayer);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = toFollow.transform.position + rotation * dir;

        transform.LookAt(toFollow.transform.position);
    }
}
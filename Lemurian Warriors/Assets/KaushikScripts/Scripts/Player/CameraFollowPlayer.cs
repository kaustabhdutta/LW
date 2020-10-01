using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraFollowPlayer : MonoBehaviour
{
    public NewPlayerController toFollow;
    public Vector3 vectorToPlayer = new Vector3(1, -1, 0);
    [Range(0, 1)]
    public float FollowMovementScale;
    public float maxDistanceToPlayer;
    public float minDistanceToPlayer;
    public float aimMaxDist;
    public float aimMinDist;
    float distToPlayer;
    float aimDistToPlayer;
    float absDistToPlayer;
    bool aiming;
    private Vector3 Displacement;
    [Range(0.1f, 1)]
    public float smoothFactor;

    private float currentX;
    private float currentY;
    [SerializeField]
    private float camRateX = 1;
    [SerializeField]
    private float camRateY = 1;
    [SerializeField]
    private float yAngleMin;
    [SerializeField]
    private float yAngleMax;
    [SerializeField]
    private float scrollRate = 1;
    public LayerMask blocksView;
    public LayerMask blocksProjectiles;
    [SerializeField][Range(0, 0.5f)]
    private float wallDistance;
    [Range(0, 1)]
    float distLerp = 0;
    [SerializeField][Range(.1f, 2)]
    float lerpTime = 0.1f;
    bool lerping;
    private void Start()
    {
        Physics.IgnoreLayerCollision(12, 13);
        Physics.IgnoreLayerCollision(9, 11);
        distToPlayer = maxDistanceToPlayer;
        absDistToPlayer = maxDistanceToPlayer;
        aimDistToPlayer = aimMinDist;
        transform.rotation = Quaternion.LookRotation(vectorToPlayer);
        transform.position = toFollow.camLookAt.position - vectorToPlayer / vectorToPlayer.magnitude * distToPlayer;
        InputController3rdP.current.mouseMovement = UpdateCam;

    }
    private void LateUpdate()
    {
        if (InputController.current.enabled)
        {
            Vector3 newPos = toFollow.camLookAt.position - vectorToPlayer.normalized * distToPlayer;
            transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
            transform.LookAt(toFollow.transform);
        }
        //transform.rotation = Quaternion.LookRotation(ToFollow.transform.position - transform.position, Vector3.Cross(Vector3.Cross(ToFollow.transform.position - transform.position, Vector3.up), ToFollow.transform.position - transform.position));
    }
    private void UpdateCam(float inX, float inY, float scroll, float time)
    {
        currentX += inX * camRateX;
        currentY -= inY * camRateY;
        
        if (toFollow.aiming)
        {
            aiming = true;
            distToPlayer = Mathf.Clamp(distToPlayer - scroll * scrollRate, aimMinDist, aimMaxDist);
            if(absDistToPlayer != distToPlayer)
            {
                DoDistLerping(true, time);
            }
        }
        else
        {
            aiming = false;
            aimDistToPlayer = Mathf.Clamp(aimDistToPlayer - scroll * scrollRate, minDistanceToPlayer, maxDistanceToPlayer);
            if (absDistToPlayer != aimDistToPlayer)
            {
                DoDistLerping(false, time);
            }
        }
        currentY = Mathf.Clamp(currentY, yAngleMin, yAngleMax);
        Vector3 dir = new Vector3(0, 0, -absDistToPlayer);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = toFollow.camLookAt.position + rotation * dir;
        absDistToPlayer = (toFollow.camLookAt.position - transform.position).magnitude;
        transform.LookAt(toFollow.camLookAt.position);
        RaycastHit hit;
        if(Physics.Linecast(toFollow.camLookAt.position, transform.position, out hit, blocksView)){
            transform.position = hit.point - (toFollow.camLookAt.position - transform.position).normalized * wallDistance;
        }
    }
    private void DoDistLerping(bool toAim, float deltaT)
    {
        if (toAim)
        {
            distLerp += deltaT / lerpTime;
        }
        else
        {
            distLerp -= deltaT / lerpTime;
        }
        distLerp = Mathf.Clamp(distLerp, 0, 1);
        absDistToPlayer = Mathf.Lerp(aimDistToPlayer, distToPlayer, distLerp);
        toFollow.camLookAt.localPosition = Vector3.Lerp(toFollow.defaultCamLookAt, toFollow.aimingCamLookAt.localPosition, distLerp);
    }
}
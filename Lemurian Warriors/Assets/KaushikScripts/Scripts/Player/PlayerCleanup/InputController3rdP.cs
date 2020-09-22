using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController3rdP : MonoBehaviour
{
    public static InputController3rdP current;
    Vector3 directionalInput;

    // Start is called before the first frame update
    void Start()
    {
        current = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePresent)
        {
            mouseMovement?.Invoke(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
        if (Input.GetMouseButtonDown(0))
        {
            LeftClick?.Invoke();
        }
        if (Input.GetMouseButton(1))
        {
            RightClick?.Invoke();
        }
        if (!Input.GetMouseButton(1))
        {
            RightClickUp?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            NextSpell?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PrevSpell?.Invoke();
        }
        directionalInput = Vector3.ClampMagnitude(Camera.main.transform.right * Input.GetAxis("Horizontal") + 
            VectorMath.ZeroY(Camera.main.transform.forward).normalized * Input.GetAxis("Vertical"), 1);
        Movement?.Invoke(directionalInput, Time.deltaTime);
        Debug.Log(directionalInput);
    }
    private void FixedUpdate()
    {
        
    }
    public delegate void DelNoParams();
    public delegate void MouseMovement(float inX, float inY);
    public delegate void DelVect2(Vector3 v, float time);
    public DelVect2 Movement;
    public MouseMovement mouseMovement;
    public DelNoParams LeftClick;
    public DelNoParams RightClick;
    public DelNoParams RightClickUp;
    public DelNoParams NextSpell;
    public DelNoParams PrevSpell;
}

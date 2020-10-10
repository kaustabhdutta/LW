using UnityEngine;
using UnityEngine.Events;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Roll?.Invoke();
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
        Sprint?.Invoke(Input.GetKey(KeyCode.LeftShift));

        directionalInput = Vector3.ClampMagnitude(Camera.main.transform.right * Input.GetAxis("Horizontal") + 
            VectorMath.ZeroY(Camera.main.transform.forward).normalized * Input.GetAxis("Vertical"), 1);
        Movement?.Invoke(directionalInput, Time.deltaTime);
    }
    private void LateUpdate()
    {
        if (Input.mousePresent)
        {
            mouseMovement?.Invoke(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.mouseScrollDelta.y * Time.deltaTime, Time.deltaTime);
        }
    }
    public delegate void DelNoParams();
    public delegate void MouseMovement(float inX, float inY, float scroll, float time);
    public delegate void DelVect2(Vector3 v, float time);
    public delegate void DelBool(bool b);
    public DelVect2 Movement;
    public MouseMovement mouseMovement;
    public DelNoParams LeftClick;
    public DelNoParams RightClick;
    public DelNoParams RightClickUp;
    public DelNoParams NextSpell;
    public DelNoParams PrevSpell;
    public DelBool Sprint;
    public DelNoParams Roll;
}

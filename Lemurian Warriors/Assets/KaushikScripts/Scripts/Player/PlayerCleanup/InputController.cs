using UnityEngine;

public class InputController : MonoBehaviour
{
    int currentSpell = -1;
    public static InputController current;
    void Start()
    {
        current = this;
        //make this the only input controller that is actually referenced to avoid double actions and stuff.
    }

    void Update()
    {
        //binding each action to each key.
        if (Input.GetMouseButtonDown(0))
        {
            LeftClick();
        }
        if (Input.GetMouseButton(1))
        {
            RightClick();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Space();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Sprint(true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Sprint(false);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectSpell(0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SelectSpell(1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectSpell(2);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SelectSpell(3);
        }
        if ((Input.GetKeyUp(KeyCode.Q) && currentSpell == 0) ||
            (Input.GetKeyUp(KeyCode.W) && currentSpell == 1) ||
            (Input.GetKeyUp(KeyCode.E) && currentSpell == 2) ||
            (Input.GetKeyUp(KeyCode.R) && currentSpell == 3))
        {
            SelectSpell(-1);
        }
    }
    public delegate void DelNoParams();
    public DelNoParams onRightClick;
    public DelNoParams onLeftClick;
    public DelNoParams onSpace;

    public delegate void DelInt(int val);
    public DelInt onSelectSpell;

    public delegate void DelBool(bool val);
    public DelBool onSprint;

    public void RightClick()
    {
        onRightClick?.Invoke();
    }
    public void Space()
    {
        onSpace?.Invoke();
    }
    public void LeftClick()
    {
        onLeftClick?.Invoke();
    }
    public void SelectSpell(int spell)
    {
        onSelectSpell?.Invoke(spell);
        currentSpell = spell;
    }
    public void Sprint(bool sprint)
    {
        onSprint?.Invoke(sprint);
    }
}
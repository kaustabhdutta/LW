using UnityEngine;

public class FootageEnemyAnimEvents : MonoBehaviour
{
    // Start is called before the first frame update
    public FootageEnemy toCall;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ActivateAttack(int atk)
    {
        toCall.ActivateAttack(atk);
    }
    void DeactivateAttack(int atk)
    {
        toCall.DeactivateAttack(atk);
    }
    void CanActTrue()
    {
        toCall.CanActTrue();
    }
    void CanActFalse()
    {
        toCall.CanActFalse();
    }
    void BackToIdle()
    {
        toCall.BackToIdle();
    }
}

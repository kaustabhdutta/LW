using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPlayer : MonoBehaviour
{

    public static TargetPlayer instance;
    void Awake()
    {
        instance = this;
    }
    public GameObject Player;
}

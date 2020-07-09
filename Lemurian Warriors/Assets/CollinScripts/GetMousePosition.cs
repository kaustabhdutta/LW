using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMousePosition : MonoBehaviour
{
    public static Vector3 FindMousePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 location = Vector3.zero;
        if (Physics.Raycast(ray.origin, ray.direction * 1000f, out hit))
        {
            location = hit.point;
        }
        return location;
    }
}

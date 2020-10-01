using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralUtil : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static bool Has<comp>(GameObject obj)
    {
        return obj.GetComponent<comp>() != null;
    }
    public static IEnumerator WaitForFrames(int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return 0;
        }
    }

    public RaycastHit DoLineCast(Ray ray, float distance, int layerMask)
    {
        RaycastHit hit;
        Physics.Linecast(ray.origin, ray.origin + ray.direction * distance, out hit, layerMask);
        return hit;
    }
    public RaycastHit DoLineCast(Vector3 start, Vector3 end, float distance, int layerMask)
    {
        RaycastHit hit;
        Physics.Linecast(start, end, out hit, layerMask);
        return hit;
    }
}

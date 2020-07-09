using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
    
{
    objectPooler ObjectPooler;
    
  
    // Start is called before the first frame update
    void Start()
    {
        ObjectPooler=objectPooler.Instance;
    }
   
    // Update is called once per frame
 
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        ObjectPooler.SpawnFromPool("enemy", transform.position, Quaternion.identity);
        // {
        // InvokeRepeating("Spawner", 0.5f, 1000);
        gameObject.GetComponent<MeshCollider>().enabled = false;
        //  }
        
    }
 

}

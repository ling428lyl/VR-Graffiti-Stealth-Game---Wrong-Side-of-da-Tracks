using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTunnel : MonoBehaviour
{
    public int speed=3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0,0, speed) *Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("DestroyTunnel"))
        {
            Destroy(gameObject);
        }
    }
}

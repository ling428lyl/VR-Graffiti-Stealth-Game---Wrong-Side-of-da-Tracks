using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject sectionTriggerPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Train"))
        {
            Instantiate(sectionTriggerPrefab,new Vector3((float)-1.907349e-06, (float)0.005362979, (float)-13.61492),Quaternion.identity);
        }
    }
}

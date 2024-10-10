using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeArea : MonoBehaviour
{
    // Start is called before the first frame update
    public HandData leftHand;
    public HandData rightHand;
    public GameObject leftTypeHand;
    public GameObject rightTypeHand;

    public void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.layer == leftHand.gameObject.layer)
        //{
            HandData hand;
            hand = other.gameObject.transform.parent.transform.parent.GetComponentInChildren<HandData>();
            if (hand = null)
            {
                //Debug.Log("null");
            }
            if (other.gameObject.transform.parent.transform.parent.GetComponentInChildren<HandData>().handType == leftHand.handType)
            {
                Debug.Log("left");
                leftTypeHand.SetActive(true);
            }
            else if (other.gameObject.transform.parent.transform.parent.GetComponentInChildren<HandData>().handType == rightHand.handType)
            {
                rightTypeHand.SetActive(true);
            }
        //}

    }

    public  void OnTriggerExit(Collider other)
    {
        //if (other.gameObject.layer == leftHand.gameObject.layer)
        //{
            HandData hand = other.gameObject.transform.parent.GetComponentInChildren<HandData>();
            if (hand = null)
            {
                return;
            }
            if (other.gameObject.transform.parent.transform.parent.GetComponentInChildren<HandData>().handType == leftHand.handType)
            {
                leftTypeHand.SetActive(false);
            }
            else if (other.gameObject.transform.parent.transform.parent.GetComponentInChildren<HandData>().handType == rightHand.handType)
            {
                rightTypeHand.SetActive(false);
            }
        //}
    }
}

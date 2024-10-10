using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandData : MonoBehaviour
{
    // Start is called before the first frame update
    public enum HandModelType { Left,Right };
    public HandModelType handType;
    public Animator animator;
    public Transform[] fingerBones;
    public Transform root;
}

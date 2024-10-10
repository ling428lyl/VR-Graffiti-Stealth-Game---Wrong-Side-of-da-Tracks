using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SprayCanHands : MonoBehaviour
{
    public float poseTransitionDuration = 0.2f;

    public HandData rightHandPose; // pose of grabbing hand
    public HandData leftHandPose; // pose of grabbing hand

    private Vector3 startingHandPosition;
    private Vector3 finalHandPosition;

    private Vector3 startingHandScale;
    private Vector3 finalHandScale;

    private Quaternion finalHandRotation;
    private Quaternion startingHandRotation;

    private Vector3[] startingFingerPositions;
    private Vector3[] finalFingerPositions;

    private Vector3[] startingFingerScale;
    private Vector3[] finalFingerScale;

    private Quaternion[] startingFingerRotations;
    private Quaternion[] finalFingerRotations;

    private XRGrabInteractable grabInteractable;
    private InputAction triggerAction;
    private bool isGrabbing = false;

    public bool isTriggering = false;

    public Animator animatorRight;
    public Animator animatorLeft;
    public Animator animatorCap;

    public Transform hole;
    public float maxDistance = 5.0f;
    //public WallManager wallManager;
    public ParticleSystem sprayParticle;

    private AudioSource audioSource;
    public AudioClip clipSpray;
    public AudioClip clipRattle;

    [SerializeField] private WallManager[] wallManagers;
    [SerializeField] private TMP_Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        rightHandPose.gameObject.SetActive(false);
        leftHandPose.gameObject.SetActive(false);
        sprayParticle.Stop();
        //animatorRight.SetBool("Grabbing", isGrabbing);
        audioSource=GetComponent<AudioSource>();
    }

    private void OnGrab(BaseInteractionEventArgs arg)
    {
        isGrabbing = true;
        SetupPose(arg);
        audioSource.clip = clipRattle;
        audioSource.loop = false;
        audioSource.Play();
        //Debug.Log("arg111interactableObject.transform111=" + arg.interactorObject.transform.parent);
        
        if (arg.interactorObject is NearFarInteractor )
        {
            //Debug.Log("arg111interactableObject.transform111=" + arg.interactorObject.transform.parent.GetComponentInChildren<HandData>());

            //Enable the score text when the user grabs the spray can.
            scoreText.gameObject.SetActive(true);

			if (arg.interactorObject.transform.parent.GetComponentInChildren<HandData>().handType == HandData.HandModelType.Right)
            {
                triggerAction = new InputAction(binding: "<XRController>{RightHand}/trigger");
            }
            else
            {
                triggerAction = new InputAction(binding: "<XRController>{LeftHand}/trigger");
            }


            triggerAction.performed += OnTriggerPressed;
            triggerAction.canceled += OnTriggerReleased;
            triggerAction.Enable();
        }
    }

    private void OnRelease(BaseInteractionEventArgs arg)
    {
        audioSource.Stop();
        isGrabbing = false;
        animatorRight.SetBool("GrabCan", false);
        animatorLeft.SetBool("GrabCan", false);
        animatorCap.SetBool("GrabCan", false);
        sprayParticle.Stop();
        UnSetPose(arg);

		//Disable the score text when the user releases the spray can.
		scoreText.gameObject.SetActive(false);

		if (triggerAction != null)
        {
            
            triggerAction.performed -= OnTriggerPressed;
            triggerAction.canceled -= OnTriggerReleased;
            triggerAction.Disable();
            triggerAction = null;
        }
    }

    public void SetupPose(BaseInteractionEventArgs arg)
    {
       // Debug.Log(animatorRight.GetBool("Grabbing"));
        if (arg.interactorObject is NearFarInteractor )
        {
            Transform transform = arg.interactorObject.transform.parent;
            HandData handData = transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = false;
           // Debug.Log("animatorRight"+animatorRight.enabled);
            animatorCap.SetBool("GrabCan", true);
            if (handData.handType == HandData.HandModelType.Right)
            {
                //animatorRight.SetInteger("Grab", 1);
                animatorRight.SetBool("GrabCan",true);
                SetHandDataValues(handData, rightHandPose);
                
            }
            if (handData.handType == HandData.HandModelType.Left)
            {
                animatorLeft.SetBool("GrabCan", true);
                SetHandDataValues(handData, leftHandPose);
            }

            SetHandData(handData, finalHandPosition, finalHandRotation, finalHandScale, finalFingerRotations, finalFingerPositions, finalFingerScale);
        }
    }

    public void SetHandDataValues(HandData h1, HandData h2)
    {
        startingHandPosition = h1.root.localPosition;
        finalHandPosition = h2.root.localPosition;

        startingHandRotation = h1.root.localRotation;
        finalHandRotation = h2.root.localRotation;

        startingHandScale = h1.root.localScale;
        finalHandScale = h2.root.localScale;

        startingFingerPositions = new Vector3[h1.fingerBones.Length];
        finalFingerPositions = new Vector3[h1.fingerBones.Length];

        startingFingerRotations = new Quaternion[h1.fingerBones.Length];
        finalFingerRotations = new Quaternion[h1.fingerBones.Length];

        startingFingerScale = new Vector3[h1.fingerBones.Length];
        finalFingerScale = new Vector3[h1.fingerBones.Length];

        for (int i = 0; i < h1.fingerBones.Length; i++)
        {
            startingFingerPositions[i] = h1.fingerBones[i].localPosition;
            finalFingerPositions[i] = h2.fingerBones[i].localPosition;

            startingFingerRotations[i] = h1.fingerBones[i].localRotation;
            finalFingerRotations[i] = h2.fingerBones[i].localRotation;

            startingFingerScale[i] = h1.fingerBones[i].localScale;
            finalFingerScale[i] = h2.fingerBones[i].localScale;
        }
    }

    public void SetHandData(HandData h, Vector3 newPosition, Quaternion newRotation, Vector3 newScale,
        Quaternion[] newBonesRotation, Vector3[] newBonesPosition, Vector3[] newBonesScale)
    {
        h.root.localPosition = newPosition;
        h.root.localRotation = newRotation;
        h.root.localScale = newScale;
        for (int i = 0; i < h.fingerBones.Length; i++)
        {
            h.fingerBones[i].localRotation = newBonesRotation[i];
            h.fingerBones[i].localPosition = newBonesPosition[i];
            h.fingerBones[i].localScale = newBonesScale[i];
        }
    }

    public void UnSetPose(BaseInteractionEventArgs arg)
    {
        if (arg.interactorObject is NearFarInteractor )
        {
            
            Transform transform = arg.interactorObject.transform.parent;
            HandData handData = transform.GetComponentInChildren<HandData>();

            handData.animator.enabled = true;

            SetHandData(handData, startingHandPosition, startingHandRotation, startingHandScale, startingFingerRotations, startingFingerPositions, startingFingerScale);
        }
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        //Debug.Log("istrigger111");

        if (isGrabbing)
        {
            audioSource.clip = clipSpray;
            audioSource.loop = true;
            audioSource.Play();
            isTriggering = true;
            //Debug.Log("istrigger" + isTriggering);
            animatorRight.SetBool("Spray", true);
            animatorLeft.SetBool("Spray", true);
            animatorCap.SetBool("Spray", true);
            sprayParticle.Play();
            DetectSpray();
        }
    }

    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        sprayParticle.Stop();
        audioSource.Stop();
        audioSource.loop = false;
        isTriggering = false;
            animatorRight.SetBool("Spray", false);
            animatorLeft.SetBool("Spray", false);
            animatorCap.SetBool("Spray", false);
        
    }

    public void DetectSpray()
    {
        
        Vector3 rayOrigin = hole.transform.position;
        Vector3 rayDirection = hole.transform.forward;
        Physics.queriesHitBackfaces = true;
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Draw"); 
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 2,layerMask))
        {
            sprayParticle.Play();
            Physics.queriesHitBackfaces = false;
            foreach (WallManager wallManager in wallManagers)
            {
                if (hit.collider.gameObject == wallManager.drawingObject)
                {
                    float distanceToWall = hit.distance;
                    //Debug.Log("Distance to wall: " + distanceToWall);
                    if (wallManager.enabled && wallManager.drawingStage.Equals(DrawingStage.None) || wallManager.drawingStage.Equals(DrawingStage.Drawing))
                    {
                        //Debug.Log($"Start drawing: {hit.textureCoord}");
                        wallManager.DrawAtPosition(this, hit.textureCoord);
                    }
                }
            }
           
        }
    }

}

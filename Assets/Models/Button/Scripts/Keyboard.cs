using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Keyboard : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject normalButtons;
    public GameObject capsButtons;
    public TMP_Text result;

    public Renderer railRenderer; // ??TargetTexture????Renderer
    public Renderer railRenderer1;
    public Renderer roomRenderer;
    public Renderer shopRenderer;

    public Material blackMaterial; 

    public Animator railAnimator;
    public Animator railAnimator1;
    public Animator roomAnimator;
    public Animator shopAnimator;

    public Renderer railCam;
    public Renderer railCam1;
    public Renderer roomCam;
    public Renderer shopCam;

    public Material turnOffMaterial;

    public Camera railCamera;
    public Camera railCamera1;
    public Camera roomCamera;
    public Camera shopCamera;
    public UnityEvent onEnterCorrectPassword;

    public AudioClip success;
    public AudioClip wrong;
    public AudioSource audioSource;

    private bool caps;
    // Start is called before the first frame update
    void Start()
    {
        caps = false;
        result.gameObject.SetActive(false);
    }

    public void InsertChar(string c)
    {
        inputField.text += c;
    }    

    public void DeleteChar()
    {
        if(inputField.text.Length>0)
        {
            inputField.text=inputField.text.Substring(0,inputField.text.Length-1);
        }

    }

    public void EnterChar()
    {
        result.gameObject.SetActive(true);
        if (inputField.text=="")
        {
            result.text = "Please enter the password.";
            result.color = Color.white;

        }
        if(inputField.text== "ARt1f4ct5")
        {
            audioSource.Stop();
            audioSource.clip = success;
            audioSource.Play();
            result.text = "Success";
            result.color = Color.green;
            inputField.text = "";

            railRenderer.material = blackMaterial; // ?????????
            railRenderer1.material = blackMaterial;
            roomRenderer.material = blackMaterial;
            shopRenderer.material = blackMaterial;

            railCam.material = turnOffMaterial;
            railCam1.material = turnOffMaterial;
            shopCam.material = turnOffMaterial;
            roomCam.material = turnOffMaterial;

            railAnimator.SetBool("rotate", false);
            railAnimator1.SetBool("rotate", false);
            roomAnimator.SetBool("rotate", false);
            shopAnimator.SetBool("rotate", false);
            
            railCamera.gameObject.SetActive (false);
            railCamera1.gameObject.SetActive(false);
            roomCamera.gameObject.SetActive(false);
            shopCamera.gameObject.SetActive(false);
            onEnterCorrectPassword?.Invoke();

		}
        if (inputField.text != "ARt1f4ct5" && inputField.text!="")
        {
            audioSource.Stop();
            audioSource.clip = wrong;
            audioSource.Play();
            result.text = "Wrong password!";
            result.color = Color.red;
            inputField.text = "";
        }

        result.gameObject.SetActive(true);
    }

    public void InsertSpace()
    {
        inputField.text += " ";
    }
    
    public void CapsPressed()
    {
        if(!caps)
        {
            normalButtons.SetActive(false);
            capsButtons.SetActive(true);
            caps = true;

        }
        else
        {
            capsButtons.SetActive(false);
            normalButtons.SetActive(true);
            caps = false;
        }
    }
}

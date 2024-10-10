using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SecurityCamera : MonoBehaviour
{
    public Camera securityCamera;
    public Image progressBar; // ??????Image??
    public Transform player;
    public Renderer targetRenderer; // ??TargetTexture????Renderer
    public Material blackMaterial; // ????
    private float timeInView = 0f;
    public Animator animator;
    private float alertTime = 6f;//????????
    private bool alertTriggered = false;
    public Color startColor = Color.green;
    public Color endColor = Color.red;
    private void Start()
    {
        //animator = GetComponent<Animator>();
        progressBar.fillAmount = 0f; // ???????0
        progressBar.gameObject.SetActive(false); // ????????
        animator.SetBool("rotate", true);
    }

    void Update()
    {
        if(securityCamera.isActiveAndEnabled)
        {
            Vector3 screenPoint = securityCamera.WorldToViewportPoint(player.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            if (onScreen)
            {
                //Debug.Log("onscreen");
                Vector3 directionToPlayer = player.position - securityCamera.transform.position;
                Ray ray = new Ray(securityCamera.transform.position, directionToPlayer);
                RaycastHit hit;
                //LayerMask layerMask = LayerMask.GetMask("CameraDetect");
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == player)
                    {
                        //Debug.Log("hit player");
                        // ????????????
                        timeInView += Time.deltaTime;
                        float remainingTime = alertTime - timeInView;
                        progressBar.gameObject.SetActive(true); // ?????
                        progressBar.fillAmount = remainingTime / alertTime; // ?????
                        float t = timeInView / alertTime;
                        progressBar.color = Color.Lerp(startColor, endColor, t);
                        if (timeInView >= alertTime && !alertTriggered)//player's caught
                        {
                            animator.SetBool("rotate", false);
                            //Debug.Log("caught");
                            //targetRenderer.material = blackMaterial; // ?????????
                            alertTriggered = true; // ?????????
                            SceneManager.LoadScene("prison");
                            progressBar.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        // ?????
                        DecreaseProgress();
                    }
                }
            }
            else
            {
                // ??????????
                DecreaseProgress();
            }
        }
    }

    private void DecreaseProgress()
    {
        if (timeInView > 0f)
        {
            timeInView -= Time.deltaTime; // ?? timeInView ??
            //progressBar.fillAmount = timeInView / alertTime; // ?????
            float remainingTime = alertTime - timeInView;
            progressBar.gameObject.SetActive(true); // ?????
            progressBar.fillAmount = remainingTime / alertTime; // ?????
            float t = timeInView / alertTime;
            progressBar.color = Color.Lerp(startColor, endColor, t);
            if (timeInView <= 0f)
            {
                progressBar.gameObject.SetActive(false); // ?????
                timeInView = 0f; // ?? timeInView ???? 0
            }
        }
        alertTriggered = false; // ??????
    }
}


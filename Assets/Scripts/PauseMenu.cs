using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class PauseMenu : MonoBehaviour
{
    public GameObject wristUI; // Reference to the UI that should be displayed/hidden
    public SnapTurnProvider snapTurnProvider;
    public XRRayInteractor left;
    public XRRayInteractor right;
    public TeleportationProvider teleportationProvider;

    private bool isWristUIActive = false; // Keeps track of whether the UI is active

    void Start()
    {
        wristUI.SetActive(false);
        // Ensure the UI starts in the correct state
        //UpdateWristUI(isWristUIActive);
    }

    // Method called when the pause button is pressed
    public void PauseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleWristUI();
        }
    }

    // Toggles the display of the wrist UI and pauses/unpauses the game
    private void ToggleWristUI()
    {
        isWristUIActive = !isWristUIActive;
        UpdateWristUI(isWristUIActive);
    }

    // Updates the wrist UI's visibility, the game's time scale, and the teleportation provider's state
    private void UpdateWristUI(bool isActive)
    {
        wristUI.SetActive(isActive);
        Time.timeScale = isActive ? 0 : 1;

        if (isActive)
        {
            left.interactionLayers = InteractionLayerMask.GetMask("Pause");
            right.interactionLayers = InteractionLayerMask.GetMask("Pause");
            snapTurnProvider.enabled = false;
            teleportationProvider.enabled = false;
        }
        else
        {
            left.interactionLayers = InteractionLayerMask.GetMask("Teleport");
            right.interactionLayers = InteractionLayerMask.GetMask("Teleport");
            snapTurnProvider.enabled = true;
            teleportationProvider.enabled = true;
        }
    }

    public void MusicButton()
    {
        SoundManager.Instance.OnButtonPress();
    }
    // Load the start scene instead of restarting the current scene
    public void RestartGame()
    {
        Time.timeScale = 1; // Ensure time scale is reset before restarting
        ScoreManager.Instance.ResetScore(); // Reset the score when the game restarts
        SceneManager.LoadScene("1 start");
    }


    // Exits the application
    public void ExitGame()
    {
        Application.Quit();
    }
}

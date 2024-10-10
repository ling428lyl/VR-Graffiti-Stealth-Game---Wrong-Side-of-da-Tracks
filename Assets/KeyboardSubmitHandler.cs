using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class KeyboardSubmitHandler : MonoBehaviour
{
    public XRKeyboardDisplay xrKeyboardDisplay;
    public NameImageCreator nameImageCreator;

    void Start()
    {
        if (xrKeyboardDisplay != null)
        {
            // Subscribe to the keyboard's text submitted event
            xrKeyboardDisplay.onTextSubmitted.AddListener(OnKeyboardTextSubmitted);
        }
    }

    private void OnKeyboardTextSubmitted(string submittedText)
    {
        // Ensure that the input field has the correct value
        if (nameImageCreator != null && nameImageCreator.nameInputField.text.Length == 4)
        {
            nameImageCreator.CreateNameImages();
        }
    }
}

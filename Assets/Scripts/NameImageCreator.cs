using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

public class NameImageCreator : MonoBehaviour
{
	private const string LogTag = "NAME_IMAGE_CREATOR"; // Custom tag for filtering in Android Logcat

	public string leftImageFolder = "Alphanumerics";
	public int leftImageLayersCount = 3;
	public string rightImageFolder = "Alphanumerics2";
	public int rightImageLayersCount = 1;
	public MenuManager menuManager;
	public GameObject loadingPanel;
	public GameObject loginPanel;
	public TMP_InputField nameInputField;  // Reference to the TMP_InputField
	public Button createButton;            // Reference to the Button (not needed for enter key now)
	public XRKeyboard xrKeyboard;          // Reference to the XRKeyboard component
	public string savePath = "Assets/Resources/SavedTextures";
	public int maxProcessedPixelsPerFrame = 256;
	public Shader blendingShader;

	private struct SpritesLayer
	{
		public Texture2D[] letters;
	}

	private SpritesLayer[] leftImageSpritesLayers;
	private SpritesLayer[] rightImageSpritesLayers;
	private Material blendingMaterial;

	private void Start()
	{
		//Debug.Log($"{LogTag}: Starting NameImageCreator...");

		// Set the character limit to 4
		nameInputField.characterLimit = 4;

		// Add validation for letters only
		nameInputField.onValidateInput += delegate (string input, int charIndex, char addedChar)
		{
			return ValidateChar(addedChar);
		};

		// Disable the create button initially
		createButton.interactable = false;

		// Add listener to check input field length
		nameInputField.onValueChanged.AddListener(delegate { ValidateInputLength(); });

		// Add listener to show XR keyboard when input field is selected
		nameInputField.onSelect.AddListener(ShowXRKeyboard);

		// Hide XR keyboard initially
		xrKeyboard.gameObject.SetActive(false);

		// Add listener for Enter key press
		xrKeyboard.onTextSubmitted.AddListener(OnKeyboardTextSubmitted);

		LoadLeftImageSprites();
		LoadRightImageSprites();
		blendingMaterial = new Material(blendingShader);
	}

	private void ShowXRKeyboard(string text)
	{
		//Debug.Log($"{LogTag}: XR Keyboard shown.");
		xrKeyboard.Open(nameInputField, true); // Opens XRKeyboard and links it with the TMP_InputField
	}

	private void OnKeyboardTextSubmitted(KeyboardTextEventArgs args)
	{
		string submittedText = args.keyboardText;
		//Debug.Log($"{LogTag}: Text submitted: {submittedText}");

		if (submittedText.Length == 4)
		{
			CreateNameImages();
		}
		else
		{
			//Debug.LogWarning($"{LogTag}: Name must be exactly 4 characters long!");
		}
	}

	private void LoadLeftImageSprites()
	{
		//Debug.Log($"{LogTag}: Loading left image sprites...");
		leftImageSpritesLayers = LoadSprites(leftImageFolder, leftImageLayersCount);
	}

	private void LoadRightImageSprites()
	{
		//Debug.Log($"{LogTag}: Loading right image sprites...");
		rightImageSpritesLayers = LoadSprites(rightImageFolder, rightImageLayersCount);
	}

	private SpritesLayer[] LoadSprites(string folderName,int numberOfLayers)
	{
		//Debug.Log($"{LogTag}: Loading sprites from folder: {folderName}");

		var spritesLayers = new SpritesLayer[numberOfLayers];

		for (int i = 0; i < numberOfLayers; i++)
		{
			string lettersPath = $"{folderName}/Letters Layer {i + 1}";

			Texture2D[] letters = Resources.LoadAll<Texture2D>(lettersPath);

			SpritesLayer layer = new SpritesLayer
			{
				letters = letters
			};

			spritesLayers[i] = layer;
		}

		// For debugging: print the loaded sprites
		for (int i = 0; i < spritesLayers.Length; i++)
		{
			//Debug.Log($"{LogTag}: Layer {i + 1}:");
			//Debug.Log($"{LogTag}: Letters: {string.Join(", ", spritesLayers[i].letters.Select(s => s.name))}");
		}
		return spritesLayers;
	}

	private void ValidateInputLength()
	{
		//Debug.Log($"{LogTag}: Input length: {nameInputField.text.Length}");
		// Enable the create button only if the input length is exactly 4
		createButton.interactable = nameInputField.text.Length == 4;
	}

	public void CreateNameImages()
	{
		//Debug.Log($"{LogTag}: Creating name images...");
		StartCoroutine(CreateImagesCoroutine());
		loadingPanel.SetActive(true);
		loginPanel.SetActive(false);
	}

	public IEnumerator CreateImagesCoroutine()
	{
		string playerName = nameInputField.text.ToUpper();  // Get and uppercase the input name
		//Debug.Log($"{LogTag}: CreateNameImage called with name: " + playerName);
		Texture2D[] leftImageLayersTexture = null;
		Texture2D[] rightImageLayersTexture = null;
		//Debug.Log($"{LogTag}:timestamp 1");
		yield return Run<Texture2D[]>(CreateNameLayer(playerName, leftImageFolder, leftImageSpritesLayers), (output) => leftImageLayersTexture = output);
		//Debug.Log($"{LogTag}:timestamp 2");
		yield return Run<Texture2D[]>(CreateNameLayer(playerName, rightImageFolder, rightImageSpritesLayers), (output) => rightImageLayersTexture = output);
		//Debug.Log($"{LogTag}:timestamp 3");

		Texture2D leftImageAllLayersTexture = null;
		Texture2D rightImageAllLayersTexture = null;
		yield return Run<Texture2D>(BlendTextures(leftImageLayersTexture), (output) => leftImageAllLayersTexture = output);
		//Debug.Log($"{LogTag}:timestamp 4");
		//SaveTexture(playerName, leftImageAllLayersTexture, savePath + "/" + leftImageFolder + "/" + playerName);

		yield return Run<Texture2D>(BlendTextures(rightImageLayersTexture), (output) => rightImageAllLayersTexture = output);
		//Debug.Log($"{LogTag}:timestamp 5");
		//SaveTexture(playerName, rightImageAllLayersTexture, savePath + "/" + rightImageFolder + "/" + playerName);

		//Debug.Log($"{LogTag}: Textures saved and menu setup started.");
		// Setup Menu Manager
		menuManager.SetupMenu(leftImageLayersTexture, leftImageAllLayersTexture, rightImageLayersTexture, rightImageAllLayersTexture);

		//Debug.Log($"{LogTag}:timestamp 6");
		gameObject.SetActive(false);
		//Debug.Log($"{LogTag}: NameImageCreator process complete.");
	}

	private IEnumerator CreateNameLayer(string playerName, string folderName, SpritesLayer[] spritesLayers)
	{
		//Debug.Log($"{LogTag}: Creating name layer for {playerName} in folder {folderName}");
		Texture2D[] layersTexture = new Texture2D[spritesLayers.Length];

		Texture2D[] usedLetterTextures = new Texture2D[playerName.Length];
		// Calculate total width of the combined image
		float totalWidth = 0f;
		float scaleFactor = 0.5f;  // Adjust this value to scale the sprites

		foreach (char letter in playerName)
		{
			Texture2D letterSprite = GetTextureByName(spritesLayers[0], letter.ToString());

			if (letterSprite != null)
			{
				totalWidth += letterSprite.width * scaleFactor;
				//Debug.Log($"{LogTag}: Letter {letter} width: {letterSprite.width}");
			}
		}

		// Loop through each layer
		for (int k = 0; k < spritesLayers.Length; k++)
		{
			// Calculate the starting position
			float startX = -totalWidth / 2;

			// Load and instantiate images
			float currentX = startX;
			for (int i = 0; i < playerName.Length; i++)
			{
				char letter = playerName[i];
				Texture2D letterSprite = GetTextureByName(spritesLayers[k], letter.ToString());

				usedLetterTextures[i] = letterSprite;
				if (letterSprite != null)
				{
					// Update the current X position
					currentX += letterSprite.width * scaleFactor;
					//Debug.Log($"{LogTag}: Layer {k + 1}, Letter {letter}: Position X = {currentX}");
				}
				else
				{
					Debug.LogWarning($"{LogTag}: No sprite found for character: " + letter);
				}
			}

			// Render and store the combined image
			yield return Run<Texture2D>(CombineTextures(usedLetterTextures), (output) => layersTexture[k] = output);

			// Save combined texture
			//SaveTexture(folderName, playerName, k + 1, layersTexture[k]);

			//Debug.Log($"{LogTag}: Layer {k + 1} texture saved.");

		}
		yield return layersTexture;
	}

	private void SaveTexture(string folderName, string playerName, int layer, Texture2D combinedTexture)
	{
		string newPath = savePath + "/" + playerName + "/" + folderName + "/Layer" + layer;
		string fileName = playerName + " Layer " + layer;

		//Debug.Log($"{LogTag}: Saving texture: {fileName} to {newPath}");

		SaveTexture(fileName, combinedTexture, newPath);
	}

	private void SaveTexture(string fileName, Texture2D combinedTexture, string destinationPath)
	{
		// Ensure the save path exists
		if (!Directory.Exists(destinationPath))
		{
			Directory.CreateDirectory(destinationPath);
			//Debug.Log($"{LogTag}: Created directory: {destinationPath}");
		}

		// Encode texture to PNG format
		byte[] bytes = combinedTexture.EncodeToPNG();

		// Create the file name with the user's name
		string filePath = Path.Combine(destinationPath, fileName + ".png");

		// Write the file to disk
		File.WriteAllBytes(filePath, bytes);

		//Debug.Log($"{LogTag}: Saved texture to: " + filePath);
	}

	private IEnumerator CombineTextures(Texture2D[] textures)
	{
		int width = 0;
		int height = 0;
		foreach (Texture2D texture in textures)
		{
			width += texture.width;
			height = Mathf.Max(height, texture.height);
		}

		Texture2D combinedTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		int currentX = 0;
		foreach (Texture2D texture in textures)
		{
			RenderTexture renderTex = new RenderTexture(texture.width, texture.height, 24);
			renderTex.Create();
			Graphics.Blit(texture, renderTex);
			var oldRt = RenderTexture.active;
			RenderTexture.active = renderTex;
			combinedTexture.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), currentX, 0);
			RenderTexture.active = oldRt;
			currentX += texture.width;
			yield return null;
		}

		combinedTexture.Apply();
		yield return combinedTexture;
	}

	private IEnumerator BlendTextures(Texture2D[] textures)
	{
		//Debug.Log($"{LogTag}: Blending textures...");
		int width = textures[0].width;
		int height = textures[0].height;

		//Texture2D finalTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);

		RenderTexture renderTex = new RenderTexture(width, height, 24);
		renderTex.Create();

		foreach (Texture2D texture in textures)
		{
			blendingMaterial.SetTexture("_MainTex", renderTex);
			// Create a temporary render texture
			RenderTexture temp = RenderTexture.GetTemporary(renderTex.width, renderTex.height, 0, renderTex.format);
			blendingMaterial.SetTexture("_BlendTex", texture);

			// First Blit: Apply the blend material to the temporary render texture
			Graphics.Blit(null, temp, blendingMaterial);

			// Second Blit: Copy the result from the temporary render texture to the destination
			Graphics.Blit(temp, renderTex);
			// Release the temporary render texture
			RenderTexture.ReleaseTemporary(temp);
			yield return null;
		}
		var oldRt = RenderTexture.active;
		RenderTexture.active = renderTex;
		Texture2D finalTexture = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, false);
		finalTexture.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
		finalTexture.Apply();
		RenderTexture.active = oldRt;

		yield return finalTexture;
	}

	private char ValidateChar(char addedChar)
	{
		// Allow only letters
		if (char.IsLetter(addedChar))
		{
			return addedChar;
		}
		//Debug.LogWarning($"{LogTag}: Invalid character input: {addedChar}");
		return '\0';  // Invalid character
	}

	private Texture2D GetTextureByName(SpritesLayer spritesLayer, string name)
	{
		//Debug.Log($"{LogTag}: Getting texture by name: {name}");
		foreach (Texture2D texture2D in spritesLayer.letters)
		{
			if (texture2D.name == name)
			{
				return texture2D;
			}
		}
		//Debug.LogError($"{LogTag}: Texture with name {name} not found in layer!");
		throw new KeyNotFoundException();
	}

	public static IEnumerator Run<T>(IEnumerator target, Action<T> output)
	{
		object result = null;
		while (target.MoveNext())
		{
			result = target.Current;
			yield return result;
		}
		output((T)result);
	}
}
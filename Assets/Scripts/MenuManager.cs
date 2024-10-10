using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

	[SerializeField] private Image leftImage;
	[SerializeField] private Image rightImage;
	[SerializeField] private WallManager[] wallManagers;
	public static Texture2D blendTextureLeft = null;
	public static Texture2D blendTextureRight = null;

	private void Start()
	{
		if(blendTextureLeft != null)
		{
			leftImage.sprite = Sprite.Create(blendTextureLeft, new Rect(0, 0, blendTextureLeft.width, blendTextureLeft.height), new Vector2(0.5f, 0.5f));
		}
		if (blendTextureRight != null)
		{
			rightImage.sprite = Sprite.Create(blendTextureRight, new Rect(0, 0, blendTextureRight.width, blendTextureRight.height), new Vector2(0.5f, 0.5f));
		}
	}

	public void SetupMenu(Texture2D[] generatedLayersLeft, Texture2D blendTextureLeft, Texture2D[] generatedLayersRight, Texture2D blendTextureRight)
	{
		leftImage.sprite = Sprite.Create(blendTextureLeft, new Rect(0, 0, blendTextureLeft.width, blendTextureLeft.height), new Vector2(0.5f, 0.5f));
		rightImage.sprite = Sprite.Create(blendTextureRight, new Rect(0, 0, blendTextureRight.width, blendTextureRight.height), new Vector2(0.5f, 0.5f));
		MenuManager.blendTextureLeft = blendTextureLeft;
		MenuManager.blendTextureRight = blendTextureRight;
		SetWallManager(generatedLayersLeft, generatedLayersRight);

	}
	public void SetWallManager(Texture2D[] leftDrawing, Texture2D[] rightDrawing)
	{
		foreach (WallManager wallManager in wallManagers)
		{
			wallManager.SetWallManager(leftDrawing, rightDrawing);
			wallManager.enabled = true;
		}
		
	}
}

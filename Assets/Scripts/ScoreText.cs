using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
	[SerializeField] private TMP_Text scoreText;
	private Transform playerHead;
	private void OnEnable()
	{
		UpdateScore(ScoreManager.Instance.GetCurrentScore());
		ScoreManager.Instance.onScoreUpdated.AddListener(UpdateScore);
		playerHead = Camera.main.transform;
	}

	private void Update()
	{
		transform.rotation = Quaternion.LookRotation(transform.position - new Vector3(
			playerHead.position.x,
			playerHead.position.y,
			playerHead.position.z
			));	
	}

	private void UpdateScore(int newScore)
	{
		if (scoreText != null)
		{
			scoreText.text = newScore.ToString() + " PTS";
		}
	}


	private void OnDisable()
	{
		ScoreManager.Instance.onScoreUpdated.RemoveListener(UpdateScore);
	}
}

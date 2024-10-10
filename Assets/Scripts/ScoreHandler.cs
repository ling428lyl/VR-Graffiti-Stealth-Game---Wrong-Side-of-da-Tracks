using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreHandler : MonoBehaviour
{
	[SerializeField] private float targetScore;
	[SerializeField] private UnityEvent onTargetScoreReached;

	private void Start()
	{
		ScoreManager.Instance.updateScore = true;
		ScoreManager.Instance.onScoreUpdated.AddListener(OnScoreUpdated);
	}

	private void OnScoreUpdated(int currentScore)
	{
		if( currentScore >= targetScore)
		{
			onTargetScoreReached?.Invoke();
		}
	}
}

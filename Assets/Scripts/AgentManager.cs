using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AgentManager : MonoBehaviour
{
    public bool inDisguiseMode;
    [SerializeField] private ObjectSyncronizer objectSyncronizer;
    [SerializeField] private Slider slider;
    [SerializeField] private Animator animator;
    [SerializeField] private string saluteAnimation;
    [SerializeField] private string gameOverScene;

	private void Start()
	{
		SetDisguiseMode(objectSyncronizer.syncHat && SyncronizerManager.instance.hatOn);
	}
	public void SetDisguiseMode(bool newState)
	{
		inDisguiseMode = newState;
	}
	internal void DisableSlider()
	{
		slider.gameObject.SetActive(false);
	}

	internal void EndSalute()
	{
		if (animator)
		{
			animator.SetBool(saluteAnimation, false);
			animator.SetFloat("speed", 1);
		}
	}

	internal bool InDisguiseMode()
	{
		return inDisguiseMode;
	}

	internal void SetSlider(float percentage)
	{
        if (!slider.gameObject.activeSelf)
        {
            slider.gameObject.SetActive(true);
        }
		slider.value = percentage;
		if(slider.value == 1)
		{
			GameOver();
		}
	}

	internal void StartSalute()
	{
		animator.SetBool(saluteAnimation, true);
	}

	private void GameOver()
	{
		SceneManager.LoadScene(gameOverScene);
	}
}

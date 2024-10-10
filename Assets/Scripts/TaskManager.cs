using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
	[SerializeField] private GameObject[] nextLevelColliders;
	[SerializeField] private Transform tasksList;
	[SerializeField] private Transform tasksListParent;
	[SerializeField] private Transform lastTaskText;
	[SerializeField] private GameObject taskLine;

	[Serializable]
	public class Task
	{
		public int id;
		public string description;
		public bool completed;
		public UnityEvent onCompleted;
		public Toggle toggle;
		public void MarkTaskAsCompleted()
		{
			completed = true;
			onCompleted?.Invoke();
		}
		
	}
	public List<Task> tasks;

	private void Start()
	{
		foreach (var nextLevelCollider in nextLevelColliders)
		{
			nextLevelCollider.SetActive(false);
		}
		SetupTasks();
	}

	private void SetupTasks()
	{
		if(tasks.Count == 0)
		{
			return;
		}
		for(int i = 0; i < tasks.Count - 1; i++)
		{
			SetupTask(i);
		}
		SetupLastTask();
	}

	private void SetupLastTask()
	{
		Task task = tasks[tasks.Count - 1];
		lastTaskText.GetChild(0).GetComponent<TMP_Text>().text = task.description;
	}

	private void SetupTask(int i)
	{
		Task task = tasks[i];
		var taskLine = Instantiate(this.taskLine, tasksListParent);
		taskLine.transform.GetChild(0).GetComponent<TMP_Text>().text = task.description;
		task.toggle = taskLine.transform.GetChild(1).GetComponent<Toggle>();
	}

	public Task FindTaskByID(int id)
	{
		return tasks.Find(t => t.id == id);
	}
	public void SetTaskAsCompleted(int taskId)
	{
		Task task = FindTaskByID(taskId);
		if (task != null)
		{
			task.MarkTaskAsCompleted();
			if(task.toggle != null)
			{
				task.toggle.isOn = true;
			}
			VerifyTasks();
		}
	}

	private void VerifyTasks()
	{
		if (tasks.Count == 0)
		{
			return;
		}
		for (int i = 0; i < tasks.Count - 1; i++)
		{
			if (!tasks[i].completed)
			{
				return;
			}
		}
	    StartCoroutine(ActivateLastTask());
		
	}
	private IEnumerator ActivateLastTask()
	{
		yield return new WaitForSeconds(3f);
		tasksList.gameObject.SetActive(false);
		lastTaskText.gameObject.SetActive(true);
		EnablePortals();

	}
	private void EnablePortals()
	{
		foreach (var nextLevelCollider in nextLevelColliders)
		{
			nextLevelCollider.SetActive(true);
		}
	}
}

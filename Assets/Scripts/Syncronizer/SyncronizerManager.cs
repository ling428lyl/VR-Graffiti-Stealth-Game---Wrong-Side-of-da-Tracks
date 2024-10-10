using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncronizerManager : MonoBehaviour
{
	public static SyncronizerManager instance;
	public bool hatOn;
	public bool beltOn;
	public bool leftCanOn;
	public bool rightCanOn;
	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}


}

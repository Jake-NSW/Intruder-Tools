using UnityEngine;
using System;

public class ReplaceAll : MonoBehaviour
{
	[ExecuteInEditMode()]
	public bool go = false;

	public GameObject[] toReplace;
	public GameObject pref;

	public void Update()
	{
		if (go)
		{
			for (int i = 0; i < toReplace.Length; i++)
			{

				GameObject p = (GameObject)Instantiate(pref);

				p.transform.parent = toReplace[i].transform.parent;
				p.transform.position = toReplace[i].transform.position;
				p.transform.rotation = toReplace[i].transform.rotation;
				p.transform.localScale = toReplace[i].transform.localScale;
			}

			go = false;
		}
	}
}
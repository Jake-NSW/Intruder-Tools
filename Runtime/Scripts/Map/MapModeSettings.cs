using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapModeSettings : MonoBehaviour
{
	public static MapModeSettings main;
	public int roundTimeLength = 0;

	public void Awake()
	{
		main = this;
	}
}
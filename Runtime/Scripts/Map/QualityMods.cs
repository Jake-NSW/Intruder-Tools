using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityMods : MonoBehaviour
{
	public float shadowDistance = 300;

	void Awake()
	{
		QualitySettings.shadowDistance = shadowDistance;
	}
}

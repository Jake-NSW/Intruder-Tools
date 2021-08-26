using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TuningProxy : MonoBehaviour
{
	[TextAreaAttribute(10, 50)]
	public string tuningParameters;

	public void CheckJSON()
	{
		string escName = UnityWebRequest.EscapeURL(tuningParameters);
		escName = escName.Replace("+", "%20");
		Application.OpenURL("https://jsonlint.com/?json=" + escName);
	}
}

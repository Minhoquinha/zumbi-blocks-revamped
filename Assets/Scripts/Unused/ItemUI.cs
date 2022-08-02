using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour 
{
	public Transform PlayerTransform;
	public Canvas ItemCanvas;
	public Text ItemText;

	void Start ()
	{
		ItemCanvas = GetComponentInChildren<Canvas> ();
		ItemText = GetComponentInChildren<Text> ();

		PlayerTransform = FindObjectOfType<PlayerHUD>().transform;
	}

	void Update ()
	{
		ItemCanvas.transform.LookAt (PlayerTransform);
	}
}

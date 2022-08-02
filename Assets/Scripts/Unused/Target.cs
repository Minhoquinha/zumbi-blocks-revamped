using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Target : MonoBehaviour 
{
	public float Health = 50f;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}

	public void TakeDamage (float Amount)
	{
		Health -= Amount;
		if (Health <= 0f) 
		{
			Death ();
		}
	}

	void Death ()
	{
		Debug.Log (this.name + " was destroyed;");
		Destroy (gameObject);
	}
}

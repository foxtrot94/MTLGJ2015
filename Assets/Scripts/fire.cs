﻿using UnityEngine;
using System.Collections;

public class fire : MonoBehaviour
{
	GameObject score;

    Flammable m_FlammableParent;

	// Use this for initialization
	void Start ()
    {
		score = GameObject.FindGameObjectWithTag("Score");
	}

    public void Initialize(Flammable flammableParent)
    {
        m_FlammableParent = flammableParent;
    }

	void OnTriggerEnter2D(Collider2D collider)
    {
		if (collider.gameObject.tag == "Player")
        {
			collider.gameObject.SendMessage ("Kill", SendMessageOptions.DontRequireReceiver);
		}
		if (collider.gameObject.tag == "Water")
		{
			score.SendMessage("Increment", 1);
			Destroy(gameObject);
	    }
	}

    public void OnDestroy()
    {
        Debug.Log("Fire should extinguish tile");
    }
}

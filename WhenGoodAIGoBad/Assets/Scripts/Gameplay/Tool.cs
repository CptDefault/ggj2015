using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Collider2D))]
public class Tool : MonoBehaviour 
{
    public bool Carried { get; private set; }
    public Action OnPickup;

    public enum ToolType {
        Generic, Battery, Extinguisher
    };

    public ToolType Type;

    private void Start() {
        //Type = ToolType.Generic;
    }

	public void PickUp(Transform par)
	{
	    Carried = true;
	    transform.parent = par;
	    transform.localPosition = Vector2.up*.1f;

		if(OnPickup != null)
        	OnPickup();
	}

    public void Drop(Vector2 pos)
    {
        Carried = false;
        transform.parent = null;
        transform.position = pos;
    }
}

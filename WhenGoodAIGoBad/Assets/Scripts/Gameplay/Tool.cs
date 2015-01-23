using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Tool : MonoBehaviour 
{
    public bool Carried { get; private set; }

	public void PickUp(Transform par)
	{
	    Carried = true;
	    transform.parent = par;
	    transform.localPosition = Vector2.up*.1f;
	}

    public void Drop(Vector2 pos)
    {
        Carried = false;
        transform.parent = null;
        transform.position = pos;
    }
}

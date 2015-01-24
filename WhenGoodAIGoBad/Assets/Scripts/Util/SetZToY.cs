using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetZToY : MonoBehaviour
{
    public bool mobile;
    public Collider2D col;

    protected void Awake()
    {
        Reposition(null);
        if (mobile)
            StartCoroutine(Routine());
    }

    private IEnumerator Routine()
    {
        if (col == null)
            col = GetComponent<Collider2D>();
        while (true)
        {
            var position = transform.position;
            position.z = position.y * 0.001f;
            if (col != null)
                position.z = col.bounds.center.y * 0.001f;
            transform.position = position;
            yield return null;
        }
    }

    private void Reposition(SetZToY caller)
    {
        if(caller == this)
            return;

        var position = transform.position;
        position.z = position.y*0.001f;

        if (col == null)
            col = GetComponent<Collider2D>();

        if (col != null)
            position.z = col.bounds.center.y*0.001f;

        transform.position = position;
        BroadcastMessage("Reposition", this);
    }
}

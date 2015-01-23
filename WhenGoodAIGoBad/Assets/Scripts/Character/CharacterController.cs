using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    public float MoveSpeed = 5;

    private Rigidbody2D _rigid;
    private Vector2 _desiredSpeed;

    protected void Awake()
    {
        _rigid = rigidbody2D;
    }

    public void SetDesiredSpeed(Vector2 speed)
    {
        _desiredSpeed = speed;
    }

    protected void FixedUpdate()
    {
        _rigid.velocity = _desiredSpeed;
    }
}

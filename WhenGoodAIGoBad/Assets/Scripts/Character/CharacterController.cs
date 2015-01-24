using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    public float MoveSpeed = 5;
    public float Acceleration = 5;
    public bool ForceDiagonalMovement;

    private Rigidbody2D _rigid;
    private Vector2 _desiredSpeed;
    public Room Room { get; private set; }

    protected void Awake()
    {
        _rigid = rigidbody2D;
    }

    public void SetDesiredSpeed(Vector2 speed)
    {
        _desiredSpeed = speed * MoveSpeed;


        if (ForceDiagonalMovement)
        {
            var x = Mathf.Abs(_desiredSpeed.x);
            var y = Mathf.Abs(_desiredSpeed.y);
            if (x < y / 2)
                _desiredSpeed.x = 0;
            else if (y < x / 2)
                _desiredSpeed.y = 0;
            else
            {
                _desiredSpeed.x *= (y + x) / (x + 1);
                _desiredSpeed.y *= (y + x) / (y + 1);
            }
        }
    }

    protected void FixedUpdate()
    {
        var speed = _rigid.velocity;

        speed += Vector2.ClampMagnitude(_desiredSpeed - speed, Acceleration*Time.fixedDeltaTime);

        _rigid.velocity = speed;
    }

    protected void OnTriggerEnter2D(Collider2D col)
    {
        var room = col.GetComponent<Room>();
        if (room != null)
        {
            Room = room;
        }
    }
}

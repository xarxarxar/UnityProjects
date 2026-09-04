using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovingType
{
    None,
    Left,
    Right
}

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteMovement : MonoBehaviour
{
    // public MovementSpeedType movementSpeedType;

    // public float speed = 1f;

    // public MovingType moving;

    // private Vector3 startPos;

    public float parallaxFactor = 1f;

    private void Awake()
    {
        // startPos = transform.position;
    }

    // public void ReStart()
    // {
    //     transform.position = startPos;
    // }

    // public void AllowMove(MovingType moving)
    // {
    //     this.moving = moving;
    // }

    // void Update()
    // {
    //     if (this.moving == MovingType.None) return;

    //     var newPosition = transform.position;
    //     newPosition.x += (moving == MovingType.Left ? -1 : 1) * speed* Time.deltaTime;
    //     transform.position = newPosition;
    // }

    public void Move(float delta)
    {
        Vector3 newPos = transform.position;
        newPos.x -= delta * parallaxFactor;
        transform.localPosition = newPos;
    }
}

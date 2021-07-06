using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Main;
using UnityEngine;
using UnityEngine.InputSystem;
using URPTemplate.UI;

public class MoveCamera : MonoBehaviour, GameInput.IMoveActions
{
    private Vector2 direction;
    public float speed = 10f;

    void OnEnable()
    {
        GameInputHandler.SetCallback(this);
    }

    void OnDisable()
    {
        GameInputHandler.RemoveCallback(this);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    void Update()
    {
        transform.position += new Vector3(-direction.y, 0, direction.x) * Time.deltaTime * speed;
    }
}

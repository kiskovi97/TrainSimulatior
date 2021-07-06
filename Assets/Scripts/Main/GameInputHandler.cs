using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Main
{
    public class GameInputHandler : MonoBehaviour, @GameInput.IMoveActions
    {
        private static GameInputHandler Instance;
        private GameInput inputActions;
        private static List<@GameInput.IMoveActions> subscribers = new List<@GameInput.IMoveActions>();


        public static void SetCallback(@GameInput.IMoveActions characterActions)
        {
            subscribers.Add(characterActions);
            Debug.Log("SetCallback");
        }

        public static void RemoveCallback(@GameInput.IMoveActions characterActions)
        {
            subscribers.Remove(characterActions);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                inputActions = new GameInput();
                inputActions.Enable();
                inputActions.Move.SetCallbacks(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed) return;
            foreach (var subscriber in subscribers)
            {
                subscriber.OnMove(context);
            }
        }
    }
}

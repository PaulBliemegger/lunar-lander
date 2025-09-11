using System;
using UnityEngine;

namespace ProjectSelene.Code.Gameplay
{
    public class Rotate : MonoBehaviour
    {
        private DefaultInputActions _inputActions;
        // Start is called before the first frame update
        void Start()
        {
            _inputActions = new DefaultInputActions();
            _inputActions.Player.Look.performed += (t => RotateObject(t.ReadValue<Vector2>()));
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        void RotateObject(Vector2 direction)
        {
            transform.Rotate(direction);
        }
    }
}

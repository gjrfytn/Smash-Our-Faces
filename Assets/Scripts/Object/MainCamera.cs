using UnityEngine;
using UnityEngine.UIElements;

namespace Sof.Object
{
    public class MainCamera : Auxiliary.SofSceneMonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private float _MoveSpeedByKey;
        [SerializeField]
        private MouseButton _MoveMouseButton;
#pragma warning restore 0649

        private Camera _Camera;
        private Vector3 _PrevMousePos;

        private void Start()
        {
            _Camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown((int)_MoveMouseButton))
                _PrevMousePos = Input.mousePosition;

            if (Input.GetMouseButton((int)_MoveMouseButton))
            {
                var mousePos = Input.mousePosition;

                var mouseDelta = _Camera.ScreenToWorldPoint(mousePos) - _Camera.ScreenToWorldPoint(_PrevMousePos);
                _PrevMousePos = mousePos;

                transform.position = new Vector3(transform.position.x - mouseDelta.x, transform.position.y - mouseDelta.y, transform.position.z);

                return;
            }

            var delta = _MoveSpeedByKey * Time.deltaTime;

            if (Input.GetKey(KeyCode.A))
                transform.position = new Vector3(transform.position.x - delta, transform.position.y, transform.position.z);

            if (Input.GetKey(KeyCode.W))
                transform.position = new Vector3(transform.position.x, transform.position.y + delta, transform.position.z);

            if (Input.GetKey(KeyCode.D))
                transform.position = new Vector3(transform.position.x + delta, transform.position.y, transform.position.z);

            if (Input.GetKey(KeyCode.S))
                transform.position = new Vector3(transform.position.x, transform.position.y - delta, transform.position.z);
        }
    }

}
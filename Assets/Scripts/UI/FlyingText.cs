using Gjrfytn.Dim.Object;
using UnityEngine;

namespace Sof.UI
{
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public abstract class FlyingText : SofMonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private float _Lifetime;

        [SerializeField]
        private float _AscensionSpeed;
#pragma warning restore 0649

        protected string Text
        {
            set => GetComponent<UnityEngine.UI.Text>().text = value;
        }

        private float _LifetimeLeft;

        private void Start()
        {
            _LifetimeLeft = _Lifetime;
        }

        private void Update()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + _AscensionSpeed, transform.localPosition.z);

            _LifetimeLeft -= Time.deltaTime;

            if (_LifetimeLeft <= 0)
                Destroy(gameObject);
        }
    }
}

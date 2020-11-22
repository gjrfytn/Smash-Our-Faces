using UnityEngine;

namespace Sof.UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FlyingIcon : Auxiliary.SofMonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private float _Lifetime;

        [SerializeField]
        private float _AscensionSpeed;
#pragma warning restore 0649

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

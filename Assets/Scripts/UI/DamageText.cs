using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Text))]
public class DamageText : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private float _Lifetime;

    [SerializeField]
    private float _AscensionSpeed;
#pragma warning restore 0649

    public int Damage
    {
        set => GetComponent<UnityEngine.UI.Text>().text = value.ToString();
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

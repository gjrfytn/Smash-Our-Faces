using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Ground : Auxiliary.SofSceneMonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField]
        private Sprite[] _Sprites;
#pragma warning restore 0649

        private void Start()
        {
            if (!_Sprites.Any())
                throw new System.Exception("Sprite array is empty.");

            GetComponent<SpriteRenderer>().sprite = _Sprites[Random.Range(0, _Sprites.Length)];
        }
    }
}

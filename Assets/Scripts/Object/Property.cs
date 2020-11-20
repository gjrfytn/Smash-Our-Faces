using UnityEngine;

namespace Sof.Object
{
    public abstract class Property : MapObject
    {
#pragma warning disable 0649
        [SerializeField]
        private SpriteRenderer _OwnerFactionSprite;
#pragma warning restore 0649

        protected abstract GameManager GameManager { get; }
        protected abstract UIManager UIManager { get; }
        protected abstract Model.MapObject.Property.Property ModelProperty { get; }

        private void Start()
        {
            ModelProperty.OwnerChanged += Property_OwnerChanged;

            SetOwnerColor();
        }

        private void Property_OwnerChanged()
        {
            SetOwnerColor();
            ShowCapture();
        }

        private void SetOwnerColor()
        {
            _OwnerFactionSprite.color = GameManager.GetFactionColor(ModelProperty.Owner);
        }

        private void ShowCapture()
        {
            UIManager.OnPropertyCapture(this);
        }
    }
}

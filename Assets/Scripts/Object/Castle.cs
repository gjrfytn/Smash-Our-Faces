using System.Linq;
using UnityEngine;

namespace Sof.Object
{
    public class Castle : MapObject
    {
        private GameManager _GameManager;
        private SpriteRenderer _OwnerFactionSprite;

        public Model.MapObject.Castle ModelCastle { get; private set; }

        private void Awake()
        {
            _OwnerFactionSprite = transform.GetComponentsInChildren<SpriteRenderer>().Single(r => r.name == "castle_roof_bw");
        }

        public void Initialize(Model.MapObject.Castle castle, GameManager gameManager)
        {
            ModelCastle = castle ?? throw new System.ArgumentNullException(nameof(castle));
            _GameManager = gameManager != null ? gameManager : throw new System.ArgumentNullException(nameof(gameManager));

            castle.OwnerChanged += Castle_OwnerChanged;

            SetOwnerColor();
        }

        public override bool OnHover() => false;
        public override bool OnLeftClick() => false;

        public override bool OnRightClick()
        {
            _GameManager.OnCastleRightClick(this);

            return true;
        }

        private void Castle_OwnerChanged()
        {
            SetOwnerColor();
        }

        private void SetOwnerColor()
        {
            _OwnerFactionSprite.color = _GameManager.GetFactionColor(ModelCastle.Owner);
        }
    }
}

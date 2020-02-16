namespace Sof.Object
{
    public class Castle : Property
    {
        private GameManager _GameManager;

        public Model.MapObject.Castle ModelCastle { get; private set; }

        protected override GameManager GameManager => _GameManager;
        protected override Model.MapObject.Property ModelProperty => ModelCastle;

        public void Initialize(Model.MapObject.Castle castle, GameManager gameManager)
        {
            ModelCastle = castle ?? throw new System.ArgumentNullException(nameof(castle));
            _GameManager = gameManager != null ? gameManager : throw new System.ArgumentNullException(nameof(gameManager));
        }

        public override bool OnHover() => false;
        public override bool OnLeftClick() => false;

        public override bool OnRightClick()
        {
            _GameManager.OnCastleRightClick(this);

            return true;
        }
    }
}

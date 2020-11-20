namespace Sof.Object
{
    public class Castle : Property
    {
        private GameManager _GameManager;
        private UIManager _UIManager;

        public Model.MapObject.Property.Castle ModelCastle { get; private set; }

        protected override GameManager GameManager => _GameManager;
        protected override UIManager UIManager => _UIManager;
        protected override Model.MapObject.Property.Property ModelProperty => ModelCastle;

        public void Initialize(Model.MapObject.Property.Castle castle, GameManager gameManager, UIManager uiManager)
        {
            ModelCastle = castle ?? throw new System.ArgumentNullException(nameof(castle));
            _GameManager = gameManager != null ? gameManager : throw new System.ArgumentNullException(nameof(gameManager));
            _UIManager = uiManager != null ? uiManager : throw new System.ArgumentNullException(nameof(uiManager));
        }

        public override bool OnHover() => false;
        public override bool OnLeftClick() => false;

        public override bool OnRightClick()
        {
            _UIManager.OnCastleRightClick(this);

            return true;
        }
    }
}

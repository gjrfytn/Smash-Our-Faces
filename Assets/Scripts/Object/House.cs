namespace Sof.Object
{
    public class House : Property
    {
        private GameManager _GameManager;

        public Model.MapObject.Property.House ModelHouse { get; private set; }

        protected override GameManager GameManager => _GameManager;
        protected override Model.MapObject.Property.Property ModelProperty => ModelHouse;

        public void Initialize(Model.MapObject.Property.House house, GameManager gameManager)
        {
            ModelHouse = house ?? throw new System.ArgumentNullException(nameof(house));
            _GameManager = gameManager != null ? gameManager : throw new System.ArgumentNullException(nameof(gameManager));
        }

        public override bool OnHover() => false;
        public override bool OnLeftClick() => false;
        public override bool OnRightClick() => false;
    }
}

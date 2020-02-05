namespace Sof.Object
{
    public class Castle : MapObject
    {
        private Model.MapObject.Castle _Castle;

        public void Initialize(Model.MapObject.Castle castle)
        {
            _Castle = castle ?? throw new System.ArgumentNullException(nameof(castle));
        }
    }
}

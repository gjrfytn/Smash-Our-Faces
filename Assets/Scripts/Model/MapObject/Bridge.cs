namespace Sof.Model.MapObject
{
    public class Bridge : MapObject
    {
        public override int MoveCostModificator => -3;
        public override float DefenceModificator => -0.25f;
    }
}

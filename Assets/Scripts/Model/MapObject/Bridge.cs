namespace Sof.Model.MapObject
{
    public class Bridge : MapObject
    {
        public override int MoveCostModificator => -2;
        public override float DefenceModificator => -0.25f;
    }
}

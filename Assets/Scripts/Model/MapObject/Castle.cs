namespace Sof.Model.MapObject
{
    public class Castle : MapObject
    {
        public override int MoveCostModificator => -2;
        public override float DefenceModificator => 0.75f;
    }
}

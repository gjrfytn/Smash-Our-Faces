namespace Sof.Model.MapObject
{
    public class Road : MapObject
    {
        public override int MoveCostModificator => -2;
        public override float DefenceModificator => 0;
    }
}

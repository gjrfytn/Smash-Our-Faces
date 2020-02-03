namespace Sof.Model.MapObject
{
    public class House : MapObject
    {
        public override int MoveCostModificator => -2;
        public override float DefenceModificator => 0.5f;
    }
}

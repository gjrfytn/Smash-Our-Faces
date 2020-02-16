namespace Sof.Model.MapObject
{
    public class House : Property
    {
        public override int MoveCostModificator => -2;
        public override float DefenceModificator => 0.5f;

        public House(ITime time, int income) : base(time, income)
        {
        }
    }
}

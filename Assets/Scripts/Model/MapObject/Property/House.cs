using Gjrfytn.Dim;

namespace Sof.Model.MapObject.Property
{
    public class House : Property
    {
        public override int MoveCostModificator => -2;
        public override float DefenceModificator => 0.5f;

        public House(ITime time, IMap map, PositiveInt income, PositiveInt heal) : base(time, map, income, heal)
        {
        }
    }
}

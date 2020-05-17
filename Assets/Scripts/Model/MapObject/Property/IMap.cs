namespace Sof.Model.MapObject.Property
{
    public interface IMap
    {
        Tile GetTileOf(Property property);
        Unit GetUnitIn(Property property);
    }
}
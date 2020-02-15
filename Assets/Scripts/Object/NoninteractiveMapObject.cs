namespace Sof.Object
{
    public class NoninteractiveMapObject : MapObject
    {
        public override bool OnHover() => false;
        public override bool OnLeftClick() => false;
        public override bool OnRightClick() => false;
    }
}

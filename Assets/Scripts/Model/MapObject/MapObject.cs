namespace Sof.Model.MapObject
{
    public class MapObject //TODO abstract
    {
        public MapObjectType Type { get; }

        public MapObject(MapObjectType mapObjectType)
        {
            Type = mapObjectType;
        }

        public int MoveCostModificator //TODO abstract 
        {
            get
            {
                switch (Type)
                {
                    case MapObjectType.None:
                        return 0;
                    case MapObjectType.Castle:
                        return -2;
                    case MapObjectType.House:
                        return -2;
                    case MapObjectType.Bridge:
                        return -2;
                    case MapObjectType.Road:
                        return -2;
                    case MapObjectType.Forest:
                        return 2;
                    default:
                        throw new System.ArgumentOutOfRangeException(nameof(Type));
                }
            }
        }
        public float DefenceModificator //TODO abstract 
        {
            get
            {
                switch (Type)
                {
                    case MapObjectType.None:
                        return 0;
                    case MapObjectType.Castle:
                        return 0.75f;
                    case MapObjectType.House:
                        return 0.5f;
                    case MapObjectType.Bridge:
                        return -0.25f;
                    case MapObjectType.Road:
                        return 0;
                    case MapObjectType.Forest:
                        return 0.5f;
                    default:
                        throw new System.ArgumentOutOfRangeException(nameof(Type));
                }
            }
        }
    }
}

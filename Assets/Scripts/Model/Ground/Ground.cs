namespace Sof.Model.Ground
{
    public class Ground //TODO abstract
    {
        public GroundType Type { get; }

        public Ground(GroundType groundType)
        {
            Type = groundType;
        }

        public int MoveCost //TODO abstract
        {
            get
            {
                switch (Type)
                {
                    case GroundType.Water:
                        return 6;
                    case GroundType.Grass:
                        return 3;
                    case GroundType.Mountain:
                        return 6;
                    default:
                        throw new System.ArgumentOutOfRangeException(nameof(GroundType));
                }
            }
        }

        public float Defence //TODO abstract
        {
            get
            {
                switch (Type)
                {
                    case GroundType.Water:
                        return 0;
                    case GroundType.Grass:
                        return 0.25f;
                    case GroundType.Mountain:
                        return 0.5f;
                    default:
                        throw new System.ArgumentOutOfRangeException(nameof(GroundType));
                }
            }
        }
    }
}

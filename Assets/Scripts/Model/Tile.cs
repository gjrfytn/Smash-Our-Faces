﻿
using Sof.Auxiliary;

namespace Sof.Model
{
    public class Tile
    {
        public Ground.Ground Ground { get; }
        public MapObject.MapObject Object { get; }
        public Unit Unit { get; private set; }

        public PositiveInt MoveCost => new PositiveInt(Ground.MoveCost.Value + (Object?.MoveCostModificator ?? 0));
        public float Defence => UnityEngine.Mathf.Min(1, Ground.Defence + (Object?.DefenceModificator ?? 0));

        public bool Blocked => Unit != null;

        public event System.Action<Unit> UnitRemoved;

        public Tile(Ground.Ground ground, MapObject.MapObject @object)
        {
            Ground = ground ?? throw new System.ArgumentNullException(nameof(ground));
            Object = @object;
        }

        public void PlaceUnit(Unit unit)
        {
            if (Unit != null)
                throw new System.InvalidOperationException("Tile cannot contain more than one unit.");

            Unit = unit;
            Unit.Died += RemoveUnit;

            if (Object is MapObject.Property.Property property) //TODO
                property.Owner = unit.Faction;
        }

        public void RemoveUnit()
        {
            if (Unit == null)
                throw new System.InvalidOperationException("Tile has no unit.");

            Unit.Died -= RemoveUnit;
            var unit = Unit;
            Unit = null;

            UnitRemoved?.Invoke(unit);
        }
    }
}

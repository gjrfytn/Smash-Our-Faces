using UnityEngine;

namespace Sof.Object
{
    public class Tile : Auxiliary.SofMonoBehaviour
    {
        private UIManager _UIManager;
        private MapObject _Object;

        public Model.Tile ModelTile { get; private set; }

        public void Initialize(Model.Tile tile, UIManager uiManager)
        {
            ModelTile = tile ?? throw new System.ArgumentNullException(nameof(tile));
            _UIManager = uiManager != null ? uiManager : throw new System.ArgumentNullException(nameof(uiManager));
        }

        public T InstantiateMapObject<T>(T @object) where T : MapObject
        {
            if (@object == null)
                throw new System.ArgumentNullException(nameof(@object));

            if (_Object != null)
                throw new System.InvalidOperationException("Tile already has object.");

            var objectInstance = Instantiate(@object, transform);

            _Object = objectInstance;

            return objectInstance;
        }

        private void OnMouseEnter()
        {
            if (_Object == null || !_Object.OnHover())
                _UIManager.OnTileHover(this);
        }

        private async void OnMouseOver()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (_Object == null || !_Object.OnLeftClick())
                    await _UIManager.OnTileLeftClick(this);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                if (_Object == null || !_Object.OnRightClick())
                    _UIManager.OnTileRightClick(this);
            }
        }
    }
}
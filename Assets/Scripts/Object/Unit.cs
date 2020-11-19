using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sof.Auxiliary;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Sof.Object
{
    public class Unit : MonoBehaviour, Model.MapObject.Property.Castle.IUnitTemplate
    {
#pragma warning disable 0649
        [SerializeField]
        private SpriteRenderer _MoveTile;
        [SerializeField]
        private SpriteRenderer _AttackTile;

        [SerializeField]
        private int _Speed;
        [SerializeField]
        private int _Health;
        [SerializeField]
        private int _Damage;
        [SerializeField]
        private int _AttackRange;
        [SerializeField]
        private int _GoldCost;
        [SerializeField]
        private SpriteRenderer _OwnerFactionSprite;
#pragma warning restore 0649

        private readonly List<SpriteRenderer> _UI_Sprites = new List<SpriteRenderer>();

        public Model.Unit ModelUnit { get; private set; }

        public PositiveInt MovePoints => new PositiveInt(_Speed);
        public PositiveInt Health => new PositiveInt(_Health);
        public PositiveInt Damage => new PositiveInt(_Damage);
        public PositiveInt AttackRange => new PositiveInt(_AttackRange);
        public PositiveInt GoldCost => new PositiveInt(_GoldCost);

        private GameManager _GameManager;
        private UIManager _UIManager;
        private Map _Map;

        public void Initialize(Model.Unit unit, GameManager gameManager, UIManager uiManager, Map map)
        {
            _GameManager = gameManager != null ? gameManager : throw new System.ArgumentNullException(nameof(gameManager));
            _UIManager = uiManager != null ? uiManager : throw new System.ArgumentNullException(nameof(uiManager));
            _Map = map != null ? map : throw new System.ArgumentNullException(nameof(map));

            ModelUnit = unit ?? throw new System.ArgumentNullException(nameof(unit));
            ModelUnit.MovingAlongPath.AddSubscriber(ModelUnit_MovingAlongPath);
            ModelUnit.Attacked.AddSubscriber(ModelUnit_Attacked);
            ModelUnit.TookHit += ModelUnit_TookHit;
            ModelUnit.Healed += ModelUnit_Healed;
            ModelUnit.Died.AddSubscriber(ModelUnit_Died);

            map.ModelMap.UnitBanished += ModelMap_UnitBanished;

            SetOwnerColor();
        }

        public void ShowUI()
        {
            var moveArea = ModelUnit.GetMoveRange().ToArray();

            if (moveArea.Length > 1)
                foreach (var movePoint in moveArea)
                    _UI_Sprites.Add(Instantiate(_MoveTile, _Map.GetWorldPos(movePoint.Tile), Quaternion.identity, transform));

            var attackArea = ModelUnit.GetAttackArea().ToArray();

            if (attackArea.Length > 1)
                foreach (var attackTile in attackArea)
                    _UI_Sprites.Add(Instantiate(_AttackTile, _Map.GetWorldPos(attackTile), Quaternion.identity, transform));
        }

        public void HideUI()
        {
            foreach (var @object in _UI_Sprites)
                Destroy(@object.gameObject);

            _UI_Sprites.Clear();
        }

        private Task ModelUnit_MovingAlongPath(IEnumerable<Model.Tile> path)
        {
            if (path == null)
                throw new System.ArgumentNullException(nameof(path));

            return FollowPath(path);
        }

        private Task ModelUnit_Attacked()
        {
            return PlayAttack();
        }

        private void ModelUnit_TookHit(PositiveInt damage)
        {
            _UIManager.OnUnitHit(this, damage);
        }

        private void ModelUnit_Healed(PositiveInt heal)
        {
            _UIManager.OnUnitHeal(this, heal);
        }

        private Task ModelUnit_Died()
        {
            if (ModelUnit.Critical)
            {
                _UIManager.OnCriticalUnitDeath(ModelUnit.Faction);
            }

            DestroySelf();

            return Task.CompletedTask;
        }

        private void ModelMap_UnitBanished(Model.Unit unit)
        {
            if (unit == null)
                throw new System.ArgumentNullException(nameof(unit));

            if (ModelUnit == unit)
                DestroySelf();
        }

        private void DestroySelf() => Destroy(gameObject);

        private Task FollowPath(IEnumerable<Model.Tile> path)
        {
            return SuspendUI(followPath);

            async Task followPath()
            {
                foreach (var tile in path)
                {
                    Vector3 tilePos = _Map.GetWorldPos(tile);
                    while (transform.position != tilePos)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, tilePos, Time.deltaTime);

                        await Task.Yield();
                    }
                }
            }
        }

        private Task PlayAttack()
        {
            return SuspendUI(playAttack);

            async Task playAttack()
            {
                transform.localRotation = Quaternion.Euler(0, 0, -30);

                await Task.Delay(500);

                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        private async Task SuspendUI(System.Func<Task> action)
        {
            _UIManager.DisableUIInteraction = true;
            var shouldRestoreUI = _UI_Sprites.Any();
            HideUI();

            await action();

            if (shouldRestoreUI)
                ShowUI();

            _UIManager.DisableUIInteraction = false;
        }

        private void SetOwnerColor()
        {
            _OwnerFactionSprite.color = _GameManager.GetFactionColor(ModelUnit.Faction);
        }
    }
}
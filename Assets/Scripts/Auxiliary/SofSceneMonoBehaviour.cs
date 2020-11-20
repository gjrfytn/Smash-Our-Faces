using System.Linq;
using UnityEngine;

namespace Sof.Auxiliary
{
    public abstract class SofSceneMonoBehaviour : MonoBehaviour
    {
        protected void Awake()
        {
            var type = GetType();
            var serializableFieldsWithNull = type.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                                 .Where(f => f.IsDefined(typeof(SerializeField), false))
                                                 .Where(f => f.GetValue(this) == null)
                                                 .ToArray();

            if (serializableFieldsWithNull.Any())
                throw new System.Exception($"'{type}' - some serializable fields are not set: '{string.Join(", ", serializableFieldsWithNull.Select(f => f.Name))}'.");

            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}
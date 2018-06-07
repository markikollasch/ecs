using System;
using System.Collections.Generic;

namespace Ecs
{
    public class Entity
    {
        public Guid Id { get; }
        private IDictionary<Type, Object> Components { get; }

        public Entity(Guid id)
        {
            Id = id;
            Components = new Dictionary<Type, object>();
        }

        public void SetComponent<T>(T? component) where T : struct
        {
            if (component.HasValue)
            {
                Components[typeof(T)] = component.Value;
            }
            else
            {
                Components.Remove(typeof(T));
            }
        }

        public T? GetComponent<T>() where T : struct
        {
            if (Components.TryGetValue(typeof(T), out object found))
            {
                return found as T?;
            }
            else
            {
                return null;
            }
        }

        public (T1, T2)? GetComponents<T1, T2>() where T1 : struct where T2 : struct
        {
            T1? component1 = GetComponent<T1>();
            if (!component1.HasValue)
            {
                return null;
            }

            T2? component2 = GetComponent<T2>();
            if (!component2.HasValue)
            {
                return null;
            }

            return (component1.Value, component2.Value);
        }

        public Entity Clone(Guid id)
        {
            Entity other = new Entity(id);
            foreach (var kvp in Components)
            {
                other.Components[kvp.Key] = kvp.Value;
            }
            return other;
        }
    }
}

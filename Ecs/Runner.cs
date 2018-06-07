using System;
using System.Collections.Generic;
using System.Linq;

namespace Ecs
{
    public static class Runner
    {
        public static IEnumerable<TResult> Run<TComponent, TResult>(
            IEnumerable<Entity> entities,
            Func<Guid, TComponent, TResult> system,
            Func<Entity, TComponent?> projection)
        where TComponent : struct
        {
            return entities
                .Select(entity => (entity: entity, component: projection(entity)))
                .Where(pair => pair.component.HasValue)
                .Select(pair => system(pair.entity.Id, pair.component.Value));
        }

        public static IEnumerable<TResult> Run<TComponent, TResult>(
            IEnumerable<Entity> entities,
            Func<Guid, TComponent, TResult> system
        )
        where TComponent : struct
        {
            return Run(entities, system, e => e.GetComponent<TComponent>());
        }

        public static MultiRunner Multi { get { return new MultiRunner(); } }

        public class MultiRunner
        {
            private class System
            {
                public static System Of<TComponent>(Action<Guid, TComponent> process, Func<Entity, TComponent?> projection)
                where TComponent : struct
                {
                    return new System(
                        (id, component) => process(id, (TComponent)component),
                        entity => projection(entity));
                }

                private System(Action<Guid, object> process, Func<Entity, object> projection)
                {
                    Process = process;
                    Projection = projection;
                }
                public Action<Guid, object> Process { get; }
                public Func<Entity, object> Projection { get; }
            }

            private IDictionary<Type, System> Systems { get; } = new Dictionary<Type, System>();

            public MultiRunner Add<TComponent>(
                Action<Guid, TComponent> system,
                Func<Entity, TComponent?> projection)
            where TComponent : struct
            {
                Systems.Add(typeof(TComponent), System.Of(system, projection));
                return this;
            }

            public MultiRunner Add<TComponent>(Action<Guid, TComponent> system) where TComponent : struct
            {
                return Add(system, e => e.GetComponent<TComponent>());
            }

            public void Run(IEnumerable<Entity> entities)
            {
                foreach (var entity in entities)
                {
                    foreach (var kvp in Systems)
                    {
                        object component = kvp.Value.Projection(entity);
                        if (component != null)
                        {
                            kvp.Value.Process(entity.Id, component);
                        }
                    }
                }
            }
        }
    }
}

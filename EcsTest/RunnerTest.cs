using System;
using Ecs;
using Xunit;

namespace EcsTest
{
    public class RunnerTest
    {
        private Func<Guid, Component<T>, (Guid, Component<T>)> ReturnsArguments<T>()
        {
            return (Guid id, Component<T> component) => (id, component);
        }

        [Fact]
        public void DoesNotProcessEntitiesWithoutComponents()
        {
            Entity entity = new Entity(Guid.NewGuid());
            var result = Runner.Run(new[] { entity }, ReturnsArguments<string>());
            Assert.Empty(result);
        }

        [Fact]
        public void ProcessesEntities()
        {
            Entity entity = CreateEntity("value");
            var result = Runner.Run(new[] { entity }, ReturnsArguments<string>());
            Assert.Collection(result, pair =>
            {
                Assert.Equal((entity.Id, Component.Of("value")), pair);
            });
        }

        [Fact]
        public void ProcessesMultipleEntities()
        {
            Entity first = CreateEntity("first");
            Entity second = CreateEntity("second");

            var result = Runner.Run(new[] { first, second }, ReturnsArguments<string>());

            Assert.Collection(result,
                pair => Assert.Equal((first.Id, Component.Of("first")), pair),
                pair => Assert.Equal((second.Id, Component.Of("second")), pair)
            );
        }

        [Fact]
        public void ProcessesOnlyEntitiesWithComponents()
        {
            Entity wantedEntity = CreateEntity("value");
            Entity unwantedEntity = new Entity(Guid.NewGuid());
            unwantedEntity.SetComponent<long>(2L);
            var result = Runner.Run(new[] { unwantedEntity, wantedEntity }, ReturnsArguments<string>());

            Assert.Collection(result,
                pair => Assert.Equal((wantedEntity.Id, Component.Of("value")), pair)
            );
        }

        [Fact]
        public void ProcessesProjectedEntities()
        {
            Entity wantedEntity = CreateEntity("value");
            Entity unwantedEntity = new Entity(Guid.NewGuid());
            unwantedEntity.SetComponent<int>(2);
            var result = Runner.Run(
                new[] { unwantedEntity, wantedEntity },
                ReturnsArguments<int>(),
                entity =>
                {
                    var component = entity.GetComponent<Component<string>>();
                    if (!component.HasValue)
                    {
                        return null;
                    }
                    return component.Value.Value.Length;
                });

            Assert.Collection(result,
                pair => Assert.Equal((wantedEntity.Id, Component.Of(5)), pair)
            );
        }

        private Entity CreateEntity<T>(T value)
        {
            Entity entity = new Entity(Guid.NewGuid());
            entity.SetComponent<Component<T>>(Component.Of(value));
            return entity;
        }
    }
}

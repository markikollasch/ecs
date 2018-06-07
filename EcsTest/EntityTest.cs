using System;
using Ecs;
using Xunit;

namespace EcsTest
{
    public class EntityTest
    {
        [Fact]
        public void HasAnId()
        {
            Guid testGuid = Guid.NewGuid();
            Entity entity = new Entity(testGuid);
            Assert.Equal(testGuid, entity.Id);
        }

        [Fact]
        public void ReturnsNothingWhenNoComponent()
        {
            Entity entity = new Entity(Guid.NewGuid());
            var result = entity.GetComponent<Component<string>>();
            Assert.False(result.HasValue);
        }

        [Fact]
        public void ReturnsGivenComponent()
        {
            Entity entity = new Entity(Guid.NewGuid());
            Component<string> testValue = "something";

            entity.SetComponent<Component<string>>(testValue);
            Component<string>? result = entity.GetComponent<Component<string>>();

            Assert.True(result.HasValue);
            Assert.Equal(testValue, result.Value);
        }

        [Fact]
        public void OverwritesComponent()
        {
            Entity entity = new Entity(Guid.NewGuid());
            var first = "first";
            var second = "second";

            entity.SetComponent<Component<string>>(first);
            entity.SetComponent<Component<string>>(second);

            Component<string>? result = entity.GetComponent<Component<string>>();

            Assert.True(result.HasValue);
            Assert.Equal(second, result.Value);
        }

        [Fact]
        public void ClearsComponent()
        {

            Entity entity = new Entity(Guid.NewGuid());
            var testValue = "something";

            entity.SetComponent<Component<string>>(testValue);
            entity.SetComponent<Component<string>>(null);
            Component<string>? result = entity.GetComponent<Component<string>>();

            Assert.False(result.HasValue);
        }

        [Fact]
        public void ContainsMultipleComponents()
        {
            Entity entity = new Entity(Guid.NewGuid());
            var testString = new Component<string>("something");
            var testInt = new Component<int>(4);

            entity.SetComponent<Component<string>>(testString);
            entity.SetComponent<Component<int>>(testInt);

            Assert.Equal(testString, entity.GetComponent<Component<string>>().Value);
            Assert.Equal(testInt, entity.GetComponent<Component<int>>().Value);
        }

        [Fact]
        public void ClonesEntityWithComponents()
        {
            Entity entity1 = new Entity(Guid.NewGuid());
            entity1.SetComponent<Component<string>>("value");

            Entity entity2 = entity1.Clone(Guid.NewGuid());
            Assert.NotEqual(entity1.Id, entity2.Id);
            Assert.Equal("value", entity2.GetComponent<Component<string>>());
        }

        [Fact]
        public void CopiesComponentsToClone()
        {
            Entity entity1 = new Entity(Guid.NewGuid());
            entity1.SetComponent<Component<string>>("unchanged");

            Entity entity2 = entity1.Clone(Guid.NewGuid());
            entity2.SetComponent<Component<string>>("changed");

            Assert.Equal("changed", entity2.GetComponent<Component<string>>());
            Assert.Equal("unchanged", entity1.GetComponent<Component<string>>());
        }

        [Fact]
        public void GetsMultipleComponents()
        {
            Entity entity = new Entity(Guid.NewGuid());
            var testString = new Component<string>("something");
            var testInt = new Component<int>(4);

            entity.SetComponent<Component<string>>(testString);
            entity.SetComponent<Component<int>>(testInt);

            var result = entity.GetComponents<Component<string>, Component<int>>();
            Assert.True(result.HasValue);
            Assert.Equal("something", result.Value.Item1);
            Assert.Equal((int)4, (int)result.Value.Item2);
        }

        [Fact]
        public void OnlyGetsComponentsThatExist()
        {
            Entity entity = new Entity(Guid.NewGuid());
            var testString = new Component<string>("something");

            entity.SetComponent<Component<string>>(testString);

            var result = entity.GetComponents<Component<string>, Component<int>>();
            Assert.False(result.HasValue);
        }
    }
}

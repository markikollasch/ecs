using System;
using System.Collections.Generic;
using Ecs;
using Xunit;

namespace EcsTest
{
    public class MultiRunnerTest
    {
        [Fact]
        public void ProcessesEntities()
        {
            Entity noComponents = new Entity(Guid.NewGuid());
            Entity oneComponent = new Entity(Guid.NewGuid());
            oneComponent.SetComponent<Component<string>>(new Component<string>("one"));
            Entity otherComponent = new Entity(Guid.NewGuid());
            otherComponent.SetComponent<Component<bool>>(new Component<bool>(true));
            Entity bothComponents = new Entity(Guid.NewGuid());
            bothComponents.SetComponent<Component<string>>(new Component<string>("two"));
            bothComponents.SetComponent<Component<bool>>(new Component<bool>(false));

            List<string> reports = new List<string>();

            Runner.Multi
                .Add((Guid id, Component<bool> component) => reports.Add(string.Format("Boolean : {0} : {1}", id, component.Value.ToString())))
                .Add((Guid id, Component<string> component) => reports.Add(string.Format("String : {0} : {1}", id, component.Value.ToString())))
                .Run(new[] { noComponents, oneComponent, otherComponent, bothComponents });

            Assert.Equal(new string[]{
                "String : " + oneComponent.Id.ToString() + " : one",
                "Boolean : " + otherComponent.Id.ToString() + " : " + true.ToString(),
                "Boolean : " + bothComponents.Id.ToString() + " : " + false.ToString(),
                "String : " + bothComponents.Id.ToString() + " : two",
            }, reports);
        }
    }
}

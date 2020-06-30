using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreTesting
{
    public class BasicTest
    {
        [Fact]
        public void Example_Scenario_Failing_Test()
        {
            var dataParent = new DataParent()
            {
                Id = "123",
                Value = "new",
                Children = new List<DataChild>() {
                    new DataChild()
                    {
                        Id = "1234",
                        Value = "new"
                    }
                }
            };

            var domainParent = dataParent.ToDomainParent();

            var child = domainParent.Children.FirstOrDefault();

            child.Value = "updated";

            Assert.Equal("updated", child.Value); // True
            Assert.Equal("updated", domainParent.Children.ElementAt(0).Value); // False
        }

        [Fact]
        public void Example_Scenario_Passing_Test()
        {
            var dataParent = new DataParent()
            {
                Id = "123",
                Value = "new",
                Children = new List<DataChild>() {
                    new DataChild()
                    {
                        Id = "1234",
                        Value = "new"
                    }
                }
            };

            var domainParent = dataParent.MapToDomainParentCorrectly();

            var child = domainParent.Children.FirstOrDefault();

            child.Value = "updated";

            Assert.Equal("updated", child.Value); // True
            Assert.Equal("updated", domainParent.Children.ElementAt(0).Value); // True
        }

        [Fact]
        public void Call_First_Or_Default_More_Than_Once_Objects_Shoule_Be_The_Same_Failing()
        {
            var dataParent = new DataParent()
            {
                Id = "123",
                Value = "new",
                Children = new List<DataChild>() {
                    new DataChild()
                    {
                        Id = "1234",
                        Value = "new"
                    }
                }
            };

            var domainParent = dataParent.ToDomainParent();

            var child = domainParent.Children.FirstOrDefault();
            var child2 = domainParent.Children.FirstOrDefault();

            Assert.True(child.Equals(child2));
        }
    }

    public static class Mappers
    {
        public static DomainParent ToDomainParent(this DataParent dataParent)
        {
            var children = dataParent.Children.Select(x => new DomainChild
            {
                Id = x.Id,
                Value = x.Value
            });

            return new DomainParent
            {
                Id = dataParent.Id,
                Value = dataParent.Value,
                Children = children
            };
        }

        public static DomainParent MapToDomainParentCorrectly(this DataParent dataParent)
        {
            var children = dataParent.Children.Select(x => new DomainChild
            {
                Id = x.Id,
                Value = x.Value
            });

            return new DomainParent
            {
                Id = dataParent.Id,
                Value = dataParent.Value,
                Children = children.ToList()
            };
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EFCoreTesting
{
    public class IEnumerableTests
    {
        [Fact]
        public void Update_IEnumerable()
        {
            var parent = new Parent
            {
                Id = "4C2AF538-67AF-448C-A015-B888B15930DE",
                Children = new List<Child>
                {
                    new Child
                    {
                        Id = "16E6C2B9-3D67-491C-8BE3-E1689D15DBAC"
                    }
                }
            };

            var child = parent.Children.FirstOrDefault();

            child.Id = "updated";

            Assert.Equal("updated", parent.Children.ElementAt(0).Id);
        }

        public class Parent
        {
            public string Id { get; set; }
            public IEnumerable<Child> Children { get; set; }
        }

        public class Child
        {
            public string Id { get; set; }
        }
    }
}

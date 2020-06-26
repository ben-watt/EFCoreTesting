using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFCoreTesting
{
    public class Tests
    {
        [Fact]
        public async Task Parent_with_single_child()
        {
            const string parentId = "8A0EBECB-C231-4C7C-A5CC-14B599687F3A";

            var parent = new DomainParent
            {
                Id = parentId,
                Value = "new",
                Children = new List<DomainChild>
                {
                    new DomainChild
                    {
                        Id = "D4506C2B-C837-4BF3-BAB6-D131EC8E296F",
                        Value = "new"
                    }
                }
            };

            using var database = new Database();

            await database.Add(parent);

            var result = await database.Get(parentId);

            Assert.NotNull(result);
            Assert.Equal(parentId, result.Id);
            Assert.NotEmpty(result.Children);
            Assert.Single(result.Children);
            Assert.Equal("D4506C2B-C837-4BF3-BAB6-D131EC8E296F", result.Children.ElementAt(0).Id);
        }

        [Fact]
        public async Task Parent_with_multiple_children()
        {
            const string parentId = "8A0EBECB-C231-4C7C-A5CC-14B599687F3A";

            var parent = new DomainParent
            {
                Id = parentId,
                Value = "new",
                Children = new List<DomainChild>
                {
                    new DomainChild
                    {
                        Id = "D4506C2B-C837-4BF3-BAB6-D131EC8E296F",
                        Value = "new"
                    },
                    new DomainChild
                    {
                        Id = "C87E84DA-D9BD-419C-851E-AB8693D357D9",
                        Value = "new"
                    }
                }
            };

            using var database = new Database();

            await database.Add(parent);

            var result = await database.Get(parentId);

            Assert.NotNull(result);
            Assert.Equal(parentId, result.Id);
            Assert.NotEmpty(result.Children);
            Assert.Equal(2, result.Children.Count());
            Assert.Equal("D4506C2B-C837-4BF3-BAB6-D131EC8E296F", result.Children.ElementAt(0).Id);
            Assert.Equal("C87E84DA-D9BD-419C-851E-AB8693D357D9", result.Children.ElementAt(1).Id);
        }

        [Fact]
        public async Task Add_child()
        {
            const string parentId = "8A0EBECB-C231-4C7C-A5CC-14B599687F3A";

            var parent = new DomainParent
            {
                Id = parentId,
                Value = "new"
            };

            using var database = new Database();
            
            await database.Add(parent);

            var result = await database.Get(parentId);

            Assert.NotNull(result);
            Assert.Equal(parentId, result.Id);
            Assert.Empty(result.Children);

            result.Children = new List<DomainChild>
            {
                new DomainChild
                {
                    Id = "D4506C2B-C837-4BF3-BAB6-D131EC8E296F",
                    Value = "new"
                }
            };

            await database.Update(result);

            var final = await database.Get(parentId);

            Assert.NotNull(final);
            Assert.Equal(parentId, final.Id);
            Assert.NotEmpty(final.Children);
            Assert.Single(final.Children);
            Assert.Equal("D4506C2B-C837-4BF3-BAB6-D131EC8E296F", final.Children.ElementAt(0).Id);
        }

        [Fact]
        public async Task Update_child()
        {
            const string parentId = "8A0EBECB-C231-4C7C-A5CC-14B599687F3A";

            var parent = new DomainParent
            {
                Id = parentId,
                Value = "new",
                Children = new List<DomainChild>
                {
                    new DomainChild
                    {
                        Id = "D4506C2B-C837-4BF3-BAB6-D131EC8E296F",
                        Value = "new"
                    }
                }
            };

            using var database = new Database();

            await database.Add(parent);

            var result = await database.Get(parentId);

            Assert.NotNull(result);
            Assert.Equal(parentId, result.Id);
            Assert.NotEmpty(result.Children);
            Assert.Single(result.Children);
            Assert.Equal("D4506C2B-C837-4BF3-BAB6-D131EC8E296F", result.Children.ElementAt(0).Id);

            var child = result.Children.FirstOrDefault();

            child.Value = "updated";

            await database.Update(result);

            var final = await database.Get(parentId);

            Assert.NotNull(final);
            Assert.Equal(parentId, final.Id);
            Assert.NotEmpty(final.Children);
            Assert.Single(final.Children);
            Assert.Equal("updated", final.Children.ElementAt(0).Value);
        }
    }

    public class Database : IDisposable
    {
        private readonly DbContextOptions<MyContext> _options;

        public Database()
        {
            _options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase("my-context")
                .Options;
        }

        public async Task Add(DomainParent parent)
        {
            await using var context = new MyContext(_options);

            await context.AddAsync(parent.ToDataParent());
            await context.SaveChangesAsync();
        }

        public async Task Update(DomainParent parent)
        {
            await using var context = new MyContext(_options);

            var dataParent = await context.Parents
                .Include(x => x.Children)
                .Where(x => x.Id == parent.Id)
                .FirstOrDefaultAsync();

            foreach (var child in parent.Children)
            {
                var dataChild = dataParent.Children.FirstOrDefault(x => x.Id == child.Id);

                if (dataChild == null)
                {
                    continue;
                }

                dataChild.Value = child.Value;
            }

            await context.SaveChangesAsync();
        }

        public async Task<DomainParent> Get(string id)
        {
            await using var context = new MyContext(_options);

            var query = context.Parents
                .Include(x => x.Children)
                .Where(x => x.Id == id);

            var parent = await query.FirstOrDefaultAsync();

            //await query.Include(x => x.Children).LoadAsync();

            return parent.ToDomainParent();
        }

        public void Dispose()
        {
            using var context = new MyContext(_options);

            context.Database.EnsureDeleted();
        }
    }

    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }

        public DbSet<DataParent> Parents { get; set; }
    }

    public class DataParent
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public ICollection<DataChild> Children { get; set; }

        public DomainParent ToDomainParent()
        {
            return new DomainParent
            {
                Id = Id,
                Children = Children?.Select(x => new DomainChild
                {
                    Id = x.Id
                })
            };
        }
    }

    public class DataChild
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }

    public class DomainParent
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public IEnumerable<DomainChild> Children { get; set; }

        public DataParent ToDataParent()
        {
            return new DataParent
            {
                Id = Id,
                Children = Children?.Select(x => new DataChild
                {
                    Id = x.Id
                }).ToList()
            };
        }
    }

    public class DomainChild
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}

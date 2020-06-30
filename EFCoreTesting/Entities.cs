using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFCoreTesting
{
    public class DataParent
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public ICollection<DataChild> Children { get; set; }
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
        public IReadOnlyCollection<DomainChild> Children { get; set; }
    }

    public class DomainChild
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}

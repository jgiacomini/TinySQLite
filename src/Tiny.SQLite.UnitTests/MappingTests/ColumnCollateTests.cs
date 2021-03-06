﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tiny.SQLite.Attributes;

namespace Tiny.SQLite.UnitTests
{
    [TestClass]
    public class ColumnCollateTests : BaseColumnTest
    {
        public class CollateColumnTable
        {
            [Collation(Collate.Binary)]
            public string BINARY { get; set; }

            [Collation(Collate.NoCase)]
            public string NOCASE { get; set; }

            [Collation(Collate.RTrim)]
            public string RTRIM { get; set; }
        }

        [TestMethod]
        public void TestCollationColumn()
        {
            TableMapper mapper = new TableMapper(true, true);
            var mapping = mapper.Map<CollateColumnTable>();

            var column0 = GetColumnByPropertyName(mapping, nameof(CollateColumnTable.BINARY));
            var column1 = GetColumnByPropertyName(mapping, nameof(CollateColumnTable.NOCASE));
            var column2 = GetColumnByPropertyName(mapping, nameof(CollateColumnTable.RTRIM));

            Assert.IsTrue(column0.Collate == Collate.Binary, "Column 'BINARY' not well mapped");
            Assert.IsTrue(column1.Collate == Collate.NoCase, "Column 'NOCASE' not well mapped");
            Assert.IsTrue(column2.Collate == Collate.RTrim, "Column 'RTRIM' not well mapped");
        }

        [TestMethod]
        public async Task CreateTableWithCollation()
        {
            using (var context = new DbContext(PathOfDb))
            {
                try
                {
                    var table = context.Table<CollateColumnTable>();
                    await table.CreateAsync();

                    Assert.IsTrue(await table.ExistsAsync());
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
        }
    }
}
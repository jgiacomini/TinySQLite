﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tiny.SQLite.Attributes;

namespace Tiny.SQLite.UnitTests
{
    [TestClass]
    public class ColumnMaxLenghtTest : BaseColumnTest
    {
        public class MaxLenghtTable
        {
            [MaxLength(255)]
            public string Max { get; set; }
        }

        [TestMethod]
        public void TestMaxLenght()
        {
            TableMapper mapper = new TableMapper(true, true);
            var mapping = mapper.Map<MaxLenghtTable>();
            var column = GetColumnByPropertyName(mapping, "Max");

            Assert.IsTrue(column.ColumnType == "VARCHAR(255)", $"Expected :VARCHAR(255), current : {column.ColumnType} ");
        }
    }
}

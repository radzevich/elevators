using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace Elevators.FakeExmo.Helpers
{
    public static class CsvHelper
    {
        public static TestTicks[] ReadTestTicks(string filePath)
        {
            using var streamReader = new StreamReader(filePath);
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };
            csvConfig.RegisterClassMap<TestTicksClassMap>();

            using var csvReader = new CsvReader(streamReader, csvConfig);
            var records = csvReader.GetRecords<TestTicks>().ToArray();

            return records;
        }

        public class TestTicks
        {
            public DateTime Date { get; set; }
            public decimal BtcOpen { get; set; }
            public decimal BtcClose { get; set; }
            public decimal EthOpen { get; set; }
            public decimal EthClose { get; set; }
            public decimal LtcOpen { get; set; }
            public decimal LtcClose { get; set; }
            public decimal XrpOpen { get; set; }
            public decimal XrpClose { get; set; }
            public decimal UsdtOpen { get; set; }
            public decimal UsdtClose { get; set; }
        }

        private sealed class TestTicksClassMap : ClassMap<TestTicks>
        {
            public TestTicksClassMap()
            {
                Map(x => x.Date).Index(0);
                Map(x => x.BtcOpen).Index(1);
                Map(x => x.BtcClose).Index(2);
                Map(x => x.EthOpen).Index(3);
                Map(x => x.EthClose).Index(4);
                Map(x => x.LtcOpen).Index(5);
                Map(x => x.LtcClose).Index(6);
                Map(x => x.XrpOpen).Index(7);
                Map(x => x.XrpClose).Index(8);
                Map(x => x.UsdtOpen).Index(9);
                Map(x => x.UsdtClose).Index(10);
            }
        }
    }
}
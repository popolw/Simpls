using NUnit.Framework;
using System.Collections.Concurrent;
using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System;

namespace Simpls
{
    [TestFixture]
    public class QueueTest
    {
        [Test]
        public async Task Queue()
        {
            //MessageChannelNotify notify = new MessageChannelNotify();
            //await notify.SendMessageAsync("ABC", "HELLO");
            //await notify.SendMessageAsync("ABCD", "HELLO");
            //notify.OnRevice = (channe, messgge,ack) => {
            //   return Task.Run(() => {
            //       //ack.Ack();
            //   });
            //};

            var x = new NotifyMessage<string>("AA", "BB");
            var y = new NotifyMessage< string>("AA", "ZZ");
            var z = new NotifyMessage<string>("AA", "BB");

            ConcurrentDictionary<NotifyMessage<string>, bool> set = new ConcurrentDictionary<NotifyMessage<string>, bool>();
            set.GetOrAdd(x, (key) => true);
            set.GetOrAdd(y, (key) => true);
            set.GetOrAdd(z, (key) => true);
            var b = false;
            IEquatable<bool> eq = b;
            eq.Equals(true);
            // item.Ack();
        }

        [Test]
        public void Test()
        {

            //var sql= @"declare @table table(
            //    [Id] int primary key identity,
            //    [Date] varchar(50),
            //    [StartDate] datetime,
            //    [AMessage] nvarchar(2000),
            //    [EndDate] datetime,
            //    [BMessage] nvarchar(2000),
            //    [Elapase] int
            //)
            //insert into @table([Date], [StartDate], [AMessage], [EndDate], [BMessage], [Elapase])
            //select CONVERT(varchar, a.UDate, 23) [Date],a.UDate as StartDate, a.UMessage AMessage,
            //b.UDate as EndDate,b.UMessage BMessage, DATEDIFF(SECOND, a.UDate, b.UDate) Elapsed
            //from UTT a
            //left join UTT b on a.UGroup = b.UGroup and CONVERT(varchar, a.UDate, 23) = CONVERT(varchar, b.UDate, 23)
            //where a.UType = 1 and b.UType = 2
            //order by a.Id asc

            //select[Date],COUNT([Date]) UCount,AVG(Elapase)[AGV],MAX(Elapase) MX,MIN(Elapase) MI
            //from @table
            //group by[Date] order by[Date]

            //select* from @table order by Id";

            var files = Directory.GetFiles(@"G:\UV4.3\logs", "*Log.txt");
            var table = new DataTable("UTT");
            table.Columns.Add("UDate", typeof(DateTime));
            table.Columns.Add("UType", typeof(int));
            table.Columns.Add("UGroup", typeof(int));
            table.Columns.Add("UMessage", typeof(string));

            foreach (var file in files) 
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HHmmss:fff}] Analysis File {file}.......");
                this.Read(file, table);
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HHmmss:fff}] Analysis Completed");
            }

            string connectionString = "Server=(LocalDB)\\MSSQLLocalDB;Database=UVAnalysis;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.BatchSize = 5000;
                    bulkCopy.DestinationTableName = "UTT";
                    bulkCopy.ColumnMappings.Add("UDate", "UDate");
                    bulkCopy.ColumnMappings.Add("UType", "UType");
                    bulkCopy.ColumnMappings.Add("UGroup", "UGroup");
                    bulkCopy.ColumnMappings.Add("UMessage", "UMessage");
                    bulkCopy.WriteToServer(table);
                }
            }

            Console.WriteLine("All Completed");
        }


        private void Read(string path,DataTable table)
        {
            var group = 0;
            using (var fs = new FileStream(path,FileMode.Open))
            {
                using (var reader = new StreamReader(fs,Encoding.UTF8))
                {
                    while (!reader.EndOfStream) 
                    {
                        var value = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(value)) continue;
                        if (value.Contains("AGV状态加载UTT下料任务"))
                        {
                            group++;
                            this.ValueAnalysis(value, table, group);
                        }
                        else if (value.Contains("AGV状态UTT下料任务后数据处理"))
                        {
                            this.ValueAnalysis(value, table, group);
                        }
                    }
                }
            }
        }

        private void ValueAnalysis(string value,DataTable table,int group)
        {
            var array = value.Split('|');
            var row = table.NewRow();
            var type = value.Contains("AGV状态加载UTT下料任务") ? 1 : 2;
            row["UDate"] = DateTime.ParseExact(array[0], "yyyyMMdd HHmmss:fff", CultureInfo.InvariantCulture, DateTimeStyles.None);
            row["UType"] = type;
            row["UGroup"] = group;
            row["UMessage"] = array[2];
            table.Rows.Add(row);
        }

    }
}

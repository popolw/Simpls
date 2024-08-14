using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Simpls
{

    public class APIResult
    {
        /// <summary>
        /// Success，Error
        /// </summary>
        public string Statue = "";
        /// <summary>
        /// 
        /// </summary>
        public string Msg = "";
    }

    public class StringNumToEnumConverter : JsonConverter<ARCCALLTYPE>
    {
        public override ARCCALLTYPE Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return (ARCCALLTYPE)Convert.ToInt32(value);
        }

        public override void Write(Utf8JsonWriter writer, ARCCALLTYPE value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(Convert.ToInt32(value).ToString());
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class CodeAttribute : Attribute
    {
        public string Code { get; private set; }

        public CodeAttribute(string code)
        {
            this.Code = code;
        }
    }

    /// <summary>
    /// ARC呼叫IAR 
    /// </summary>
    [System.ComponentModel.Description("ARC呼叫IAR")]
    public enum ARCCALLTYPE : short
    {
        /// <summary>
        /// 无
        /// </summary>
        [Code("NONE")]
        [System.ComponentModel.Description("无")]
        NONE = 0x0,
        /// <summary>
        /// 7寸A位置空盘
        /// </summary>
        [Code("EMPTY_7A")]
        [System.ComponentModel.Description("7寸A位置空盘")]
        EMPTY_7A = 0x7A,
        /// <summary>
        /// 7寸B位置空盘
        /// </summary>
        [Code("EMPTY_7B")]
        [System.ComponentModel.Description("7寸B位置空盘")]
        EMPTY_7B = 0x7B,
        /// <summary>
        /// 13寸A位置空盘
        /// </summary>
        [Code("EMPTY_13A")]
        [System.ComponentModel.Description("13寸A位置空盘")]
        EMPTY_13A = 0x13A,
        /// <summary>
        /// 7寸7C位置满盘
        /// </summary>
        [Code("FULL_7C")]
        [System.ComponentModel.Description("7寸7C位置满盘")]
        FULL_7C = 0x7C,
        /// <summary>
        /// 7寸D位置满盘
        /// </summary>
        [Code("FULL_7D")]
        [System.ComponentModel.Description("7寸D位置满盘")]
        FULL_7D = 0x7D,
        /// <summary>
        /// 13寸C位置满盘
        /// </summary>
        [Code("FULL_13C")]
        [System.ComponentModel.Description("13寸C位置满盘")]
        FULL_13C = 0x13C
    }
    public class ARC_MDS_Job
    {
        /// <summary>
        /// JOBID
        /// </summary>
        public string JobID { get; set; }

        /// <summary>
        /// 目标地址
        /// </summary>
        public string DestLoc { get; set; }

        /// <summary>
        /// 来源地址 
        /// </summary>
        public string SrcLoc { get; set; }

        /// <summary>
        /// 呼叫类型
        /// </summary>
        [JsonConverter(typeof(StringNumToEnumConverter))]
        public ARCCALLTYPE Type { get; set; }
    }

    public class JsonTest
    {
        [Test]
        public void Run()
        {
            var json = "{\"Type\": \"122\", \"DestLoc\": \"TR2054ARC\",\"SrcLoc\":\"TR7-1-6\", \"JobID\":\"xxxxx\"}";
            var obj = System.Text.Json.JsonSerializer.Deserialize<ARC_MDS_Job>(json);

            var x = JsonSerializer.Deserialize<ARCCALLTYPE>("122");
        }

        [Test]
        public void MSCPlan()
        {
            var path = @"E:\sts-oven\OvenClient\Config\MCSPlanConfig.txt";
            var json = File.ReadAllText(path, Encoding.UTF8);
            var doc = JsonDocument.Parse(json);
            var array = doc.RootElement.EnumerateArray();
            var sb = new StringBuilder();
            sb.AppendLine("INSERT INTO [MCSPlan]([ProcessPlan],[ProcessStep],[EQPName],[AliasName])");
            foreach (JsonElement ele in array)
            {
                var plan = ele.GetProperty("ProcessPlan").GetString();
                var step = ele.GetProperty("ProcessStep").GetString();
                var eqp = ele.GetProperty("EQPName").GetString();
                var alias = ele.GetProperty("AliasName").GetString();
                sb.AppendLine($"SELECT '{plan}','{step}','{eqp}','{alias}' UNION ALL");
            }
            var sql= sb.ToString();
        }


        public static DataTable ReadCsvToDataTable(string path)
        {
            DataTable dt = new DataTable();

            using (TextFieldParser csvReader = new TextFieldParser(path))
            {
                csvReader.SetDelimiters(new string[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;

                // 读取标题行
                string[] colFields = csvReader.ReadFields();
                foreach (string columnTitle in colFields)
                {
                    dt.Columns.Add(columnTitle);

                }

                // 读取每行记录
                while (!csvReader.EndOfData)
                {
                    string[] fieldData = csvReader.ReadFields();
                    // 跳过空行
                    if (fieldData.Length == 0 || string.IsNullOrWhiteSpace(fieldData[0])) continue;

                    DataRow row = dt.NewRow();
                    row.ItemArray = fieldData;
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }


        [Test]
        public void AlarmConfig()
        {
            var table = ReadCsvToDataTable(@"D:\OvenAlarmTable.txt");
            var sb = new StringBuilder();
            sb.AppendLine("INSERT INTO [AlarmConfig]([Part],[FaultCode],[PLCFaultCode],[Enable],[HmiVaribale],[Level],[Content],[Measure])");
            foreach(DataRow row in table.Rows)
            {
                var enable = row["Enable"].ToString()=="y"?1:0;
                sb.AppendLine($"SELECT '{row["Part"]}','{row["FaultCode"]}','{row["PLCFaultCode"]}',{enable},'{row["HmiVaribale"]}',N'{row["Level"]}',N'{row["Content"]}',N'{row["Measure"]}' UNION ALL");
            }
            var sql = sb.ToString();
        }


        #region RobotPos
        public class RobotPosDefine
        {
            public string EquipmentID { get; set; }

            public string EquipmentName { get; set; }

            public int EquipmentType { get; set; }

            public int EquipmenPort { get; set; }

            public string EntryPosName { get; set; }

            public string DockPosName { get; set; }

            public string LeavePosName { get; set; }

            public int PosColumn { get; set; }

            public int PosRow { get; set; }

            public string EquipmentCommunicationIP { get; set; }

            public int EquipmentCommunicationPort { get; set; }

            public string EquipmentCommunicationVars { get; set; }

            public override string ToString()
            {
                return $"SELECT N'{EquipmentID}',N'{EquipmentName}',{EquipmentType},{EquipmenPort},N'{EntryPosName}',N'{DockPosName}',N'{LeavePosName}',{PosColumn},{PosRow},N'{EquipmentCommunicationIP}',{EquipmentCommunicationPort},N'{EquipmentCommunicationVars}' UNION ALL";
            }
        }
        #endregion

        [Test]
        public void RobotPos()
        {

           var path = @"E:\iararchitecture\ICS\\iar-ics-server\IAR-ICS-WebApi\Config\RobotPos.json";
           var json = File.ReadAllText(path,Encoding.UTF8);
            var sb = new StringBuilder();
            var properties = typeof(RobotPosDefine).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
            var list = JsonSerializer.Deserialize<List<RobotPosDefine>>(json);
            sb.AppendLine("INSERT INTO [RobotPos]([EquipmentID],[EquipmentName],[EquipmentType],[EquipmenPort],[EntryPosName],[DockPosName],[LeavePosName],[PosColumn],[PosRow],[EquipmentCommunicationIP],[EquipmentCommunicationPort],[EquipmentCommunicationVars])");
            foreach (var item in list)
            {
                sb.AppendLine(item.ToString());
            }
            var sql = sb.ToString();

        }

        [Test]
        public void DateTimeConvertTest()
        {
           var timestring= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var ndate = Convert.ToDateTime(timestring);
        }

        public static float GetSingle(ushort highOrderValue, ushort lowOrderValue)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(lowOrderValue).Concat(BitConverter.GetBytes(highOrderValue)).ToArray(), 0);
        }

        private static T[][] GroupArray<T>(T[] array, int groupSize)
        {
            if (groupSize <= 0)
                throw new ArgumentException("Group size must be a positive integer.");

            var groups = array.Select((item, index) => new { item, groupIndex = index / groupSize })
                               .GroupBy(x => x.groupIndex)
                               .Select(g => g.Select(x => x.item).ToArray())
                               .ToArray();
            return groups;
        }

        [Test]
        public void ByteTest()
        {
            var size = sizeof(short);
            byte[] buffer = new byte[] { 65, 32 };


            ushort ushortValue = BitConverter.ToUInt16(buffer.Reverse().ToArray(), 0);
            float floatValue = (float)ushortValue / 100.0f;
        }

    }
}

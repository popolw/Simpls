using NUnit.Framework;
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
    }
}

using NUnit.Framework;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Simpls
{
    public class ModelBuilderTest
    {
        private string BuildIO(string path)
        {
            var sb = new StringBuilder();
            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var reader = new StreamReader(fs, Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        var value = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(value)) continue;
                        var array = value.Split("\t");
                        sb.AppendLine($"private bool _{array[0]};");
                        sb.AppendLine($"public bool {array[0]}");
                        sb.AppendLine("{");
                        sb.AppendLine($"get=>_{array[0]};");
                        sb.AppendLine($"set=> this.OnPropertyChanged(ref _{array[0]}, value);");
                        sb.AppendLine("}");
                        sb.AppendLine(string.Empty);
                    }
                }
            }
            return sb.ToString();
        }


        [Test]
        public void Build监控输入信号()
        {
           var code= BuildIO(@"D:\lxtk\监控输入信号.txt");
        }

        [Test]
        public void Build监控输出信号()
        {
            var code = BuildIO(@"D:\lxtk\监控输出信号.txt");
        }

        private string TrimLeftZero(string value)
        {
            return value[0]=='0' ? value.Substring(1) : value;
        }

        [Test]
        public void Build手动按钮()
        {
            var path = @"D:\lxtk\监控输入信号.txt";
            var sb = new StringBuilder();
            using (var fs = new FileStream(path,FileMode.Open))
            {
                using (var reader =new StreamReader(fs,Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        var value = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(value)) continue;
                        var array = value.Split("\t");
                        var a2= array[2].Split(".");
                        sb.AppendLine($"private bool _{array[0]};");
                        sb.AppendLine($"[Address({a2[0].TrimStart(' ').TrimStart('W')})]");
                        sb.AppendLine($"[Offset({TrimLeftZero(a2[1])})]");
                        sb.AppendLine($"public bool {array[0]}");
                        sb.AppendLine("{");
                        sb.AppendLine($"get=>_{array[0]};");
                        sb.AppendLine($"set=> this.OnPropertyChanged(ref _{array[0]}, value);");
                        sb.AppendLine("}");
                        sb.AppendLine(string.Empty);
                    }
                }
            }
            var codeSource = sb.ToString();
        }


        private string BuildProperty(string path,string type)
        {
            var sb = new StringBuilder();
            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var reader = new StreamReader(fs, Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        var value = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(value)) continue;
                        var array = value.Split("\t");
                        var bit = array[2].Trim().TrimStart('W').TrimStart('D');
                        sb.AppendLine($"private {type} _{array[0]};");
                        sb.AppendLine($"[Address({bit})]");
                        sb.AppendLine($"public {type} {array[0]}");
                        sb.AppendLine("{");
                        sb.AppendLine($"get=>_{array[0]};");
                        sb.AppendLine($"set=> this.OnPropertyChanged(ref _{array[0]}, value);");
                        sb.AppendLine("}");
                        sb.AppendLine(string.Empty);
                    }
                }
            }
            return sb.ToString();
        }

        [Test]
        public void Build实际位置实际速度()
        {
            var code = BuildProperty(@"D:\lxtk\监控输入信号.txt", "float");
        }

        [Test]
        public void Build流程步骤()
        {
            var code = BuildProperty(@"D:\lxtk\流程步骤.txt", "byte");
        }

        [Test]
        public void Build速度设置()
        {
            var code = BuildProperty(@"D:\lxtk\速度设置.txt", "float");
        }

        [Test]
        public void Build位置设置()
        {
            while (true) 
            {
                UdpClient client = new UdpClient();
                client.Connect("192.168.100.5", 9600);
                client.Send(new byte[] { 0, 1 });
                IPEndPoint? ipe=null;
                var buffer= client.Receive(ref ipe);
                //读取到数据后处理
                //。。。。。
                client.Close();
                Thread.Sleep(1000);
            }

            var code = BuildProperty(@"D:\lxtk\位置设置.txt", "float");
        }
    }
}

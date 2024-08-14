using NUnit.Framework;

namespace Simpls
{
    public class RegexTest
    {
        [Test]
        public void IsFloat()
        {
            var value = "9.12321234567980876666";
            //float a = 999.12321234567980876666f;
            var cv = Convert.ToSingle(value);
           var result = RegexHelper.IsDecimal(value);
        }
    }
}

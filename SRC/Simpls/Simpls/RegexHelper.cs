using System.Text.RegularExpressions;

internal class RegexHelper
{
    public static bool RegMatch(string input, string pattern)
    {
        return input == null ? false : Regex.IsMatch(input, pattern);
    }

    #region regdefine       
    /// <summary>
    /// 日期验证
    /// </summary>
    public static readonly string DateReg = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]
                                 |[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|
                                 1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0
                                 2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468]
                                 [048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$";


    /// <summary>
    /// 时间验证
    /// </summary>
    public static readonly string TimeReg = @"^((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$";


    /// <summary>
    /// 日期加时间验证
    /// </summary>
    public static readonly string DataTimeReg = @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?
                                 [1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?
                                 [13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]
                                 |[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-
                                 9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[
                                 2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23
                                 |[0-1]?\d):[0-5]?\d:[0-5]?\d)$";

    public static readonly string EmailReg = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

    #endregion
    /// <summary>
    /// 是否为日期型字符串
    /// 正确格式:2012-12-25
    /// </summary>
    /// <param name="input">要验证的字符串</param>
    /// <returns>是否验证成功</returns>
    public static bool IsDate(string input)
    {
        return RegMatch(input, DateReg);
    }

    /// <summary>
    ///  是否为时间型字符串 
    /// 正确格式:(15:00:00)          
    /// </summary>          
    ///<param name="input">要验证的字符串</param>        
    /// <returns>是否验证成功</returns>          
    public static bool IsTime(string input)
    {
        return RegMatch(input, TimeReg);
    }

    /// <summary>          
    /// 是否为日期+时间型字符串
    ///正确格式: (2012-12-25 15:00:00)         
    ///</summary>          
    ///<param name="input">要验证的字符串</param> 
    /// <returns>是否验证成功</returns>
    public static bool IsDateTime(string input)
    {
        return RegMatch(input, DataTimeReg);
    }

    /// <summary>
    /// 是否是正整数
    /// 正确格式：大于0的任何自然数
    /// </summary>
    /// <param name="input">要验证的字符串</param>
    /// <returns>是否验证成功</returns>
    public static bool IsPositiveInteger(string input)
    {
        return RegMatch(input, @"^[1-9]+\d*$");
    }

    /// <summary>
    /// 是否是整数(包括正整数和负整数)
    /// 正确格式：大于等于0或者小于等于0的任何整数
    /// </summary>
    /// <param name="input">要验证的字符串</param>
    /// <returns>是否验证成功</returns>
    public static bool IsInteger(string input)
    {
        return RegMatch(input, @"\d+");
    }


    /// <summary>
    /// 是否是金钱格式(这里支持负数)
    /// 正确格式如：100、100.01、-100、-100.01
    /// </summary>
    /// <param name="input">要验证的字符串</param>
    /// <returns>是否验证成功 true成功false失败</returns>
    public static bool IsDecimal(string input)
    {
        return RegMatch(input, @"^(-{0,1}([1-9]\d*)$|^[0]$|^(-{0,1}(([1-9]\d*)|(0))\.\d+))$");
    }

    /// <summary>
    /// 是否是手机号码
    /// 正确格式：前两位是:13,14,15,16,17,18的11位数的字符串
    /// </summary>
    /// <param name="input">要验证的字符串</param>
    /// <returns>是否验证成功</returns>
    public static bool IsMobilePhone(string input)
    {
        return RegMatch(input, @"^(13|14|15|16|17|18|19)\d{9}$");
    }

    public static bool IsTelephone(string input)
    {
        return Regex.IsMatch(input, @"^(\d{3,4}-)?\d{6,8}$");
    }

    /// <summary>
    /// 是否是英文字符
    /// 正确格式：由全英文字母组的字符如：ENGLISH
    /// </summary>
    /// <param name="input">要验证的字符串</param>
    /// <returns>是否验证成功</returns>
    public static bool IsEnglish(string input)
    {
        return RegMatch(input, @"^[a-zA-Z]+$");
    }

    /// <summary>
    /// 是否是身份证号码
    /// </summary>
    /// <param name="input">要验证的字符串</param>
    /// <returns>是否验证成功</returns>
    public static bool IsIdCard(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        Regex reg = new Regex("(^\\d{15}$)|(^\\d{18}$)|(^\\d{17}(\\d|X|x)$)");//身份证正则表达式 

        if (reg.IsMatch(input))
        {
            if (input.Length == 18)//18位
            {
                string id_17 = input.Substring(0, 17); //前17位

                int[] num = new int[17];//声明数组，存放每一位数字 

                for (int i = 0; i < 17; i++)
                {
                    num[i] = Convert.ToInt32(input.Substring(i, 1));//获取每位上的数字，从左往右。                        
                }
                //出生日期验证
                reg = new Regex("^((((1[6-9]|[2-9]\\d)\\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\\d|3[01]))|(((1[6-9]|[2-9]\\d)\\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\\d|30))|(((1[6-9]|[2-9]\\d)\\d{2})-0?2-(0?[1-9]|1\\d|2[0-8]))|(((1[6-9]|[2-9]\\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
                /*声明变量，并获取身份证号码中的“出生日期”*/
                string birth = "";
                for (int i = 6, j = 0; i < 14; i++, j++)
                {
                    birth += Convert.ToString(num[i]);
                    if (j == 3 || j == 5)//添加“-”使日期格式为：yyyy-MM-dd
                    {
                        birth += "-";
                    }
                }
                //验证出生日期
                if (!reg.IsMatch(birth))
                    return false;
                //从左往右每一位的权：7－9－10－5－8－4－2－1－6－3－7－9－10－5－8－4－2
                int[] quan = new int[17] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
                int sum = 0;
                for (int i = 0; i < 17; i++)
                {
                    sum += num[i] * quan[i];
                }
                //余数（sum % 11）：  0－1－2－3－4－5－6－7－8－9－10（可视为索引）
                //对应值：1－0－X－9－8－7－6－5－4－3－2
                string[] duiyingz = new string[11] { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };

                if (!(id_17 + duiyingz[sum % 11]).Equals(input.ToUpper()))
                    return false;
            }
            else//十五位身份证
            {
                input = input.Insert(6, "19");//给15位身份证升级，补全年份。
                int[] num = new int[17];//声明数组，存放每一位数字 

                for (int i = 0; i < 17; i++)
                {
                    num[i] = Convert.ToInt32(input.Substring(i, 1));//获取每位上的数字，从左往右。                        
                }
                //出生日期验证
                reg = new Regex("^((((1[6-9]|[2-9]\\d)\\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\\d|3[01]))|(((1[6-9]|[2-9]\\d)\\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\\d|30))|(((1[6-9]|[2-9]\\d)\\d{2})-0?2-(0?[1-9]|1\\d|2[0-8]))|(((1[6-9]|[2-9]\\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
                /*声明变量，并获取身份证号码中的“出生日期”*/
                string birth = "";
                for (int i = 6, j = 0; i < 14; i++, j++)
                {
                    birth += Convert.ToString(num[i]);
                    if (j == 3 || j == 5)//添加“-”使日期格式为：yyyy-MM-dd
                    {
                        birth += "-";
                    }
                }
                //验证出生日期
                if (!reg.IsMatch(birth))
                    return false;
            }
        }
        else//不符合身份证规则(非15或18位)
            return false;
        return true;
    }


    /// <summary>
    /// 是否是密码(32未的密码)
    /// </summary>
    /// <param name="input">要验证的字符串</param>
    /// <returns>是否验证成功</returns>
    public static bool IsPassword(string input)
    {
        return RegMatch(input, @"^[a-zA-Z0-9]{32}");
    }

    /// <summary>
    /// 验证邮件地址是否正确
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool IsEmail(string email)
    {
        return RegMatch(email, EmailReg);
    }

    /// <summary>
    /// 是否只包含字母和数字
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsEnglishAndNum(string input)
    {
        return RegMatch(input, @"^[a-zA-Z0-9]");
    }

    /// <summary>
    /// 匹配数值类型，注：整数和浮点数(1-2位小数)
    /// </summary>
    /// <param name="source">待匹配字符串</param>
    /// <returns>匹配结果:true；反之:false</returns>
    public static bool IsDecimal2(string source)
    {
        if (string.IsNullOrEmpty(source))
            return false;

        if (new Regex("^[0-9]+[.]?[0-9]{1,2}$").IsMatch(source) || new Regex("^[0-9]+$").IsMatch(source))
            return true;

        return false;
    }

}
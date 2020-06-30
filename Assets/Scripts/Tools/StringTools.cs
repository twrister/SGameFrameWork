using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace SthGame
{
    public class StringTools
    {

        static readonly Regex regexAccount = new Regex("[_a-zA-Z0-9]{3,}");
        static readonly Regex regexPassword = new Regex("[_a-zA-Z0-9]{3,}");
        // 用于判断账号密码是否输入有效
        public static bool IsValidAccountInput(string input)
        {
            return regexAccount.IsMatch(input);
        }

        public static bool IsValidPasswordInput(string input)
        {
            return regexPassword.IsMatch(input);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Calculator
{
    class Parser
    {
        private string Str;
        private int CurByte;
        private string Token;

        public Parser(string S)
        {
            CurByte = 0;
            int i;
            for (i = S.Length - 1; i >= 0; --i)
                if (S[i] != ' ') break;
            Str = S.Substring(0, i+1);
        }

        public bool NotEnd()
        {
            return (CurByte < Str.Length);
        }

        public string TokenStr()
        {
            Token = "";
            while ((Str[CurByte] < 33) & (CurByte < Str.Length))
                CurByte++;

            int i = 0;
            while ((CurByte + i) < Str.Length)
            {
                if (!Regex.IsMatch(Str[CurByte + i].ToString(), "[0-9a-zA-Zа-яА-Я_]"))
                    break;
                i++;
            }

            if (i == 0) i++;
            Token = Str.Substring(CurByte, i);
            return Token;
        }

        public void NextToken()
        {
            if (Token.Length == 0)
                CurByte++;
            else
                CurByte += Token.Length;
        }
    }
}

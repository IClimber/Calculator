using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Calculator
{
    class Calculator
    {
        private struct TVars
        {
            public string Name;
            public List<String> ValStr;
            public List<String> ValTStr;
            public List<String> StepStr;
            public decimal Value;
            public decimal ValueTo;
            public decimal Step;
        }

        List<TVars> Vars = new List<TVars>();
        List<string> Formula = new List<string>();
        public struct RPoint
        {
            public decimal X;
            public decimal Y;
        }

        public RPoint[] GraphPoints;

        private decimal Factorial(decimal ch)
        {
            if (ch < 0)
                return decimal.MaxValue;
            
            decimal R = 1;
            if (ch == 0)
                return 0;
            
            for (int i = 1; i <= ch; i++)
                R *= i;
            return R;
        }

        private bool Cmp(string S1, string S2)
        {
            return String.Equals(S1, S2, StringComparison.CurrentCultureIgnoreCase);
        }

        private bool InSet(string Str, string[] Par)
        {
            foreach (string S in Par)
                if (Cmp(Str, S))
                    return true;

            return false;
        }

        private byte GetPriority(string c)
        {
            if (InSet(c, new string[] {"(", ")"})) return 1;
            if (InSet(c, new string[] {"+", "-"})) return 2;
            if (InSet(c, new string[] {"*", "/", "%", "mod", "div", "and", "xor", "or"})) return 3;
            if (InSet(c, new string[] {"^", "!"})) return 4;
            if (InSet(c, new string[] {"cos", "sin", "tan", "ctg", "arccos", "arcsin", "arctan", "arcctg",
                "sqr", "sqrt", "ln", "lg", "log2", "exp", "CTF", "CTK", "FTC", "FTK", "KTC", "KTF",
                "not", "abs", "ceil", "round", "trunc", "frac", "random"})) return 5;
            
            return 0;
        }

        private byte Digit(string S)
        {
            if (S.Length < 2) return 0;

            if (char.ToLower(S[S.Length - 1]) == 'h')
            {
                for (int i = 0; i < S.Length - 1; i++)
                    if (!Regex.IsMatch(S[i].ToString(), "[0-9a-fA-F]"))
                        return 0;
                return 1;
            }

            if (char.ToLower(S[S.Length - 1]) == 'o')
            {
                for (int i = 0; i < S.Length - 1; i++)
                    if (!Regex.IsMatch(S[i].ToString(), "[0-7]"))
                        return 0;
                return 2;
            }

            if (char.ToLower(S[S.Length - 1]) == 'b')
            {
                for (int i = 0; i < S.Length - 1; i++)
                    if (!Regex.IsMatch(S[i].ToString(), "[0-1]"))
                        return 0;
                return 3;
            }

            return 0;            
        }

        public void ParseString(string S, ref List<string> _Stack)
        {
            List<string> Temp = new List<string>();
            Parser Pars = new Parser(S);
            char DecimalSeparator = Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            bool flag = false;
            while (Pars.NotEnd())
            {
                string TStr = Pars.TokenStr();
                switch (Digit(TStr))
                {
                    case 1:
                        TStr = Convert.ToString(Convert.ToInt64(TStr.Substring(0, TStr.Length - 1), 16), 10);
                        break;
                    case 2:
                        TStr = Convert.ToString(Convert.ToInt64(TStr.Substring(0, TStr.Length - 1), 8), 10);                        
                        break;
                    case 3:
                        TStr = Convert.ToString(Convert.ToInt64(TStr.Substring(0, TStr.Length - 1), 2), 10);
                        break;
                }

                byte PR = GetPriority(TStr);
                if ((Regex.IsMatch(TStr[0].ToString(), "[0-9]") || (PR == 0)) && (TStr[0] != DecimalSeparator))
                    if (flag && (_Stack.Count > 0))
                    {
                        _Stack[_Stack.Count - 1] = _Stack[_Stack.Count - 1] + TStr;
                        flag = false;
                    }
                    else
                        _Stack.Add(TStr);                        

                if (TStr[0] == DecimalSeparator)
                {
                    _Stack[_Stack.Count-1] = _Stack[_Stack.Count-1] + TStr;
                    flag = true;
                }

                if (PR > 1)
                {
                    if (Temp.Count == 0)
                        Temp.Add(TStr);
                    else                    
                        if (PR > GetPriority(Temp[Temp.Count - 1]))
                            Temp.Add(TStr);
                        else
                        {
                            while (true)
                            {
                                _Stack.Add(Temp[Temp.Count - 1]);
                                Temp.RemoveAt(Temp.Count - 1);
                                if (Temp.Count == 0)
                                    break;
                                if (PR > GetPriority(Temp[Temp.Count - 1]))
                                    break;
                            }

                            Temp.Add(TStr);
                        }                    
                }

                if (TStr[0] == '(')
                    Temp.Add(TStr);

                if (TStr[0] == ')')
                    while (true)
                    {
                        if (Temp.Count == 0)
                            break;
                        if (Temp[Temp.Count - 1] == "(")
                        {
                            Temp.RemoveAt(Temp.Count - 1);
                            break;
                        }

                        _Stack.Add(Temp[Temp.Count - 1]);
                        Temp.RemoveAt(Temp.Count - 1);
                    }
                Pars.NextToken();
            }

            while (Temp.Count > 0)
            {
                _Stack.Add(Temp[Temp.Count - 1]);
                Temp.RemoveAt(Temp.Count - 1);
            }
        }

        private decimal Calc(ref List<string> _Stack)
        {
            List<string> Temp = new List<string>();
            decimal a1, a2;
            Random Rand = new Random();

            for (int i = 0; i < _Stack.Count; i++)
                if (Regex.IsMatch(_Stack[i][0].ToString(), "[0-9]"))
                    Temp.Add(_Stack[i]);
                else
                {
                    a2 = Convert.ToDecimal(Temp[Temp.Count - 1]);
                    Temp.RemoveAt(Temp.Count - 1);

                    a1 = 0;
                    if (InSet(_Stack[i], new string[] { "+", "-", "/", "*", "^", "%", "mod", "div", "and", "or", "xor" }))
                        if (Temp.Count > 0)
                        {
                            a1 = Convert.ToDecimal(Temp[Temp.Count - 1]);
                            Temp.RemoveAt(Temp.Count - 1);
                        }

                    switch (_Stack[i].ToLower())
                    {
                        case "+":   Temp.Add((a1 + a2).ToString());
                            break;
                        case "-": Temp.Add((a1 - a2).ToString());
                            break;
                        case "/": Temp.Add((a1 / a2).ToString());
                            break;
                        case "*": Temp.Add((a1 * a2).ToString());
                            break;
                        case "^": Temp.Add(((decimal)Math.Pow((double)a1, (double)a2)).ToString());
                            break;
                        case "!": Temp.Add(Factorial(a2).ToString());
                            break;
                        case "%": Temp.Add((a1 % a2).ToString());
                            break;
                        case "mod": Temp.Add((a1 % a2).ToString());
                            break;
                        case "div": Temp.Add(Math.Truncate(a1 / a2).ToString());
                            break;
                        case "and": Temp.Add(((Int64)a1 & (Int64)a2).ToString());
                            break;
                        case "or": Temp.Add(((Int64)a1 | (Int64)a2).ToString());
                            break;
                        case "xor": Temp.Add(((Int64)a1 ^ (Int64)a2).ToString());
                            break;
                        case "sin": Temp.Add(((decimal)Math.Sin((double)a2 / (180 / Math.PI))).ToString());
                            break;
                        case "cos": Temp.Add(((decimal)Math.Cos((double)a2 / (180 / Math.PI))).ToString());
                            break;
                        case "tan": Temp.Add(((decimal)Math.Tan((double)a2 / (180 / Math.PI))).ToString());
                            break;
                        case "ctg": Temp.Add((1/(decimal)Math.Tan((double)a2 / (180 / Math.PI))).ToString());
                            break;
                        case "arcsin": Temp.Add(((decimal)(Math.Asin((double)a2) * (180 / Math.PI))).ToString());
                            break;
                        case "arccos": Temp.Add(((decimal)(Math.Acos((double)a2) * (180 / Math.PI))).ToString());
                            break;
                        case "arctan": Temp.Add(((decimal)(Math.Atan((double)a2) * (180 / Math.PI))).ToString());
                            break;
                        case "arcctg": Temp.Add(((decimal)((Math.PI/2 - Math.Atan((double)a2)) * (180 / Math.PI))).ToString());
                            break;
                        case "ln": Temp.Add(((decimal)Math.Log((double)a2)).ToString());
                            break;
                        case "lg": Temp.Add(((decimal)Math.Log10((double)a2)).ToString());
                            break;
                        case "log2": Temp.Add(((decimal)Math.Log((double)a2, 2)).ToString());
                            break;
                        case "sqr": Temp.Add(((decimal)Math.Pow((double)a2, 2)).ToString());
                            break;
                        case "sqrt": Temp.Add(((decimal)Math.Sqrt((double)a2)).ToString());
                            break;
                        case "random": Temp.Add(Rand.Next((int)a2).ToString());
                            break;
                        case "round": Temp.Add(Math.Round(a2).ToString());
                            break;
                        case "ceil": Temp.Add(Math.Ceiling(a2).ToString());
                            break;
                        case "trunc": Temp.Add(Math.Truncate(a2).ToString());
                            break;
                        case "floor": Temp.Add(Math.Floor(a2).ToString());
                            break;
                    }                    
                }
            return decimal.Parse(Temp[0]);
        }

        private string NormalizeString(string S)
        {
            return S.Replace("pi", Math.PI.ToString()).Replace("  ", " ").Replace("(-", "(0-").Replace("--", "+").Replace("++", "+").Replace("-+", "-").Replace("+-", "-").Replace("//", "/").Replace("**", "*");
        }

        private void VarsReplace(ref List<string> Formul, int ToPos)
        {
            for (int i = 0; i < Formul.Count; i++)
                for (int j = ToPos; j >= 0; j--)
                    if (Formul[i] == Vars[j].Name)
                        Formul[i] = Vars[j].Value.ToString();
        }
        
        private void AddVars(string S)
        {
            int L = Vars.Count;
            TVars Item = new TVars();
            Item.ValStr = new List<string>();
            Item.ValTStr = new List<string>();
            Item.StepStr = new List<string>();
            Item.Value = 0;
            Item.ValueTo = 0;
            Item.Step = 0;
            
            int A = S.IndexOf('=');
            string Str = S.Substring(A + 1, S.Length - A - 1);
            List<string> Temp = new List<string>();

            Item.Name = S.Substring(0, A);

            string F = "0";
            string T = "0";
            string St = "0";

            int B = Str.IndexOf("..", Math.Min(A, Str.Length));            
            int C = Str.IndexOf(',', Math.Max(B - 1, 0));
            if (B >= 0)
                St = "1";

            if (C >= 0)
            {                
                St = Str.Substring(C + 1, Str.Length - C - 1);
                Str = Str.Substring(0, C - 1);                
            }
            
            if (B >= 0)
            {
                T = Str.Substring(B + 2, Str.Length - B - 2);
                F = Str.Substring(0, B);
            }
            else
            {
                F = Str;
                T = Str;
            }
            
            ParseString(F, ref Item.ValStr);            
            Temp.Clear();
            Temp.AddRange(Item.ValStr);            
            Vars.Add(Item);
            VarsReplace(ref Temp, L - 1);
            TVars TV = Vars[L];
            TV.ValStr = Item.ValStr;
            TV.Value = Calc(ref Temp);
            Vars[L] = TV;

            if (F != T)
            {
                ParseString(T, ref Item.ValTStr);
                Temp.Clear();
                Temp.AddRange(Item.ValTStr);
                VarsReplace(ref Temp, L - 1);
                TV = Vars[L];
                TV.ValTStr = Item.ValTStr;
                TV.ValueTo = Calc(ref Temp);
                Vars[L] = TV;
            }
            else
            {
                TV = Vars[L];
                TV.ValueTo = TV.Value;
                Vars[L] = TV;
            }

            if (St != "0")
            {
                ParseString(St, ref Item.StepStr);
                Temp.Clear();
                Temp.AddRange(Item.StepStr);
                VarsReplace(ref Temp, L - 1);
                TV = Vars[L];
                TV.StepStr = Item.StepStr;
                TV.Step = Calc(ref Temp);
                Vars[L] = TV;
            }
        }

        private void UpdateVars(int From)
        {
            List<string> Temp = new List<string>();

            for (int i = From; i < Vars.Count; i++)
            {
                Temp.Clear();
                Temp.AddRange(Vars[i].ValStr);
                VarsReplace(ref Temp, i - 1);
                var TV = Vars[i];
                TV.Value = Calc(ref Temp);
                Vars[i] = TV;
            }
        }

        private void ParseCalcText(List<string> Str)
        {
            Formula.Clear();
            Vars.Clear();
            string F = "";
            for (int i = 0; i < Str.Count; i++)
            {
                string S = Str[i];
                int P = S.IndexOf("//");
                if (P >= 0)
                    S = S.Substring(0, P);
                P = S.IndexOf('=');
                if (P >= 0)                    
                    AddVars(NormalizeString(S));
                else
                    F = F + S;
            }

            F = NormalizeString(F);
            ParseString(F, ref Formula);            
        }

        private int GetMinMax()
        {
            for (int i = 0; i < Vars.Count; i++)
                if (Vars[i].ValueTo > Vars[i].Value)
                    return i;

            return -1;
        }

        public decimal Calculate(string CalcFormula)
        {
            Array.Resize(ref GraphPoints, 0);

            if (CalcFormula == "")
                return 0;

            List<string> CalcList = new List<string>();
            CalcList = CalcFormula.Split('\n').ToList();            
            ParseCalcText(CalcList);            

            if (Formula.Count == 0)
                return 0;

            List<string> Temp = new List<string>(Formula);            
            VarsReplace(ref Temp, Vars.Count-1);
            decimal Res = Calc(ref Temp);

            int V = GetMinMax();
            if (V >= 0)
            {
                double P = (double)(100 / Math.Ceiling((Vars[V].ValueTo - Vars[V].Value) / Vars[V].Step));
                Array.Resize(ref GraphPoints, (int)Math.Ceiling((decimal)(Vars[V].ValueTo - Vars[V].Value) / Vars[V].Step) + 1);
                int i = 0;
                while (Vars[V].Value <= Vars[V].ValueTo)
                {
                    Temp.Clear();
                    Temp.AddRange(Formula);
                    GraphPoints[i].X = Vars[V].Value;
                    VarsReplace(ref Temp, Vars.Count - 1);
                    GraphPoints[i].Y = Calc(ref Temp);
                    var TV = Vars[V];
                    TV.Value += TV.Step;
                    Vars[V] = TV;
                    UpdateVars(V+1);
                    i++;
                }
                if (i != GraphPoints.Length)
                    Array.Resize(ref GraphPoints, i);
            }

            return Res;
        }
    }
}
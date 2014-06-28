using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Common
{ 
    public class SerialNumber
    {
        public enum Plant
        {
            Tolstraat = 0,
            ConnectLokeren = 1
        };
        
        const int MODEL_OFFSET = 36*36; //"?00";
        const int MODEL_SMARTSCOPE = 10; //"A"

        const int GENERATION_OFFSET = 36; //"?0";
        public enum Model
        {
            SmartScope_Proto_3_3 = (MODEL_SMARTSCOPE * MODEL_OFFSET) + (0 * GENERATION_OFFSET) + 0,
            SmartScope_3_4_0     = (MODEL_SMARTSCOPE * MODEL_OFFSET) + (1 * GENERATION_OFFSET) + 0,
            SmartScope_3_5_0     = (MODEL_SMARTSCOPE * MODEL_OFFSET) + (1 * GENERATION_OFFSET) + 1,
        }

        public static void LogModelsAndPlants()
        {
            string format = "{0,10} {1,15:G} {1,8:D} {2,5}";
            foreach (Plant p in Enum.GetValues(typeof(Plant)))
            {
                Logger.Info(String.Format(format, "Plant", p, Base36.Encode((long)p, 2)));
            }
            foreach (Model p in Enum.GetValues(typeof(Model)))
            {
                Logger.Info(String.Format(format, "Model", p, Base36.Encode((long)p, 3)));
            }
        }

        public static string Generate(Plant p, Model m, long number)
        {
            DateTime d = DateTime.Now;
            String serial = "";
            serial += Base36.Encode((long)p, 2);                            //Plant(2 digits)
            serial += d.ToString("yy").Substring(1);                        //Year (1 digit)
            serial += String.Format("{0:D2}", GetIso8601WeekOfYear(d));     //Week (2 digits)
            serial += Base36.Encode(number, 3);                             //Serial number (3 digits)
            serial += Base36.Encode((long)m, 3);                            //Model (3 digits)
            return serial.ToUpper();
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }

    public static class Base36
    {
        private const string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Encode the given number into a Base36 string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String Encode(long input, int length)
        {
            if (input < 0) throw new ArgumentOutOfRangeException("input", input, "input cannot be negative");

            char[] clistarr = CharList.ToCharArray();
            var result = new Stack<char>();
            while (input != 0)
            {
                result.Push(clistarr[input % 36]);
                input /= 36;
            }
            char[] padding = new char[length - result.Count];
            for(int i = 0; i < padding.Length; i++) {
                padding[i] = clistarr[0];
            }
            return new string(padding) + new string(result.ToArray());
        }

        /// <summary>
        /// Decode the Base36 Encoded string into a number
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Int64 Decode(string input)
        {
            var reversed = input.ToLower().Reverse();
            long result = 0;
            int pos = 0;
            foreach (char c in reversed)
            {
                result += CharList.IndexOf(c) * (long)Math.Pow(36, pos);
                pos++;
            }
            return result;
        }
    }

}

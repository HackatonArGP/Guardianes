using System;
using System.Collections.Generic;

namespace Earthwatchers.Models
{
    internal class BaseConverter
    {
        public static string NegativeNumberToBaseString(int number, int baseNum)
        {
            var remainder = number % baseNum;

            int quotient = number / baseNum;

            if (remainder < 0)
            {
                remainder = remainder + Math.Abs(baseNum);
                quotient = quotient + 1;
            }

            var resultStorage = new List<string>();
            resultStorage.Add(remainder.ToString());

            while (quotient != 0)
            {
                remainder = quotient % baseNum;
                quotient = quotient / baseNum;
                if (remainder < 0)
                {
                    remainder = remainder + Math.Abs(baseNum);
                    quotient = quotient + 1;
                }

                resultStorage.Add(remainder.ToString());
            }

            resultStorage.Reverse();
            return String.Join("", resultStorage.ToArray());
        }

        public static string NumberToBaseString(string numberStr, int baseNum)
        {
            int number;

            if (!int.TryParse(numberStr, out number))
                return "";

            if (baseNum < 0)
            {
                return NegativeNumberToBaseString(number, baseNum);
            }

            if (number < baseNum)
            {
                return number.ToString();
            }

            var rem = number % baseNum;
            var result = rem.ToString();
            var reducedNum = (number - rem) / baseNum;
            var restOfString = NumberToBaseString(reducedNum.ToString(), baseNum);
            return restOfString + result;
        }

        public static int BaseStringToValue(string digitString, int baseNum)
        {
            if (string.IsNullOrEmpty(digitString))
                return 0;

            var result = digitString.Remove(0, digitString.Length - 1);
            var remainingString = digitString.Remove(digitString.Length - 1, 1);
            var valueOfRemainingString = BaseStringToValue(remainingString, baseNum);
            return int.Parse(result) + (baseNum * valueOfRemainingString);
        }

        public static string ConvertBase(string numberString, int baseOrginal, int baseResult)
        {
            if (baseOrginal == 0 || baseResult == 0)
                return string.Empty;

            var number = BaseStringToValue(numberString, baseOrginal);
            return NumberToBaseString(number.ToString(), baseResult);
        }
    }
}

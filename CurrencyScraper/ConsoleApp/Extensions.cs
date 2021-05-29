using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public static class Extensions
    {
        public static int GetNthIndex(this string s, char characterToFind, int n)
        {
            //Get n-th appearance of char in string
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == characterToFind)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static DateTime TwoDaysAgo(this DateTime date)
        {
            long currentTicks = date.Ticks;
            long twoDayTicks = date.AddDays(2).Ticks - currentTicks;
            long twoDaysBeaforeTicks = currentTicks - twoDayTicks;
            return new DateTime(twoDaysBeaforeTicks);
        }
    }
}

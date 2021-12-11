using System;
using System.Collections.Generic;

namespace SoftwareDevelopment
{
    class Program
    {
        static void Main(string[] args)
        {
            IDictionary <String, int> dict = new HashDictionary<String, int>();

            dict["1"] = 1;
            dict["2"] = 2;
            dict["1"] = 3;

            dict.Add("3", 20);
            Console.WriteLine(dict);
            Console.WriteLine(dict["1"]);
        }
    }
}

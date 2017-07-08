using System;
using System.Collections;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<object > n = new List<object>();

            bool b = false;
            if (b)
            {
                foreach (var n1 in n)
                {
                    Console.WriteLine(n1);
                }

            }
            else
            {
                foreach (var n1 in n)
                {
                   
                    Console.WriteLine(n1);
                }
            }
          
        }
    }
}
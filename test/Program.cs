using System;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new C();
            var b =c;
            var a = b;
            b = null;
            c.Name = "c";
            Console.WriteLine(a.Name);
            Console.ReadLine();
        }
    }
    class C {

        public string Name { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPlusTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var p1 = new Patient
            {
                FirstName = "Janko",
                LastName = "Hrasko",
                Birthday = DateTime.Now,
                CardId = 1
            };
            var p2 = new Patient
            {
                FirstName = "Maria",
                LastName = "Slovakova",
                Birthday = DateTime.Now,
                CardId = 2
            };
            var blok = new Block<Patient>(10);
            blok.Add(p1);
            blok.Add(p2);
            var bytes = blok.GetBytes();
            blok.FromBytes(bytes);
        }
    }
}

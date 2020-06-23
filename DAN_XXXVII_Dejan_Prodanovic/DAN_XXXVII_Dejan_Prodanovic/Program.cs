using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAN_XXXVII_Dejan_Prodanovic
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> randomNumbers = new List<int>();
            List<int> divisibleByThree = new List<int>();
            Random rnd = new Random();

            for (int i = 0; i < 1000; i++)
            {
                randomNumbers.Add(rnd.Next(1,5001));
            }

            foreach (var rnum in randomNumbers)
            {
                if (rnum%3 == 0)
                {
                    divisibleByThree.Add(rnum);
                }
            }
            divisibleByThree.Sort();

            

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(divisibleByThree[i]);  
            }
            Console.ReadLine();
        }
    }
}

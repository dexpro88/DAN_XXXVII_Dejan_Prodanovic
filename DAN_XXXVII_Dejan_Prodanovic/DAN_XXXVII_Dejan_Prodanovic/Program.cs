using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XXXVII_Dejan_Prodanovic
{
    class Program
    {
        public static Semaphore Semaphore { get; set; }
        static Random timeRnd = new Random();
        static int counter = 0;

        public static void PrepareTrucks()
        {
            for (int i = 1; i <= 10; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(LoadOn));
                t.Start(i);
            }

           
        }
        public static void LoadOn(object o)
        {
           
            Console.WriteLine("Kamion {0} ceka na utovar", o);
            Semaphore.WaitOne();
            Console.WriteLine("Kamion {0} se utovara", o);
            
            Thread.Sleep(timeRnd.Next(500,5000));
           
            Console.WriteLine("Kamion {0} je zavrsio utovar", o);
            counter++;
            if (counter%2==0)
            {
                Semaphore.Release(2);
            }
            
           

        }
        static void Main(string[] args)
        {
            List<int> randomNumbers = new List<int>();
            List<int> divisibleByThree = new List<int>();
            Random rnd = new Random();

            Semaphore = new Semaphore(2,3);
            PrepareTrucks();
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
 

            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine(divisibleByThree[i]);  
            //}
            Console.ReadLine();
        }
    }
}

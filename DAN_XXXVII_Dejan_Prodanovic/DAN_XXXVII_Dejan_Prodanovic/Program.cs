using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XXXVII_Dejan_Prodanovic
{
    class Program
    {

        private static object theLock = new object();

        static void GenerateRandomNumbers()
        {
             
            Random rnd = new Random();
            if (File.Exists("../../Routes.txt"))
            {
                System.IO.File.WriteAllText(@"../../Routes.txt", string.Empty);
            }

            lock (theLock)
            {
               
                StreamWriter sw = File.AppendText("../../Routes.txt");
                for (int i = 0; i < 1000; i++)
                {
                    int rndNumber = rnd.Next(1, 5001);          
                    sw.WriteLine(rndNumber);
                }
                               
                sw.Close();
            }
             
        }

        static void ChooseBestRoutes( out List<int> bestRoutes)
        {
            List<int> divisibleByThree = new List<int>();

            using (StreamReader sr = new StreamReader("../../Routes.txt"))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    int number = Int32.Parse(line);
                    if (number % 3== 0)
                    {
                        divisibleByThree.Add(number); ;
                    }
                }
            }

            divisibleByThree.Sort();

            bestRoutes = divisibleByThree.Take(10).Distinct().ToList();

        }
        static void Main(string[] args)
        {
            

            TruckLoad tl = new TruckLoad();

            //tl.PrepareTrucks();

            List<int> randomNumbers = new List<int>();
            List<int> bestRoutes = new List<int>();

            Thread t1 = new Thread(()=>GenerateRandomNumbers());
            Thread t2 = new Thread(() => ChooseBestRoutes(out bestRoutes));

            t1.Start();
            t1.Join();
            
            t2.Start();

          
            t2.Join();

            foreach (var item in bestRoutes)
            {
                Console.WriteLine(item);
            }


            Console.ReadLine();
        }
    }
}

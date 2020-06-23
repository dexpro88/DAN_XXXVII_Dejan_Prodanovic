using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XXXVII_Dejan_Prodanovic
{
    class Menager
    {
        private static object theLock = new object();

        public void GenerateRandomNumbers()
        {

            Random rnd = new Random();
            if (File.Exists("../../Routes.txt"))
            {
                System.IO.File.WriteAllText(@"../../Routes.txt", string.Empty);
            }

            lock (theLock)
            {
                Thread.Sleep(10);
                StreamWriter sw = File.AppendText("../../Routes.txt");
                for (int i = 0; i < 1000; i++)
                {
                    int rndNumber = rnd.Next(1, 5001);
                    sw.WriteLine(rndNumber);
                }

                sw.Close();
                Monitor.Pulse(theLock);
                
            }
             
        }

        public void ChooseBestRoutes(out List<int> bestRoutes)
        {
            List<int> divisibleByThree = new List<int>();

            lock (theLock)
            {
                Monitor.Wait(theLock, 3000);
                using (StreamReader sr = new StreamReader("../../Routes.txt"))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        int number = Int32.Parse(line);
                        if (number % 3 == 0)
                        {
                            divisibleByThree.Add(number); ;
                        }
                    }
                }
            }


            divisibleByThree.Sort();
            bestRoutes = divisibleByThree.Distinct().Take(10).ToList();

            Console.WriteLine("Menadzer je odabrao najbolje rute a to su:");

            foreach (var item in bestRoutes)
            {
                Console.WriteLine(item);
            }

        }
    }
}

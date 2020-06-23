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

        
        static void Main(string[] args)
        {
            

            TruckLoad truckLoad = new TruckLoad();
            Menager menager = new Menager();

            //tl.PrepareTrucks();

            List<int> randomNumbers = new List<int>();
            List<int> bestRoutes = new List<int>();

            Thread t1 = new Thread(()=> menager.GenerateRandomNumbers());
            Thread t2 = new Thread(() => menager.ChooseBestRoutes(out bestRoutes));

            t2.Start();
            t1.Start();
          
            
         

            t1.Join();
            t2.Join();

            Console.WriteLine();
            truckLoad.PrepareTrucks(bestRoutes);
            Console.WriteLine();
            truckLoad.StartDestinationThreads(bestRoutes);
            Console.WriteLine();
            truckLoad.ChooseRoutesForTrucks(bestRoutes);

            Console.ReadLine();
        }
    }
}

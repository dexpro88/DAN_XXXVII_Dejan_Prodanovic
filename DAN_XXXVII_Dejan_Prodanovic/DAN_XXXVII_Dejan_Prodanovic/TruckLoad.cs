using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XXXVII_Dejan_Prodanovic
{
    class TruckLoad
    {
        public static Semaphore Semaphore { get; set; }
        Random timeRnd = new Random();
        Dictionary<int,Thread> trucks = new Dictionary<int,Thread>();
        Dictionary<int,Thread> destinations = new Dictionary<int,Thread>();
       
        static Dictionary<int, bool> isTruckArrivedIndicators = new Dictionary<int, bool>();
        static Dictionary<int, bool> isTruckLateIndicators = new Dictionary<int, bool> ();
        static Dictionary<int, object> locks = new Dictionary<int, object>();
        static int counter = 0;
        bool isArrived = false;
        bool isLate = false;

        public TruckLoad()
        {
            Semaphore = new Semaphore(2, 3);
           
        }

        public void PrepareTrucks(List<int> bestRoutes)
        {
            int counter = 1;
            foreach (var route in bestRoutes)
            {
                Thread thread = new Thread(new ThreadStart(LoadOn));
                thread.Name = String.Format("Kamion{0}", counter++);
                trucks.Add(route, thread);
                thread.Start();
            }
            foreach (var truck in trucks)
            {
                truck.Value.Join();
            }
        }
        public void LoadOn()
        {
            Console.WriteLine("{0} ceka na utovar", Thread.CurrentThread.Name);  
            Semaphore.WaitOne();
            Console.WriteLine("{0} se utovara", Thread.CurrentThread.Name);

            Thread.Sleep(timeRnd.Next(500, 5000));

            Console.WriteLine("{0} je zavrsio utovar", Thread.CurrentThread.Name);
            counter++;
            if (counter % 2 == 0)
            {
                Semaphore.Release(2);
            }
        }

        

        public void ChooseRoutesForTrucks(List<int>bestRoutes)
        {
            trucks.Clear();
            int counter = 1;
            
            foreach (var route in bestRoutes)
            {
                
               
                int timeOfDelivery = timeRnd.Next(500, 5000);
             

                Thread thread = new Thread(() => StartDelivery(route, timeOfDelivery));
                thread.Name = String.Format("Kamion{0}", counter++);
                trucks.Add(route, thread);
               
            }
            foreach (var thread in trucks)
            {
                thread.Value.Start();
            }
            
        }

        public void StartDestinationThreads(List<int> bestRoutes)
        {
            int counter = 1;
            
            foreach (var route in bestRoutes)
            {
                object theLock = new object();
                locks.Add(route, theLock);
                isTruckArrivedIndicators.Add(route, false);
                isTruckLateIndicators.Add(route, false);
                Thread thread = new Thread(() => DestinationWait(route));
                thread.Name = String.Format("Destinacija{0}", counter++);
                destinations.Add(route, thread);
                thread.Start();
            }
        }

        public void StartDelivery(int route,int timeOfDelivery)
        {
           
            lock (locks[route])
            {
                
                Console.WriteLine("{0} je dobio rutu {1} i krece na odrediste {2}." +
                    " Krenuo sam i dolazim za {3}", Thread.CurrentThread.Name, route,
                destinations[route].Name, timeOfDelivery);
                var watch = System.Diagnostics.Stopwatch.StartNew();

                Monitor.Wait(locks[route], timeOfDelivery);
                if (isTruckLateIndicators[route])
                {
                    watch.Stop();

                    Console.WriteLine("\n{0} - {1} je zakasinio na odrediste", destinations[route].Name,
                        trucks[route].Name);
                    Console.WriteLine("{0} - Vracam se na pocetnu tacku\n",trucks[route].Name);
                    Thread.Sleep((int)watch.ElapsedMilliseconds);
                    Console.WriteLine("{0} - Stigao sam na pocetnu tacku", trucks[route].Name);
                }
                else
                {
                    watch.Stop();

                    isTruckArrivedIndicators[route] = true;
                    Monitor.Pulse(locks[route]);
                }
            }
           
        }

        public void DestinationWait(int route)
        {
            lock (locks[route])
            {
                Console.WriteLine("{0} ceka ", Thread.CurrentThread.Name);
                Monitor.Wait(locks[route], 3000);
                if (isTruckArrivedIndicators[route])
                {
                    Console.WriteLine("\n{0} - {1} je stigao na odrediste",destinations[route].Name,
                        trucks[route].Name);
                }
                else
                {
                    isTruckLateIndicators[route] = true;
                    Monitor.Pulse(locks[route]);
                }
            }
        }
    }
}

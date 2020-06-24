﻿using System;
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
        //stores threads that represent trucks
        //key is the route of that truck
        Dictionary<int,Thread> trucks = new Dictionary<int,Thread>();
        //stores threads that represent destinations
        //key is the route of that destination 
        Dictionary<int,Thread> destinations = new Dictionary<int,Thread>();
       
        static Dictionary<int, bool> isTruckArrivedIndicators = new Dictionary<int, bool>();
        static Dictionary<int, bool> isTruckLateIndicators = new Dictionary<int, bool> ();
        static Dictionary<int, object> locks = new Dictionary<int, object>();
        //stores values of time duration of load for ever truck
        //key is the thruck name
        static Dictionary<string, int> loadDurations = new Dictionary<string, int>();
        static int counter = 0;
       
        public TruckLoad()
        {
           
            Semaphore = new Semaphore(2, 3);
           
        }

        /// <summary>
        /// method that create 10 threads  that represent truck
        /// every thread perform LoadOn method
        /// </summary>
        /// <param name="bestRoutes"></param>
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
        /// <summary>
        /// method that simulates load of truck
        /// it sleeps random amount of time between 0.5 and 5 s
        /// it usese semaphore and on every even value of counter it releses sempahore for two threads
        /// </summary>
        public void LoadOn()
        {
            Console.WriteLine("{0} ceka na utovar", Thread.CurrentThread.Name);  
            Semaphore.WaitOne();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("{0} se utovara", Thread.CurrentThread.Name);

            Thread.Sleep(timeRnd.Next(500, 5000));

            Console.WriteLine("{0} je zavrsio utovar", Thread.CurrentThread.Name);
            counter++;
            watch.Stop();
            loadDurations.Add(Thread.CurrentThread.Name, (int)watch.ElapsedMilliseconds);
            if (counter % 2 == 0)
            {
                Semaphore.Release(2);
            }
        }


        /// <summary>
        /// method that create 10 threads that represent 10 trucks
        /// those trucks are suposed to start delivery to destination
        /// every thread perform StartDelivery method with route and timeOfDelivery ad parameters
        /// </summary>
        /// <param name="bestRoutes"></param>
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

        /// <summary>
        /// method that starts 10 threads that represent destinations
        /// every thread performs DestinationWait method
        /// it sets indicators in isTruckArrivedIndicators and isTruckLateIndicators on false
        /// </summary>
        /// <param name="bestRoutes"></param>
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
        /// <summary>
        /// method that represents truck delivery
        /// it calls  Monitor.Wait method for randomly generated time  
        /// if method DestinationWait pulse the lock it will inform user that he is late and than 
        /// it will simulate returning to starting point by sleeping for the same amount of time
        /// it needed to get to the current point
        /// </summary>
        /// <param name="route"></param>
        /// <param name="timeOfDelivery"></param>
        public void StartDelivery(int route,int timeOfDelivery)
        {
           
            lock (locks[route])
            {
                
                Console.WriteLine("\n{0} je dobio rutu {1} i krece na odrediste {2}." +
                    "\n{3} : Krenuo sam i dolazim za {4} ms", Thread.CurrentThread.Name, route,
                destinations[route].Name, Thread.CurrentThread.Name, timeOfDelivery);
                //the stopwatch starts
                var watch = System.Diagnostics.Stopwatch.StartNew();

                Monitor.Wait(locks[route], timeOfDelivery);
                if (isTruckLateIndicators[route])
                {
                    //the stopwatch stops so we can know the amount of time the truck needed to get to this point
                    //then it will sleep for the same amount of time to simulate returning  to the start point
                    watch.Stop();

                    Console.WriteLine("\n{0} : {1} je zakasinio na odrediste", destinations[route].Name,
                        trucks[route].Name);
                    Console.WriteLine("{0} : Vracam se na pocetnu tacku\n",trucks[route].Name);
                    Thread.Sleep((int)watch.ElapsedMilliseconds);
                    Console.WriteLine("\n{0} : Vratio sam se na pocetnu tacku", trucks[route].Name);
                }
                else
                {
                    watch.Stop();

                    isTruckArrivedIndicators[route] = true;
                    Monitor.Pulse(locks[route]);
                }
            }
           
        }
        /// <summary>
        /// method that represents reaction of destination
        /// it calls Monitor.Wait method for 3s. If the StartDelivery method pulse it will inform user that the 
        /// truck is arrived on the destination and it will simulate unloading of the truck 
        /// </summary>
        /// <param name="route"></param>
        public void DestinationWait(int route)
        {
            lock (locks[route])
            {
                Console.WriteLine("{0} ceka ", Thread.CurrentThread.Name);
                Monitor.Wait(locks[route], 3000);
                if (isTruckArrivedIndicators[route])
                {
                    Console.WriteLine("\n{0} : {1} je stigao na odrediste i pocinje da se istovara",destinations[route].Name,
                        trucks[route].Name);
                    Thread.Sleep(loadDurations[trucks[route].Name]);
                    Console.WriteLine("\n{0} je istovaren", trucks[route].Name);
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

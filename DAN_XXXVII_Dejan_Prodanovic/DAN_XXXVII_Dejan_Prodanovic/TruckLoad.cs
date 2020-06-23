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
                Thread thread = new Thread(() => StartDelivery(route));
                thread.Name = String.Format("Kamion{0}", counter++);
                trucks.Add(route, thread);
                thread.Start();
            }           
            
        }

        public void StartDestinationThreads(List<int> bestRoutes)
        {
            int counter = 1;
            foreach (var route in bestRoutes)
            {
                Thread thread = new Thread(() => DestinationWait());
                thread.Name = String.Format("Destinacija{0}", counter++);
                destinations.Add(route, thread);
                thread.Start();
            }
        }

        public void StartDelivery(int route)
        {
            Console.WriteLine("{0} je dobio rutu {1} i krece na odrediste {2}", Thread.CurrentThread.Name, route,
                 destinations[route].Name);
        }

        public void DestinationWait( )
        {
            Console.WriteLine("{0} ceka ", Thread.CurrentThread.Name);
        }
    }
}

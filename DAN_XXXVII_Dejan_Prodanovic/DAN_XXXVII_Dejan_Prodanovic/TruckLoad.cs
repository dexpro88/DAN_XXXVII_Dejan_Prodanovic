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
        Thread[] trucks = new Thread[10];
        static int counter = 0;

        public TruckLoad()
        {
            Semaphore = new Semaphore(2, 3);
        }

        public void PrepareTrucks()
        {
            for (int i = 0; i < 10; i++)
            {
                trucks[i] = new Thread(new ThreadStart(LoadOn));
                trucks[i].Name = String.Format("Kamion{0}",i+1);
                //Thread t = new Thread(new ParameterizedThreadStart(LoadOn));
                trucks[i].Start();
            }

            foreach (var truck in trucks)
            {
                truck.Join();
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

        public void StartDelivery()
        {

        }
    }
}

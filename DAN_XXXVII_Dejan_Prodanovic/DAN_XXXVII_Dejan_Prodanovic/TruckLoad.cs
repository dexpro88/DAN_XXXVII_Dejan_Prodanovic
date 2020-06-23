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
        static Random timeRnd = new Random();
        static int counter = 0;

        public TruckLoad()
        {
            Semaphore = new Semaphore(2, 3);
        }

        public void PrepareTrucks()
        {
            for (int i = 1; i <= 10; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(LoadOn));
                t.Start(i);
            }


        }
        public void LoadOn(object o)
        {
            Console.WriteLine("Kamion {0} ceka na utovar", o);
            Semaphore.WaitOne();
            Console.WriteLine("Kamion {0} se utovara", o);

            Thread.Sleep(timeRnd.Next(500, 5000));

            Console.WriteLine("Kamion {0} je zavrsio utovar", o);
            counter++;
            if (counter % 2 == 0)
            {
                Semaphore.Release(2);
            }



        }
    }
}

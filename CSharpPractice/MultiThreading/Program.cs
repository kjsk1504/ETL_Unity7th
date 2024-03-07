using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace MultiThreading
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            Thread baristaThread1 = new Thread(() => 
                                               {
                                                    HireBarista("carl")
                                                        .GoToCafe("Luke's coffee")
                                                        .MakeCoffee();

                                                    //Barista barista = HireBarista("carl");
                                                    //barista.GoToCafe("Luke's coffee");
                                                    //barista.MakeCoffee();
                                               },
                                                1024 * 1024 / 2);

            baristaThread1.Name = "Barista Thread 1";
            baristaThread1.IsBackground = true; // 메인 스레드의 백그라운드로만 사용한다. (메인 스레드가 중단되면 이 스레드도 중단됨)
            baristaThread1.Start();
            baristaThread1.Join(); // 본인을 호출한 스레드를 기다리게 하는 함수

            Thread.Sleep(1000);

            ThreadPool.SetMinThreads(1, 0);
            ThreadPool.SetMaxThreads(4, 8);

            // Task : ThreadPool에서 Thread를 빌려다가 작업을 할당하는 클래스
            Task task1 = new Task(() =>
                                  {
                                      HireBarista("carl")
                                        .GoToCafe("Luke's coffee")
                                        .MakeCoffee();
                                  });
            task1.Start();
            task1.Wait();

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                int tmpID = i;
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    HireBarista($"Barista{tmpID}")
                        .GoToCafe("Luke's coffee")
                        .MakeCoffee();
                }));
                tasks[tmpID].Wait();
            }

            Task.WaitAll(tasks.ToArray()); // task 전부를 기다림
            */
            
            // Generic Task : 작업의 결과물(결과값)을 반환받고 싶을 때 사용

            Task<string> taskWithResult = new Task<string>(() =>
            {
                return HireBarista("Smart Barista")
                            .GoToCafe("Luke's coffee")
                            .MakeCoffee()
                            .ToString();
            });
            taskWithResult.Start();
            taskWithResult.Wait();
            Console.WriteLine(taskWithResult.Result);
        }

        static Barista HireBarista(string nickname)
        {
            Barista barista = new Barista(nickname);
            return barista;
        }
    }


    public enum Beverage
    {
        Aspresso,
        Latte,
        Ade
    }


    public class Barista
    {
        public Barista(string name)
        {
            this.name = name;
        }

        public string name { get; private set; }
        private Random random = new Random();

        public Barista GoToCafe(string cafeName)
        {
            Console.WriteLine($"바리스타 {name}은(는) {cafeName} 카페로 출근합니다.");
            return this; // 체이닝 함수
        }

        public Beverage MakeCoffee()
        {
            Console.WriteLine($"바리스타 {name}은(는) 커피 추출을 시작합니다...");
            Thread.Sleep(3000);
            Console.WriteLine($"바리스타 {name}은(는) 커피 추출을 완료했습니다.");
            return (Beverage)random.Next(0, Enum.GetValues(typeof(Beverage)).Length);
        }
    }
}

namespace MultiThreadingTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int seconds = 5;
            Task<string>[] tasks = new Task<string>[2] { MakeBaristaToWork(seconds), WaitSeconds(seconds) };
            Task.WaitAll(tasks);
            for (int i = 0; i < tasks.Length; i++)
            {
                Console.Write(tasks[i].Result);
            }
            Console.WriteLine();
        }

        static async Task<string> MakeBaristaToWork(int seconds)
        {
            Beverage result = await HireBarista("Smart Barista")
                                        .GoToCafe("Luke's Coffee")
                                        .MakeCoffee(seconds);
            return $"{result}을/를 ".ToString();
        }

        static async Task<string> WaitSeconds(int seconds)
        {
            for (int i = 0; i < seconds; i++)
            {
                Console.WriteLine($"{i} seconds");
                await Task.Delay(1000);
            }
            return $"{seconds}초 동안 만듬".ToString();
        }

        static Barista HireBarista(string nickname)
        {
            Barista barista = new Barista(nickname);
            return barista;
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

            public async Task<Beverage> MakeCoffee(int seconds) // async : 비동기 구문임을 명시
            {
                Console.WriteLine($"바리스타 {name}은(는) 커피 추출을 시작합니다...");
                await Task.Delay(seconds * 1000); // 할당(new)하고 시작(Start)하고 기다리는(Wait) 로직을 await로 간소화
                //Task task1 = new Task(() =>
                //                      {
                //                          Thread.Sleep(3000);
                //                      });
                //task1.Start();
                //task1.Wait();
                Console.WriteLine($"바리스타 {name}은(는) 커피 추출을 완료했습니다.");
                return (Beverage)random.Next(0, Enum.GetValues(typeof(Beverage)).Length);
            }
        }
    }
}

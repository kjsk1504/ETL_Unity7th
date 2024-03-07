namespace MultiThreading_AsyncAwait
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task<string> task = MakeBaristaToWork();
            task.Wait();

            Console.WriteLine(task.Result);

            List<Task<Beverage>> baristaTasks = new List<Task<Beverage>>();
            Barista barista = HireBarista("Super barista");
            for (int i = 0; i < 10; i++)
            {
                baristaTasks.Add(barista.MakeCoffee());
            }

            Task.WaitAll(baristaTasks.ToArray());
            for (int i = 0; i < baristaTasks.Count; i++)
            {
                Console.WriteLine(baristaTasks[i].Result);
            }
        }

        static async Task<string> MakeBaristaToWork()
        {
            Beverage result = await HireBarista("Smart Barista")
                                        .GoToCafe("Luke's Coffee")
                                        .MakeCoffee();
            return result.ToString();
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

        public async Task<Beverage> MakeCoffee() // async : 비동기 구문임을 명시
        {
            Console.WriteLine($"바리스타 {name}은(는) 커피 추출을 시작합니다...");
            await Task.Delay(3000); // 할당(new)하고 시작(Start)하고 기다리는(Wait) 로직을 await로 간소화
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

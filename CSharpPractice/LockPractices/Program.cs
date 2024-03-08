namespace LockPractices
{
    internal class Program
    {
        private static object s_lock = new object();
        private static bool s_lockTaken;
        public static int count;


        static void Main(string[] args)
        {

        }

        static void IncreasCount()
        {
            lock (s_lock)
            {
                Console.WriteLine(PPAfter(ref count));
            }
        }

        static void IncreasCount2()
        {
            Monitor.Enter(s_lock); // wait until resource is released
            Console.WriteLine(PPAfter(ref count));
            Monitor.Exit(s_lock);
        }

        static void IncreasCount3()
        {
            Monitor.Enter(s_lock, ref s_lockTaken);
            if (s_lockTaken)
            {
                Console.WriteLine(PPAfter(ref count));
            }
            else
            {

            }

            if (s_lockTaken)
                Monitor.Exit(s_lock);
        }

        static void IncreasCount4()
        {
            try
            {
                Monitor.Enter(s_lock, ref s_lockTaken);
            }
            catch (Exception e) // 예외를 직접 핸들링
            {
                // 에러를 처리하기 힘들경우 (에디터에서만 발생하는 인증 에러 등)
                // 하나의 시도에서 여러 에러가 터질 경우
                if (e is IndexOutOfRangeException)
                {
                    Console.WriteLine("인텍스 초과함");
                }
                else if (e is NullReferenceException)
                {
                    Console.WriteLine("참조 없음");
                }
                Console.WriteLine(e.Message); // 모든 예외를 다 캐치함
            }
            finally
            {
                if (s_lockTaken)
                {
                    Console.WriteLine(PPAfter(ref count));
                    Monitor.Exit(s_lock);
                }
            }
        }

        static int PPAfter(ref int value) // 다른 스레드에서 동시에 접근하면 return은 0이지만 count는 2가 됨.
        {
            int origin = value;
            value = value + 1;
            return origin;
        }
    }
}

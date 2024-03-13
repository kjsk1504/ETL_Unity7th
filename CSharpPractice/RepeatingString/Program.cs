using System.Text;

namespace RepeatingString
{
    internal class Program
    {
        static string Repeat(string c, int n)
        {
            StringBuilder stringBuilder = new StringBuilder();
            return stringBuilder.Append(c, stringBuilder.Length, 30).ToString();
        }

        static void Main(string[] args)
        {
            Console.WriteLine(Repeat("a", 30));
        }
    }
}

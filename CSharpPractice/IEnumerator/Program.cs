using System.Collections;

namespace IEnumeratorPractice
{
    public class MyCollectionTest
    {
        static void Main()
        {
            IEnumerable C_Something()
            {
                yield return 1;
                yield return "철수";
                yield return 5.0f;
            }

            MyCollection myCollection = new MyCollection(new ArrayList() { 1, "철수", 5.0f });

            foreach (var item in C_Something())
            {
                Console.WriteLine(item);
            }

            IEnumerator e = myCollection.GetEnumerator();
            while (e.MoveNext())
            {
                Console.WriteLine(e.Current);
            }
            e.Reset();

            foreach (var item in myCollection)
            {
                Console.WriteLine(item);
            }

            List<int> list = new List<int>();
            using (IEnumerator<int> e1 = list.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    Console.WriteLine(e1.Current);
                }
                e1.Reset();
            }
        }

        public class MyCollection : IEnumerable
        {
            public MyCollection(ArrayList data)
            {
                _data = data;
            }

            private ArrayList _data;

            public IEnumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public struct Enumerator : IEnumerator
            {
                public Enumerator(MyCollection list)
                {
                    _current = null;
                    _items = list._data;
                    _index = 0;
                }

                public object? Current => _current;
                private object? _current;
                private readonly ArrayList _items;
                private int _index;

                public bool MoveNext()
                {
                    if (_index < _items.Count)
                    {
                        _current = _items[_index++];
                        return true;
                    }

                    return false;
                }

                public void Reset()
                {
                    _index = 0;
                    _current = default;
                }
            }
        }
    }
}

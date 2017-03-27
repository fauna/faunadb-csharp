using FaunaDB.Types;

namespace Test
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public ObjectV Attribs { get; set; }

        public Person(string name, int age, ObjectV attribs)
        {
            Name = name;
            Age = age;
            Attribs = attribs;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Person;
            return other != null &&
                Name == other.Name &&
                Age == other.Age &&
                Attribs == other.Attribs;
        }

        public override int GetHashCode() => 0;
    }
}

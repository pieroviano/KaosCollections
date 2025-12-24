namespace Kaos.Test.Collections;

public class Person
{
    public static string[] names = new string[]
        { "Walter", "Bob", "Trent", "Chuck", "Alice" , "Maynard", "Frank", "Sybil", "Eve" };

    public string Name { get; private set; }
    public Person(string name) { Name = name; }
    public override string ToString() { return Name; }
}
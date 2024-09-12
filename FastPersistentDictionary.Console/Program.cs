using FastPersistentDictionary;
using System.Diagnostics;

internal class Program
{
    static void Main(string[] args)
    {
        string strValueTest = "Star Wars (later retitled Star Wars: Episode IV – A New Hope) is a 1977 American epic space opera film written and directed by George Lucas.";
        string pathMain = Path.Combine(System.Environment.CurrentDirectory, "data.tt");
        var fastPersistentDictionary = new FastPersistentDictionary<string, string>(path: pathMain, crashRecovery: false);
        //path: pathMain,
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 0; i < 1000; i++) 
        {
            fastPersistentDictionary.Add("name_" + i.ToString(), strValueTest);

        }
        for (int i = 0; i < 1000; i++)
        {
            fastPersistentDictionary.TryGetValue("name_" + i.ToString(), out var val);


        }


        for (int i = 0; i < 100; i++)
        {
            fastPersistentDictionary.Remove("name_" + i.ToString());


        }

        for (int i = 1001; i < 1100; i++)
        {
            fastPersistentDictionary.Add("name_" + i.ToString(), strValueTest);


        }

        fastPersistentDictionary.SaveDictionary("C:\\Users\\Admin\\source\\repos\\main Pronamics Work\\Fast-Persistent-Dictionary\\FastPersistentDictionary.Console\\bin\\Debug\\net8.0\\testMMM2.tt");

        fastPersistentDictionary.Dispose();

        stopwatch.Stop();
        Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
        Console.ReadLine();
    }
}
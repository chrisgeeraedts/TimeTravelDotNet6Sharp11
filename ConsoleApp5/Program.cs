// See https://aka.ms/new-console-template for more information

using ConsoleApp5;
using Loy.Tests;


/// Scenario
var scenario = new TestScenarioOne();
scenario.Prepare();
var results = scenario.Run();

foreach (var item in results.results)
{
    ConsoleHelper.WriteResult(item);
}

Console.WriteLine("press any key to close...");
Console.ReadKey();





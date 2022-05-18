using System;

namespace TimeTravelTest.Scenarios
{
    public interface ITestScenario<T>
    {
        void Prepare();
        TimeTravelResults<T> Run();
    }
}
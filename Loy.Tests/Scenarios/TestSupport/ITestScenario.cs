using System;

namespace Loy.Tests
{
    public interface ITestScenario<T>
    {
        void Prepare();
        TimeTravelResults<T> Run();
    }
}
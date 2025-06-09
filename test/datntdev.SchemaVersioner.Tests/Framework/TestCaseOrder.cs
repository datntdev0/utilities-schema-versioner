using Xunit.Abstractions;
using Xunit.Sdk;

namespace datntdev.SchemaVersioner.Tests.Framework
{
    public class AlphabeticalOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            return testCases.OrderBy(tc => tc.TestMethod.Method.Name);
        }
    }
}

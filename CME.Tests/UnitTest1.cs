using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CME.Data;

namespace CME.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private readonly ServiceCollection _serviceCollection;
        private readonly DbContextOptions<CMEDBContext> _options;
        public UnitTest1()
        {
            var optionBuilder = new DbContextOptionsBuilder<CMEDBContext>();
            //optionBuilder.
        }
        [TestMethod]
        public void TestMethod1()
        {
            //ServiceCollection
        }
    }
}

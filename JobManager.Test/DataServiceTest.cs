using System;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using JobManager.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JobManager.Test
{
    [TestClass]
    public class DataServiceTest
    {
        private readonly Repository dataService;

        public DataServiceTest()
        {
            dataService = new Repository();
        }

        [TestMethod]
        public void TestMethod1()
        {
            //Assert.ThrowsException<DbUpdateException>(() => dataService.AddAsync(new TransferObject() {Object = new SJob() }));
            //Assert.ThrowsException<DbUpdateException>(() => dataService.AddAsync(new TransferObject() { Object = new SJob {Id = 1 } }));
           // Assert.ThrowsException<ArgumentException>(() => dataService.GetAllAsyncTransfer(new TransferObject { Type = typeof(DummyClass) }));

            //Task<TransferObject> result = dataService.GetAllAsyncTransfer(new TransferObject { Type = typeof(SJob) });
            //result.Wait();
            //Assert.AreEqual(TaskStatus.RanToCompletion, result.Status);  

        }
    }

    internal class DummyClass
    {
        internal int Property1 { get; set; }

    }

}

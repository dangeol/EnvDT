using EnvDT.DataAccess;
using EnvDT.DataAccess.Repository;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Data.Common;
using System.Linq;
using Xunit;

namespace EnvDT.DataAccessTests.Repository
{
    public class RefValueRepositoryTests : EnvDTDataAccessTestBase, IDisposable
    {
        private readonly DbConnection _connection;

        private IRefValueRepository _repository;

        private Guid _publParamId;
        private Sample _sample;

        public RefValueRepositoryTests() 
            : base(
                new DbContextOptionsBuilder<EnvDTDbContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;

            _publParamId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff");
            _sample = new Sample();
            _sample.SampleId = new Guid("4d8e0575-00cb-4ca3-bcbc-5883557ff0da");
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:;Foreign Keys=False");

            connection.Open();

            return connection;
        }

        public void Dispose() => _connection.Dispose();

        /* DOES NOT WORK, the json data seeds are not found
        [Fact]
        public void GetRefValuesByPublParamIdTest()
        {
            using (var context = new EnvDTDbContext(ContextOptions))
            {
                _repository = new RefValueRepository(context);

                var refValues = _repository.GetRefValuesWithoutAttributesByPublParamId(_publParamId);

                Assert.Equal(2, refValues.Count());
                Assert.Equal(10.0, refValues.ElementAt(0).RValue);
                Assert.Equal(30.0, refValues.ElementAt(1).RValue);
            }
        }
        */
    }
}

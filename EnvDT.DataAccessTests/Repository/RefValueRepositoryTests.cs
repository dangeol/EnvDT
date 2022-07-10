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
        private readonly Sample _sample1;
        private readonly Sample _sample2;
        private readonly Sample _sample3;

        public RefValueRepositoryTests() 
            : base(
                new DbContextOptionsBuilder<EnvDTDbContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;

            _publParamId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff");
            _sample1 = new Sample
            {
                SampleId = new Guid("4d8e0575-00cb-4ca3-bcbc-5883557ff0da"),
                MediumSubTypeId = new Guid("52a64f24-8958-4837-bf28-bf9730a44dba") // mediumSubType1
            };
            _sample2 = new Sample
            {
                SampleId = new Guid("c1d6f33a-c3e7-4954-b676-8948ea2694c0"),
                ConditionId = new Guid("6a808e55-b41e-4296-bb4f-ebd275032242") // condition1
            };
            _sample3 = new Sample
            {
                SampleId = new Guid("6582467e-774e-45d6-95eb-a1821748d1ed"),
                MediumSubTypeId = new Guid("52a64f24-8958-4837-bf28-bf9730a44dba"), // mediumSubType1
                ConditionId = new Guid("6a808e55-b41e-4296-bb4f-ebd275032242") // condition1
            };
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:;Foreign Keys=False");

            connection.Open();

            return connection;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        [Fact]
        public void GetRefValuesWithoutAttributesByPublParamIdTest()
        {
            using var context = new EnvDTDbContext(ContextOptions);
            _repository = new RefValueRepository(context);

            var refValues = _repository.GetRefValuesWithoutAttributesByPublParamId(_publParamId);

            Assert.Equal(2, refValues.Count());
            Assert.Equal(10.0, refValues.ElementAt(0).RValue);
            Assert.Equal(30.0, refValues.ElementAt(1).RValue);
        }

        [Fact]
        public void GetRefValuesWithMedSubTypesByPublParamIdAndSampleTest()
        {
            using var context = new EnvDTDbContext(ContextOptions);
            _repository = new RefValueRepository(context);

            var refValues = _repository.GetRefValuesWithMedSubTypesByPublParamIdAndSample(_publParamId, _sample1);

            Assert.Equal(3, refValues.Count());
            Assert.Equal(10.0, refValues.ElementAt(0).RValue);
            Assert.Equal(30.0, refValues.ElementAt(1).RValue);
            Assert.Equal(25.0, refValues.ElementAt(2).RValue);
        }

        [Fact]
        public void GetRefValuesWithConditionsByPublParamIdAndSampleTest()
        {
            using var context = new EnvDTDbContext(ContextOptions);
            _repository = new RefValueRepository(context);

            var refValues = _repository.GetRefValuesWithConditionsByPublParamIdAndSample(_publParamId, _sample2);

            Assert.Equal(3, refValues.Count());
            Assert.Equal(10.0, refValues.ElementAt(0).RValue);
            Assert.Equal(30.0, refValues.ElementAt(1).RValue);
            Assert.Equal(42.5, refValues.ElementAt(2).RValue);
        }

        [Fact]
        public void GetRefValuesWithMedSubTypesAndConditionsByPublParamIdAndSampleTest()
        {
            using var context = new EnvDTDbContext(ContextOptions);
            _repository = new RefValueRepository(context);

            var refValues = _repository.GetRefValuesWithMedSubTypesAndConditionsByPublParamIdAndSample(_publParamId, _sample3);

            Assert.Equal(5, refValues.Count());
            Assert.Equal(10.0, refValues.ElementAt(0).RValue);
            Assert.Equal(30.0, refValues.ElementAt(1).RValue);
            Assert.Equal(25.0, refValues.ElementAt(2).RValue);
            Assert.Equal(42.5, refValues.ElementAt(3).RValue);
            Assert.Equal(31.5, refValues.ElementAt(4).RValue);
        }
    }
}

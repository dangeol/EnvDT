using EnvDT.DataAccess;
using EnvDT.DataAccess.Repository;
using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EnvDT.DataAccessTests.Repository
{
    public class GenericRepositoryTests
    {
        private Mock<EnvDTDbContext> _contextMock;
        private IGenericRepository<Sample> _repository;

        public GenericRepositoryTests()
        {
            _contextMock = new Mock<EnvDTDbContext>();
            _repository = new SampleRepository(_contextMock.Object);
        }

        [Fact]
        public void ShouldCallProperMethodWhenTestClassObjectIsCreated()
        {
            //Arrange
            var testObj = new Sample();
            var dbSetMock = new Mock<DbSet<Sample>>();

            _contextMock.Setup(x => x.Set<Sample>()).Returns(dbSetMock.Object);

            //Act
            _repository.Create(testObj);

            //Assert
            _contextMock.Verify(cm => cm.Set<Sample>());
            dbSetMock.Verify(dm => dm.Add(It.Is<Sample>(y => y == testObj)));
        }

        [Fact]
        public void ShouldCallProperMethodWhenTestClassObjectIsDeleted()
        {
            var testObj = new Sample();
            var dbSetMock = new Mock<DbSet<Sample>>();

            _contextMock.Setup(x => x.Set<Sample>()).Returns(dbSetMock.Object);

            _repository.Delete(testObj);

            _contextMock.Verify(cm => cm.Set<Sample>());
            dbSetMock.Verify(dm => dm.Remove(It.Is<Sample>(y => y == testObj)));
        }

        [Fact]
        public void ShouldCallProperMethodWhenGettingTestClassObjectById()
        {
            var testObj = new Sample();
            var dbSetMock = new Mock<DbSet<Sample>>();

            _contextMock.Setup(x => x.Set<Sample>()).Returns(dbSetMock.Object);

            _repository.GetById(new Guid());

            _contextMock.Verify(cm => cm.Set<Sample>());
            dbSetMock.Verify(dm => dm.Find(It.IsAny<Guid>()));
        }

        [Fact]
        public void ShouldCallProperMethodWhenGettingAllTestClassObjects()
        {
            var testObj = new Sample();
            var testObjList = new List<Sample>() { testObj };

            var dbSetMock = new Mock<DbSet<Sample>>();
            dbSetMock.As<IQueryable<Sample>>().Setup(x => x.Provider).Returns(testObjList.AsQueryable().Provider);
            dbSetMock.As<IQueryable<Sample>>().Setup(x => x.Expression).Returns(testObjList.AsQueryable().Expression);
            dbSetMock.As<IQueryable<Sample>>().Setup(x => x.ElementType).Returns(testObjList.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<Sample>>().Setup(x => x.GetEnumerator()).Returns(testObjList.AsQueryable().GetEnumerator());

            _contextMock.Setup(x => x.Set<Sample>()).Returns(dbSetMock.Object);

            var result = _repository.GetAll();

            Assert.Equal(testObjList, result.ToList());
        }
    }
}

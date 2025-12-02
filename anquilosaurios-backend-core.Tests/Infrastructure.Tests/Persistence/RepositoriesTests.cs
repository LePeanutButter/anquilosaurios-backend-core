using aquilosaurios_backend_core.API;
using aquilosaurios_backend_core.Domain.Models;
using aquilosaurios_backend_core.Infrastructure.Persistence;
using FluentAssertions;
using Moq;
using MongoDB.Driver;
using Xunit;

namespace aquilosaurios_backend_core.Tests.Infrastructure.Tests.Persistence
{ 
    public class UserRepositoryTests
    {
        private readonly Mock<IMongoCollection<User>> _mockCollection;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<User>>();
            _mockCollection.Setup(c => c.CollectionNamespace)
                .Returns(new CollectionNamespace(new DatabaseNamespace("TestDB"), "Users"));

            _mockDatabase = new Mock<IMongoDatabase>();
            _mockDatabase
                .Setup(db => db.GetCollection<User>("Users", It.IsAny<MongoCollectionSettings>()))
                .Returns(_mockCollection.Object);

            _repository = new UserRepository(_mockDatabase.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertUser()
        {
            var user = new User("Test", "email@test.com", "test", "pw");

            await _repository.CreateAsync(user);

            _mockCollection.Verify(
                c => c.InsertOneAsync(user, null, default),
                Times.Once);
        }

        [Fact]
        public async Task CreateManyAsync_ShouldInsertManyUsers()
        {
            var users = new List<User>
            {
                new User("A", "a@mail.com", "a", "pw"),
                new User("B", "b@mail.com", "b", "pw")
            };

            await _repository.CreateManyAsync(users);

            _mockCollection.Verify(
                c => c.InsertManyAsync(users, null, default),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReplaceUser()
        {
            var user = new User("Test", "t@mail.com", "test", "pw")
            {
                Id = Guid.NewGuid()
            };

            await _repository.UpdateAsync(user);

            _mockCollection.Verify(c =>
                c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<User>>(),
                    user,
                    It.IsAny<ReplaceOptions>(),
                    default),
                Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldDeleteUser()
        {
            var id = Guid.NewGuid();

            await _repository.DeleteByIdAsync(id);

            _mockCollection.Verify(c =>
                c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<User>>(),
                    default),
                Times.Once);
        }

        [Fact]
        public async Task DeleteByEmailAsync_ShouldDeleteUser()
        {
            var email = "delete@mail.com";

            await _repository.DeleteByEmailAsync(email);

            _mockCollection.Verify(c =>
                c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<User>>(),
                    default),
                Times.Once);
        }
    }
}

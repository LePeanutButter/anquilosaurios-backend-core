using aquilosaurios_backend_core.API;
using aquilosaurios_backend_core.Domain.Models;
using aquilosaurios_backend_core.Infrastructure.Persistence;
using FluentAssertions;
using Moq;
using MongoDB.Driver;
using Xunit;

namespace aquilosaurios_backend_core.Tests.Infrastructure.Tests.Persistence
{
    public class TestUserRepository : UserRepository
    {
        private readonly List<User> _users;
        public TestUserRepository(IMongoCollection<User> collection, List<User> users)
            : base(collection)
        {
            _users = users;
        }

        protected override Task<List<User>> ExecuteFindToListAsync(FilterDefinition<User> filter)
        {
            return Task.FromResult(_users);
        }
    }

    public class TestUserRepositoryWithSingleResult : UserRepository
    {
        private readonly User _user;

        public TestUserRepositoryWithSingleResult(IMongoCollection<User> collection, User user)
            : base(collection)
        {
            _user = user;
        }

        protected override Task<User?> ExecuteFindFirstOrDefaultAsync(FilterDefinition<User> filter)
        {
            return Task.FromResult<User?>(_user);
        }
    }


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
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            var users = new List<User>
            {
                new User("A", "a@mail.com", "a", "pw"),
                new User("B", "b@mail.com", "b", "pw")
            };

            var mockCollection = new Mock<IMongoCollection<User>>();
            var repository = new TestUserRepository(mockCollection.Object, users);

            var result = await repository.GetAllAsync();

            result.Should().BeEquivalentTo(users);
        }

        [Fact]
        public async Task GetByFiltersAsync_ShouldReturnUsersMatchingEmailFilter()
        {
            var users = new List<User>
            {
                new User("A", "a@mail.com", "a", "pw"),
                new User("B", "b@mail.com", "b", "pw")
            };

            var filters = new UserFiltersDTO { Email = "a@mail.com" };

            var mockCollection = new Mock<IMongoCollection<User>>();
            var repository = new TestUserRepository(mockCollection.Object, users);

            var result = await repository.GetByFiltersAsync(filters);

            result.Should().BeEquivalentTo(users);
        }

        [Fact]
        public async Task GetByFiltersAsync_ShouldReturnUsersMatchingNameFilter()
        {
            var users = new List<User>
            {
                new User("Alice", "alice@mail.com", "alice", "pw"),
                new User("Bob", "bob@mail.com", "bob", "pw")
            };

            var filters = new UserFiltersDTO { Name = "Ali" };

            var mockCollection = new Mock<IMongoCollection<User>>();
            var repository = new TestUserRepository(mockCollection.Object, users);

            var result = await repository.GetByFiltersAsync(filters);

            result.Should().BeEquivalentTo(users);
        }

        [Fact]
        public async Task GetByIdentifierAsync_ShouldReturnUserByEmail()
        {
            var user = new User("Test", "test@mail.com", "testuser", "pw");
            var users = new List<User> { user };

            var mockCollection = new Mock<IMongoCollection<User>>();

            var repo = new TestUserRepositoryWithSingleResult(
                mockCollection.Object,
                user
            );

            var result = await repo.GetByIdentifierAsync("test@mail.com");

            result.Should().Be(user);
        }

        [Fact]
        public async Task GetByIdentifierAsync_ShouldReturnUserByUsername()
        {
            var user = new User("Test", "test@mail.com", "myname", "pw");
            var users = new List<User> { user };

            var mockCollection = new Mock<IMongoCollection<User>>();

            var repo = new TestUserRepositoryWithSingleResult(
                mockCollection.Object,
                user
            );

            var result = await repo.GetByIdentifierAsync("myname");

            result.Should().Be(user);
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

using NUnit.Framework;

namespace Chat
{
    [TestFixture]
    class RoomServiceTest
    {
        RoomService _roomService;

        [SetUp]
        public void SetUp()
        {
            _roomService = new RoomService();
        }

        [Test]
        public void IsUserLoggedIn_WhenGivenTokenNotRegistered_ShouldReturnFalse()
        {
            var result = _roomService.IsUserLoggedIn("test_token");

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsUserLoggedIn_WhenGivenTokenRegistered_ShouldReturnTrue()
        {
            var testToken = "test_token";
            _roomService._users.Add(new User { Token = testToken });
            var result = _roomService.IsUserLoggedIn(testToken);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsUserNameTaken_WhenGivenUserNameNotRegistered_ShouldReturnFalse()
        {
            var result = _roomService.IsUserNameTaken("userName");

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsUserNameTaken_WhenGivenUserNameRegistered_ShouldReturnTrue()
        {
            var testUserName = "userName";
            _roomService._users.Add(new User { Name = testUserName });
            var result = _roomService.IsUserNameTaken(testUserName);

            Assert.That(result, Is.True);
        }

        [Test]
        public void SetUserRoom_ShouldChangeUserRoom()
        {
            var testToken = "test_token";
            var newRoom = "newRoom";
            _roomService._users.Add(new User { Name = "userName", Room = "testRoom", Token = testToken });
            _roomService.SetUserRoom(testToken, newRoom);

            Assert.That(_roomService._users[0].Room, Is.EqualTo(newRoom));
        }
    }
}

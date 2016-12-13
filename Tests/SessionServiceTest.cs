using NSubstitute;
using NUnit.Framework;

namespace Chat
{
    [TestFixture]
    class SessionServiceTest
    {
        IRoomService _roomServiceMock;
        SessionService _sessionService;

        [SetUp]
        public void SetUp()
        {
            _roomServiceMock = Substitute.For<IRoomService>();
            _sessionService = new SessionService(_roomServiceMock);
        }

        [Test]
        public void LogIn_UserNotLoggedAndUserNameNotTaken_UserShouldLogin()
        {
            var request = new OpenSessionRequest { UserName = "login1", Token = "123" };
            _roomServiceMock.IsUserLoggedIn(Arg.Any<string>()).Returns(false);
            _roomServiceMock.IsUserNameTaken(Arg.Any<string>()).Returns(false);

            var response = _sessionService.Handle(request);

            _roomServiceMock.Received().AddUser(Arg.Any<User>());
            Assert.That(response.IsLogged, Is.True);
        }

        [Test]
        public void Login_UserNameAlreadyTaken_UserShouldNotLogin()
        {
            var request = new OpenSessionRequest { UserName = "login1", Token = "123" };
            _roomServiceMock.IsUserLoggedIn(Arg.Any<string>()).Returns(false);
            _roomServiceMock.IsUserNameTaken(Arg.Any<string>()).Returns(true);

            var response = _sessionService.Handle(request);

            Assert.That(response.IsLogged, Is.False);
        }

        [Test]
        public void Login_AlreadyLogged_UserShouldNotLogin()
        {
            var request = new OpenSessionRequest { UserName = "login1", Token = "123" };
            _roomServiceMock.IsUserLoggedIn(Arg.Any<string>()).Returns(true);
            _roomServiceMock.IsUserNameTaken(Arg.Any<string>()).Returns(false);

            var response = _sessionService.Handle(request);

            Assert.That(response.IsLogged, Is.False);
        }
    }
}

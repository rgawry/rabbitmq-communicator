using NSubstitute;
using NUnit.Framework;

namespace Chat
{
    [TestFixture]
    class SessionServiceTest
    {
        IRoomService _roomServiceStub;
        SessionService _sessionService;

        [SetUp]
        public void SetUp()
        {
            _roomServiceStub = Substitute.For<IRoomService>();
            _sessionService = new SessionService(_roomServiceStub);
        }

        [Test]
        public void LogIn_UserNotLoggedAndUserNameNotTaken_UserShouldLogin()
        {
            var request = new OpenSessionRequest { UserName = "login1", Token = "123" };
            _roomServiceStub.IsUserLoggedIn(Arg.Any<string>()).Returns(false);
            _roomServiceStub.IsUserNameTaken(Arg.Any<string>()).Returns(false);

            var response = _sessionService.Login(request);

            _roomServiceStub.Received().AddUser(Arg.Any<User>());
            Assert.That(response.IsLogged, Is.True);
        }

        [Test]
        public void Login_UserNameAlreadyTaken_UserShouldNotLogin()
        {
            var request = new OpenSessionRequest { UserName = "login1", Token = "123" };
            _roomServiceStub.IsUserLoggedIn(Arg.Any<string>()).Returns(false);
            _roomServiceStub.IsUserNameTaken(Arg.Any<string>()).Returns(true);

            var response = _sessionService.Login(request);

            Assert.That(response.IsLogged, Is.False);
        }

        [Test]
        public void Login_AlreadyLogged_UserShouldNotLogin()
        {
            var request = new OpenSessionRequest { UserName = "login1", Token = "123" };
            _roomServiceStub.IsUserLoggedIn(Arg.Any<string>()).Returns(true);
            _roomServiceStub.IsUserNameTaken(Arg.Any<string>()).Returns(false);

            var response = _sessionService.Login(request);

            Assert.That(response.IsLogged, Is.False);
        }
    }
}

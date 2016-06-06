using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Chat
{
    [TestFixture]
    class SessionServiceTest
    {
        [Test]
        public void LogIn_UserNotLoggedAndUserNameNotTaken_UserShouldLogin()
        {
            var request = new OpenSessionRequest { UserName = "login1", Token = "123" };
            var rs = Substitute.For<IRoomService>();
            var ss = new SessionService(rs);
            rs.IsUserLoggedIn(Arg.Any<string>()).Returns(false);
            rs.IsUserNameTaken(Arg.Any<string>()).Returns(false);

            var response = ss.Login(request);

            rs.Received().AddUser(Arg.Any<User>());
            Assert.That(response.IsLogged, Is.True);
        }

        [Test]
        public void Login_UserNameAlreadyTaken_UserShouldNotLogin()
        {
            var request1 = new OpenSessionRequest { UserName = "login1", Token = "123" };
            var rs = Substitute.For<IRoomService>();
            var ss = new SessionService(rs);
            rs.IsUserLoggedIn(Arg.Any<string>()).Returns(false);
            rs.IsUserNameTaken(Arg.Any<string>()).Returns(true);

            var response1 = ss.Login(request1);

            Assert.That(response1.IsLogged, Is.False);
        }

        [Test]
        public void Login_AlreadyLogged_UserShouldNotLogin()
        {
            var request = new OpenSessionRequest { UserName = "login1", Token = "123" };
            var rs = Substitute.For<IRoomService>();
            var ss = new SessionService(rs);
            rs.IsUserLoggedIn(Arg.Any<string>()).Returns(true);
            rs.IsUserNameTaken(Arg.Any<string>()).Returns(false);

            var response = ss.Login(request);

            Assert.That(response.IsLogged, Is.False);
        }

        [Test]
        public void Login_TwoUsersLoggingConcurrently_ShouldLogin()
        {
            var request1 = new OpenSessionRequest { UserName = "login1", Token = "123" };
            var request2 = new OpenSessionRequest { UserName = "login2", Token = "321" };
            var rs = Substitute.For<IRoomService>();
            var ss = new SessionService(rs);
            rs.IsUserLoggedIn(Arg.Any<string>()).Returns(false);
            rs.IsUserNameTaken(Arg.Any<string>()).Returns(false);

            var response1task = Task.Run(() => ss.Login(request1));
            var response2task = Task.Run(() => ss.Login(request2));

            Task.WaitAll(response1task, response2task);

            Assert.That(response1task.Result.IsLogged, Is.True);
            Assert.That(response2task.Result.IsLogged, Is.True);
        }
    }
}

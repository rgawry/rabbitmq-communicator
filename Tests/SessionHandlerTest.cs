using NUnit.Framework;
using System.Threading.Tasks;

namespace Chat
{
    [TestFixture]
    class SessionHandlerTest
    {
        [Test]
        public void Login_UserShouldLogIn()
        {
            var request = new OpenSessionRequest { UserName = "login1", Token = "123" };
            var sh = new SessionHandler();

            var response = sh.Login(request);

            Assert.That(response.IsLogged, Is.True);
        }

        [Test]
        public void Login_UserShouldNotLoginWhenUserNameAlreadyTaken()
        {
            var request1 = new OpenSessionRequest { UserName = "login1", Token = "123" };
            var request2 = new OpenSessionRequest { UserName = "login1", Token = "321" };
            var sh = new SessionHandler();

            var response1 = sh.Login(request1);
            var response2 = sh.Login(request2);

            Assert.That(response1.IsLogged, Is.True);
            Assert.That(response2.IsLogged, Is.False);
        }

        [Test]
        public void Login_UserShouldNotLogInWhenAlreadyLogged()
        {
            var request = new OpenSessionRequest { UserName = "login1", Token = "123" };
            var sh = new SessionHandler();

            var response1 = sh.Login(request);
            var response2 = sh.Login(request);

            Assert.That(response1.IsLogged, Is.True);
            Assert.That(response2.IsLogged, Is.False);
        }

        [Test]
        public void Login_TwoUsersShouldLoginConcurrently()
        {
            var request1 = new OpenSessionRequest { UserName = "login1", Token = "123" };
            var request2 = new OpenSessionRequest { UserName = "login2", Token = "321" };
            var sh = new SessionHandler();

            var response1task = Task.Run(() => sh.Login(request1));
            var response2task = Task.Run(() => sh.Login(request2));

            Task.WaitAll(response1task, response2task);

            Assert.That(response1task.Result.IsLogged, Is.True);
            Assert.That(response2task.Result.IsLogged, Is.True);
        }

        [Test]
        public void JoinRoom_UserShouldSwitchRoom()
        {
            var userName = "login1";
            var roomName1 = "room1";
            var roomName2 = "room2";
            var token = "123";
            var request1 = new JoinRoomRequest { RoomName = roomName1, Token = token };
            var request2 = new JoinRoomRequest { RoomName = roomName2, Token = token };
            var sh = new SessionHandler();

            sh.Login(new OpenSessionRequest { UserName = userName, Token = token });
            Assert.That(sh.GetUserRoom(token), Is.EqualTo("default"));
            sh.JoinRoom(request1);
            Assert.That(sh.GetUserRoom(token), Is.EqualTo(roomName1));
            sh.JoinRoom(request2);
            Assert.That(sh.GetUserRoom(token), Is.EqualTo(roomName2));
        }
    }
}

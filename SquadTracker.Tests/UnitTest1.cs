using NUnit.Framework;

namespace Torlando.SquadTracker.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void Test1()
        {
            var test = RolesPersister.LoadRolesFromFileSystem("");
            Assert.IsTrue(true);
        }
    }
}
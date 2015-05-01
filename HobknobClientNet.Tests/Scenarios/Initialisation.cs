using NUnit.Framework;

namespace HobknobClientNet.Tests.Scenarios
{
    [TestFixture]
    internal class Initialisation : TestBase
    {

        [SetUp]
        public void SetUp()
        {
            Set_application_name("app1");
        }

        [Test]
        public void Initialises_happy_path()
        {
            Given_a_toggle("app1", "feature1", "true");
            HobknobClient = Create_hobknob_client();
        }

        [Test]
        public void Initialises_ok_if_application_exists_but_has_no_features()
        {
            Given_a_toggle("app1", "feature1", "true");
            Given_a_toggle_is_removed("app1", "feature1");

            HobknobClient = Create_hobknob_client();
        }

        [Test]
        public void Initialises_ok_if_application_does_not_exist()
        {
            Set_application_name("app1234");
            HobknobClient = Create_hobknob_client();
        }

        [Test]
        public void Throws_exception_on_network_error()
        {
            
            Assert.That(() => HobknobClient = Create_hobknob_client("bad_host"),
                Throws.Exception.Message.Contains("host"));
        }
    }
}

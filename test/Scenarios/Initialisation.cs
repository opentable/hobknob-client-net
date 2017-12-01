using System;
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
            HobknobClient = Create_hobknob_client(Create_exception_throwing_error_handler());
        }

        [Test]
        public void Initialises_ok_if_application_exists_but_has_no_features()
        {
            Given_a_toggle("app1", "feature1", "true");
            Given_a_toggle_is_removed("app1", "feature1");

            HobknobClient = Create_hobknob_client(Create_exception_throwing_error_handler());
        }

        [Test]
        public void Initialises_ok_if_application_does_not_exist()
        {
            Set_application_name("app1234");
            HobknobClient = Create_hobknob_client(Create_exception_throwing_error_handler());
        }

        [Test]
        public void Throws_exception_if_error_handler_is_null()
        {            
            Assert.That(() => HobknobClient = Create_hobknob_client(null),
                Throws.Exception.Message.Contains("cacheUpdateFailed"));
        }

        [Test]
        public void Initialises_ok_on_network_error_and_error_handler_gets_called()
        {
            var errorsCounter = 0;            
            HobknobClient = Create_hobknob_client((sender, eventArgs) => { errorsCounter++; }, "bad_host");
            Assert.That(errorsCounter, Is.EqualTo(1));
        }
    }
}

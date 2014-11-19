using NUnit.Framework;

namespace HobknobClientNet.Tests.Scenarios
{
    [TestFixture]
    internal class Get : TestBase
    {
        bool? _toggleValue;

        [SetUp]
        public void SetUp()
        {
            Set_application_name("app1");
            _toggleValue = null;
        }

        [Test]
        public void Simple_get_true()
        {
            Given_a_toggle("app1", "toggle1", "true");
            When_I_get("toggle1", out _toggleValue);
            Assert.That(_toggleValue, Is.True);
        }

        [Test]
        public void Simple_get_false()
        {
            Given_a_toggle("app1", "toggle1", "false");
            When_I_get("toggle1", out _toggleValue);
            Assert.That(_toggleValue, Is.False);
        }

        [Test]
        public void Get_throws_exception_when_key_does_not_exist()
        {
            Assert.That(() => When_I_get("toggle3", out _toggleValue), Throws.Exception);
        }

        [Test]
        public void Applications_do_not_clash()
        {
            Given_a_toggle("app1", "toggle1", "true");
            Given_a_toggle("app2", "toggle1", "false");
            When_I_get("toggle1", out _toggleValue);
            Assert.That(_toggleValue, Is.True);
        }

        [Test]
        public void Bad_etcd_value_throws_exception()
        {
            Given_a_toggle("app1", "toggle1", "bad");
            Assert.That(() => When_I_get("toggle1", out _toggleValue), Throws.Exception);
        }
    }
}

using NUnit.Framework;

namespace HobknobClientNet.Tests.Scenarios
{
    [TestFixture]
    internal class GetOrDefault_SimpleFeature : TestBase
    {
        bool? _toggleValue;

        [SetUp]
        public void SetUp()
        {
            Set_application_name("app1");
            _toggleValue = null;
        }

        [Test]
        public void Default_not_used_when_key_exists()
        {
            Given_a_toggle("app1", "feature1", "true");
            When_I_get_with_default("feature1", false, out _toggleValue);
            Assert.That(_toggleValue, Is.True);
        }

        [Test]
        public void Default_is_used_when_key_does_not_exist()
        {
            When_I_get_with_default("feature3", true, out _toggleValue);
            Assert.That(_toggleValue, Is.True);
        }

        [Test]
        public void Applications_do_not_clash()
        {
            Given_a_toggle("app1", "feature1", "true");
            Given_a_toggle("app2", "feature1", "false");
            When_I_get_with_default("feature1", false, out _toggleValue);
            Assert.That(_toggleValue, Is.True);
        }
    }
}

using System;
using System.Threading;
using NUnit.Framework;

namespace HobknobClientNet.Tests.Scenarios
{
    [TestFixture]
    public class CacheUpdates : TestBase
    {
        bool? _toggleValue;

        [SetUp]
        public void SetUp()
        {
            Set_cache_update_interval(TimeSpan.FromSeconds(1));
            Set_application_name("app1");
            _toggleValue = null;
        }

        [Test]
        public void Cache_is_not_updated_when_update_interval_is_not_passed()
        {
            Given_a_toggle("app1", "toggle1", "true");
            When_I_get("toggle1", out _toggleValue);
            Assert.That(_toggleValue, Is.True);

            Given_a_toggle("app1", "toggle1", "false");

            When_I_get_without_initialising_a_new_hobknob_instance("toggle1", out _toggleValue);
            Assert.That(_toggleValue, Is.True);
        }

        [Test]
        public void Cache_is_updated_when_update_interval_is_passed()
        {
            Given_a_toggle("app1", "toggle1", "true");
            When_I_get("toggle1", out _toggleValue);
            Assert.That(_toggleValue, Is.True);

            Given_a_toggle("app1", "toggle1", "false");
            Thread.Sleep(TimeSpan.FromSeconds(1.2));

            When_I_get_without_initialising_a_new_hobknob_instance("toggle1", out _toggleValue);
            Assert.That(_toggleValue, Is.False);
        }
    }
}

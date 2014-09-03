using System;
using System.Collections.Generic;
using System.Linq;
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
            TearDown();
            Set_cache_update_interval(TimeSpan.FromSeconds(1));
            Set_application_name("cacheUpdateTest");
            _toggleValue = null;
        }

        [TearDown]
        public void TearDown()
        {
            EtcdClient.DeleteDir("v1/toggles/cacheUpdateTest", true);
        }

        [Test]
        public void Cache_is_not_updated_when_update_interval_is_not_passed()
        {
            Given_a_toggle("cacheUpdateTest", "toggle1", "true");
            When_I_get("toggle1", out _toggleValue);
            Assert.That(_toggleValue, Is.True);

            Given_a_toggle("cacheUpdateTest", "toggle1", "false");

            When_I_get_without_initialising_a_new_hobknob_instance("toggle1", out _toggleValue);
            Assert.That(_toggleValue, Is.True);
        }

        [Test]
        public void Cache_is_updated_when_update_interval_is_passed()
        {
            Given_a_toggle("cacheUpdateTest", "toggle1", "true");
            When_I_get("toggle1", out _toggleValue);
            Assert.That(_toggleValue, Is.True);

            Given_a_toggle("cacheUpdateTest", "toggle1", "false");
            Thread.Sleep(TimeSpan.FromSeconds(1.2));

            When_I_get_without_initialising_a_new_hobknob_instance("toggle1", out _toggleValue);
            Assert.That(_toggleValue, Is.False);
        }

        [Test]
        public void Cache_updated_information_is_correct()
        {
            Given_a_toggle("cacheUpdateTest", "existingNoChange", "true");
            Given_a_toggle("cacheUpdateTest", "existingChange", "true");
            Given_a_toggle("cacheUpdateTest", "existingRemoved", "true");

            var hobknobClient = Create_hobknob_client();

            Given_a_toggle("cacheUpdateTest", "newToggle", "true");
            Given_a_toggle("cacheUpdateTest", "existingChange", "false");
            Given_a_toggle_is_removed("cacheUpdateTest", "existingRemoved");

            CacheUpdatedArgs cacheUpdatedArgs = null;
            hobknobClient.CacheUpdated += (sender, args) =>
            {
                if (cacheUpdatedArgs == null)
                {
                    cacheUpdatedArgs = args;
                } 
            };

            // wait for client to update
            Thread.Sleep(TimeSpan.FromSeconds(1.2));

            Assert.That(cacheUpdatedArgs, Is.Not.Null);
            var cacheUpdates = cacheUpdatedArgs.Updates.ToDictionary(x => x.Key);
            Assert.That(cacheUpdates.Count, Is.EqualTo(3));
            AssertToggleUpdate(cacheUpdates, "existingChange", true, false);
            AssertToggleUpdate(cacheUpdates, "existingRemoved", true, null);
            AssertToggleUpdate(cacheUpdates, "newToggle", null, true);
        }

        private void AssertToggleUpdate(Dictionary<string, CacheUpdate> updates, string key, bool? oldValue, bool? newValue)
        {
            var cacheUpdate = updates[key];
            Assert.That(cacheUpdate.OldValue, Is.EqualTo(oldValue));
            Assert.That(cacheUpdate.NewValue, Is.EqualTo(newValue));
        }
    }
}


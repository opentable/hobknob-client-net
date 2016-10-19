using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace HobknobClientNet.Tests.Scenarios
{
    [TestFixture]
    internal class CacheUpdates : TestBase
    {
        private const string ApplicationName = "cacheUpdateTest";
        bool? _toggleValue;

        [SetUp]
        public void SetUp()
        {
            TearDown();
            Set_cache_update_interval(TimeSpan.FromSeconds(1));
            Set_application_name(ApplicationName);
            _toggleValue = null;
        }

        [TearDown]
        public void TearDown()
        {
            EtcdClient.DeleteDir(new Uri("v1/toggles/cacheUpdateTest/", UriKind.Relative));
        }

        [Test]
        public void Cache_is_not_updated_when_update_interval_is_not_passed()
        {
            Given_a_toggle(ApplicationName, "feature1", "true");
            When_I_get_with_default("feature1", false, out _toggleValue);
            Assert.That(_toggleValue, Is.True);

            Given_a_toggle(ApplicationName, "feature1", "false");

            When_I_get_with_default_without_initialising_a_new_hobknob_instance("feature1", out _toggleValue);
            Assert.That(_toggleValue, Is.True);
        }

        [Test]
        public void Cache_is_updated_when_update_interval_is_passed()
        {
            Given_a_toggle(ApplicationName, "feature1", "true");
            When_I_get_with_default("feature1", false, out _toggleValue);
            Assert.That(_toggleValue, Is.True);

            Given_a_toggle(ApplicationName, "feature1", "false");
            Thread.Sleep(TimeSpan.FromSeconds(1.2));

            When_I_get_with_default_without_initialising_a_new_hobknob_instance("feature1", out _toggleValue);
            Assert.That(_toggleValue, Is.False);
        }

        [Test]
        public void Cache_updated_information_is_correct()
        {
            Given_a_toggle(ApplicationName, "existingNoChange", "true");
            Given_a_toggle(ApplicationName, "existingChange", "true");
            Given_a_toggle(ApplicationName, "existingRemoved", "true");

            var hobknobClient = Create_hobknob_client(Create_exception_throwing_error_handler());

            Given_a_toggle(ApplicationName, "newToggle", "true");
            Given_a_toggle(ApplicationName, "existingChange", "false");
            Given_a_toggle_is_removed(ApplicationName, "existingRemoved");

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
            var cacheUpdate = updates["/v1/toggles/" + ApplicationName + "/" + key];
            Assert.That(cacheUpdate.OldValue, Is.EqualTo(oldValue));
            Assert.That(cacheUpdate.NewValue, Is.EqualTo(newValue));
        }
    }
}


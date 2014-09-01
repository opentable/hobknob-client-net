using NUnit.Framework;

namespace HobknobClientNet.Tests
{
    [Scenario]
    public class SimpleGet : TestBase
    {
        [Given]
        public void A_feature_toggle_exists_for_an_application()
        {
            EtcdClient.Set("v1/toggles/app1/toggle1", "true");
        }

        [When]
        public bool I_get_the_feature_toggle()
        {
            return HobknobClient.Get("toggle1");
        }

        [Then]
        public void The_value_is_correct()
        {
            Assert.That(FeatureToggleValue, Is.True);
        }
    }
}

using System;
using System.Linq;
using etcetera;
using NUnit.Framework;

namespace HobknobClientNet.Tests
{
    public class TestBase
    {
        protected readonly EtcdClient EtcdClient;
        protected HobknobClient HobknobClient;
        protected bool FeatureToggleValue;

        protected TestBase()
        {
            EtcdClient = new EtcdClient(new Uri("http://localhost:4001/v2/keys/"));
        }

        [TearDown]
        public void TearDown()
        {
            if (HobknobClient != null)
            {
                HobknobClient.Dispose();
            }
        }

        [SetUp]
        public void SetUp()
        {
            HobknobClient = new HobknobClientFactory().Create("localhost", 4001, "app1", TimeSpan.FromMinutes(1));
            FeatureToggleValue = false;

            var methods = GetType().GetMethods();
            var givenMethod = methods.SingleOrDefault(x => x.GetCustomAttributes(typeof(GivenAttribute), true).Any());
            var whenMethod = methods.SingleOrDefault(x => x.GetCustomAttributes(typeof(WhenAttribute), true).Any());

            if (givenMethod != null)
            {
                givenMethod.Invoke(this, new object[0]);
            }

            if (whenMethod != null)
            {
                if (whenMethod.ReturnType == typeof(bool))
                {
                    FeatureToggleValue = (bool)whenMethod.Invoke(this, new object[0]);
                }
                else
                {
                    whenMethod.Invoke(this, new object[0]);
                }
            }
        }
    }

    public class ScenarioAttribute : TestFixtureAttribute { }
    public class GivenAttribute : Attribute { }
    public class WhenAttribute : Attribute { }
    public class ThenAttribute : TestAttribute { }

}

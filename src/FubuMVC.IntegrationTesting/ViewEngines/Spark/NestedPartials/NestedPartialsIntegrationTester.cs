using FubuMVC.Core;
using FubuMVC.IntegrationTesting.Conneg;
using FubuMVC.TestingHarness;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.ViewEngines.Spark.NestedPartials
{
    [TestFixture]
    public class NestedPartialsIntegrationTester : SharedHarnessContext
    {
        [Test]
        public void partials_nest_deeply_1()
        {
            endpoints.Get<FamilyEndpoint>(x => x.Marcus(null)).ReadAsText().RemoveNewlines()
                .ShouldContain("<div>Marcus-><div>Silvia</div></div>");
        }

        [Test]
        public void partials_nest_deeply_2()
        {
            endpoints.Get<FamilyEndpoint>(x => x.Jack(null)).ReadAsText().RemoveNewlines()
                .ShouldContain("<div>Jack-><div>Marcus-><div>Silvia</div></div></div>");
        }

        [Test]
        public void partials_nest_deeply_3()
        {
            endpoints.Get<FamilyEndpoint>(x => x.George(null)).ReadAsText().RemoveNewlines()
                .ShouldContain("<div>George-><div>Jack-><div>Marcus-><div>Silvia</div></div></div></div>");
        }
    }
}
using System.Net;
using FubuMVC.Core.Assets.Files;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.IntegrationTesting.Assets
{
    [TestFixture]
    public class return_404_when_asset_cannot_be_found : SharedHarnessContext
    {
        [Test]
        public void look_for_nonexistent_image()
        {
            endpoints.GetAsset(AssetFolder.images, "nonexistent.png")
                .StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Test]
        public void look_for_nonexistent_script()
        {
            endpoints.GetAsset(AssetFolder.scripts, "nonexistent.js")
                .StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Test]
        public void look_for_nonexistent_style()
        {
            endpoints.GetAsset(AssetFolder.styles, "nonexistent.css")
                .StatusCode.ShouldEqual(HttpStatusCode.NotFound); 
        }
    }
}
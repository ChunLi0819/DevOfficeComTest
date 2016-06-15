using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestFramework;

namespace MSGraphTest
{
    /// <summary>
    /// Test Class for Microsoft Graph site
    /// </summary>
    [TestClass]
    public class MSGraphNavigationTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            GraphBrowser.Initialize();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            GraphBrowser.Close();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            GraphBrowser.Goto(GraphBrowser.BaseAddress);
        }

        /// <summary>
        /// Verify whether Home page can be navigated to by clicking the branding image.
        /// </summary>
        [TestMethod]
        public void BVT_Graph_S01_TC01_CanBrandingNavToHomePage()
        {
            GraphPages.Navigation.Select("Home");
            //Currently ignore the Graph explorer, since this page desn't have Microsoft
            //Graph branding image
            string[] navOptions = new string[] { 
                "Home",
                "Get started", 
                "Documentation", 
                //"Graph explorer", 
                "App registration", 
                "Samples & SDKs", 
                "Changelog" };
            string navPage = navOptions[new Random().Next(navOptions.Length)];
            GraphPages.Navigation.Select(navPage);

            GraphUtility.ClickBranding();

            Assert.IsTrue(
                GraphPages.Navigation.IsAtGraphPage("Home"),
                @"Clicking the branding image should navigate to Graph Home Page");
        }

        /// <summary>
        /// Verify whether Get started page can be navigated to.
        /// </summary>
        [TestMethod]
        public void BVT_Graph_S01_TC02_CanGoToGetstartedPage()
        {
            string title = GraphPages.Navigation.Select("Get started");
            Assert.IsTrue(
                GraphPages.Navigation.IsAtGraphPage(title),
                @"The opened page should be {0}",
                title);
        }

        /// <summary>
        /// Verify whether Documentation page can be navigated to.
        /// </summary>
        [TestMethod]
        public void BVT_Graph_S01_TC03_CanGoToDocumentationPage()
        {
            string title = GraphPages.Navigation.Select("Documentation");
            Assert.IsTrue(
                GraphPages.Navigation.IsAtGraphPage(title),
                @"The opened page should be {0}",
                title);
        }

        /// <summary>
        /// Verify whether Graph explorer page can be navigated to.
        /// </summary>
        [TestMethod]
        public void BVT_Graph_S01_TC04_CanGoToGraphExplorerPage()
        {
            string title = TestHelper.VerifyAndSelectExplorerOnNavBar();
            Assert.IsTrue(
                GraphPages.Navigation.IsAtGraphPage(title),
                @"The opened page should be ""Graph explorer""");
        }

        /// <summary>
        /// Verify whether App Registration page can be navigated to.
        /// </summary>        
        [TestMethod]
        public void BVT_Graph_S01_TC05_CanGoToAppRegistrationPage()
        {
            string title = GraphPages.Navigation.Select("App registration");
            Assert.IsTrue(
                GraphPages.Navigation.IsAtGraphPage(title),
                @"The opened page should be {0}",
                title);
        }

        /// <summary>
        /// Verify whether Samples and SDKs page can be navigated to.
        /// </summary>
        [TestMethod]
        public void BVT_Graph_S01_TC06_CanGoToSamplesAndSDKsPage()
        {
            string title = GraphPages.Navigation.Select("Samples & SDKs");
            Assert.IsTrue(
                GraphPages.Navigation.IsAtGraphPage(title),
                @"The opened page should be {0}",
                title);
        }

        /// <summary>
        /// Verify whether Changelog page can be navigated to.
        /// </summary>        
        [TestMethod]
        public void BVT_Graph_S01_TC07_CanGoToChangelogPage()
        {
            string title = GraphPages.Navigation.Select("Changelog");
            Assert.IsTrue(
                GraphPages.Navigation.IsAtGraphPage(title),
                @"The opened page should be ""Changelog""");
        }

        /// <summary>
        /// Verify whether the default banner image can be loaded.
        /// </summary>
        [TestMethod]
        public void BVT_Graph_S01_TC08_CanLoadBannerImage()
        {
            //Currently ignore Graph explorer and Documentation, since these pages don't have banner image
            //Graph branding image
            string[] navOptions = new string[] { 
                "Home",
                "Get started", 
                //"Documentation", 
                //"Graph explorer", 
                "App registration", 
                "Samples & SDKs", 
                "Changelog" };

            string navPage = navOptions[new Random().Next(navOptions.Length)];
            GraphPages.Navigation.Select(navPage);
            string imageUrl = GraphUtility.GetGraphBannerImageUrl();
           
            Assert.IsTrue(GraphUtility.FileExist(imageUrl), "The banner image should be valid to load");
        }

        /// <summary>
        /// Verify whether the default banner image can be loaded.
        /// </summary>
        [TestMethod]
        public void BVT_Graph_S01_TC09_CanLoadGraphPageImages()
        {
            //Currently ignore Graph explorer and Documentation, since these pages don't have banner image
            //Graph branding image
            string[] navOptions = new string[] {
                //"Home",
                "Get started", 
                //"Documentation", 
                //"Graph explorer", 
                "App registration",
                "Samples & SDKs",
                "Changelog" };

            foreach (string navPage in navOptions)
            {
                GraphPages.Navigation.Select(navPage);
                if (navPage == "Home")
                {
                    foreach (GraphPageImages item in Enum.GetValues(typeof(GraphPageImages)))
                    {
                        Assert.IsTrue(GraphPages.HomePage.CanLoadImages(item));
                    }
                }
                else
                {
                    var graphPage = new GraphPage();
                    foreach (GraphPageImages item in Enum.GetValues(typeof(GraphPageImages)))
                    {
                        Assert.IsTrue(graphPage.CanLoadImages(item));
                    }
                }
            }
        }
    }
}

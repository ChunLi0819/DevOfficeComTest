using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestFramework;

namespace MSGraphTest
{
    /// <summary>
    /// Test Class for Microsoft Graph Home page
    /// </summary>
    [TestClass]
    public class MSGraphHomePageTest
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
        /// Verify whether Home page can be navigated to.
        /// </summary>
        [TestMethod]
        public void BVT_Graph_S02_TC01_CanGoToHomePage()
        {
            string title = GraphPages.Navigation.Select("Home");
            Assert.IsTrue(
                GraphPages.Navigation.IsAtGraphPage(title),
                @"The opened page should be {0} when clicking it",
                title);

            //Currently ignore the Graph explorer, since this page desn't have Microsoft
            //MS Graph nav bar
            string[] navOptions = new string[] { 
                "Get started", 
                "Documentation", 
                //"Graph explorer", 
                "App registration", 
                "Samples & SDKs", 
                "Changelog" };

            //Go to the other page to click "Home" on nav bar
            string navPage = navOptions[new Random().Next(navOptions.Length)];
            GraphPages.Navigation.Select(navPage);

            title = GraphPages.Navigation.Select("Home");
            Assert.IsTrue(
                GraphPages.Navigation.IsAtGraphPage(title),
                @"The opened page should be {0} when clicking it {1} page's nav bar",
                title,
                navPage);
        }

        /// <summary>
        /// Verify whether Overview is shown when "See overview" is clicked.
        /// </summary>
        [TestMethod]
        public void Acceptance_Graph_S02_TC02_ClickSeeOverviewCanShowDocumentaionPage()
        {
            GraphUtility.SelectToSeeOverView();
            bool isOverview = GraphUtility.ValidateDocument(GraphBrowser.BaseAddress + "/docs/overview/overview");
            string docTitle = GraphUtility.GetDocTitle();
            Assert.IsTrue(
                isOverview,
                "The documentation should be {0} when clicking See overview on Home page",
                docTitle);
        }

        /// <summary>
        /// Verify whether Overview is shown when "Try the API" is clicked.
        /// </summary>
        [TestMethod]
        public void Acceptance_Graph_S02_TC03_ClickTryAPIOnExplorerCanShowPage()
        {
            string explorerTitle = TestHelper.VerifyAndSelectExplorerOnNavBar();
            GraphBrowser.GoBack();

            GraphUtility.SelectToTryAPI();
            Assert.IsTrue(
                GraphBrowser.SwitchToWindow(explorerTitle),
                @"The opened page should be ""{0}"" when clicking Try the API",
                explorerTitle);
            GraphBrowser.SwitchBack();
        }
    }
}
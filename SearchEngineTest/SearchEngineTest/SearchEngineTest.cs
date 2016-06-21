using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace SearchEngineTest
{
    [TestClass]
    public class SearchEngineTest
    {
        static string searchEngine = ConfigurationManager.AppSettings["SearchEngine"].ToLower();
        public static IWebDriver WebDriver;
        static int lowestRanking = Int32.Parse(ConfigurationManager.AppSettings["LowestRanking"]);
        static string baseAddress;
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            switch (ConfigurationManager.AppSettings["Browser"].ToLower())
            {
                case ("chrome"):
                    WebDriver = new ChromeDriver(System.IO.Directory.GetCurrentDirectory() + @"/Drivers/");
                    break;
                case ("ie32"):
                    WebDriver = new InternetExplorerDriver(System.IO.Directory.GetCurrentDirectory() + @"/Drivers/IE32/");
                    break;
                case ("ie64"):
                    WebDriver = new InternetExplorerDriver(System.IO.Directory.GetCurrentDirectory() + @"/Drivers/IE64/", new InternetExplorerOptions() { RequireWindowFocus = true });
                    break;
                case ("firefox"):
                default:
                    WebDriver = new FirefoxDriver();
                    break;
            }

            int waitTime = Int32.Parse(ConfigurationManager.AppSettings["WaitTime"]);
            WebDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(waitTime));
            WebDriver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(waitTime));
            if (searchEngine.Equals("bing"))
        {
            baseAddress = "http://www.bing.com";
            }
            else if (searchEngine.Equals("google"))
            {
                baseAddress = "http://www.google.com/ncr";
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            WebDriver.Quit();
        }
        /// <summary>
        /// Verify whether Graph site can be found on search engine
        /// </summary>
        [TestMethod]
        public void BVT_S01_TC01_CanFindGraphSite()
        {
            WebDriver.Navigate().GoToUrl(baseAddress);
            int ranking;

            DateTime time = DateTime.Now;
            bool isFounded = CanFindSiteinSearchResults(SearchSite.MSGraph,
                "graph.microsoft.io",
                out ranking);

            Trace.WriteLine(String.Format("Keywords:{0} Ranking:{1}", GetDescription(SearchSite.MSGraph), ranking + 1));
            Assert.IsTrue(isFounded,
                "{0}: The result{1} Microsoft Graph production site on {2} when searching {3}.",
                time.ToString(),
                isFounded ? String.Format(" at position {0} is", ranking + 1) : "s in top 5 don't contain",
                searchEngine,
                GetDescription(SearchSite.MSGraph));

            WebDriver.Navigate().Back();
            time = DateTime.Now;
            isFounded = CanFindSiteinSearchResults(SearchSite.MSGraphAPI,
                "graph.microsoft.io",
                out ranking);

            Trace.WriteLine(String.Format("Keywords:{0} Ranking:{1}", GetDescription(SearchSite.MSGraphAPI), ranking + 1));
            Assert.IsTrue(isFounded,
                "{0}: The result{1} Microsoft Graph production site on {2} when searching {3}.",
                time.ToString(),
                isFounded ? String.Format(" at position {0} is", ranking + 1) : "s in top 5 don't contain",
                searchEngine,
                GetDescription(SearchSite.MSGraphAPI));

            WebDriver.Navigate().Back();
            time = DateTime.Now;
            isFounded = CanFindSiteinSearchResults(SearchSite.GraphMS,
                "graph.microsoft.io",
                out ranking);

            Trace.WriteLine(String.Format("Keywords:{0} Ranking:{1}", GetDescription(SearchSite.GraphMS), ranking + 1));
            Assert.IsTrue(isFounded,
                "{0}: The result{1} Microsoft Graph production site on {2} when searching {3}.",
                time.ToString(),
                isFounded ? String.Format(" at position {0} is", ranking + 1) : "s in top 5 don't contain",
                searchEngine,
                GetDescription(SearchSite.GraphMS));

            WebDriver.Navigate().Back();
        }

        /// <summary>
        /// Verify whether dev.office.com site can be found on search engine
        /// </summary>
        [TestMethod]
        public void BVT_S01_TC02_CanFindOfficeDevCenterSite()
        {
            WebDriver.Navigate().GoToUrl(baseAddress);
            int ranking;

            DateTime time = DateTime.Now;
            bool isFounded = CanFindSiteinSearchResults(SearchSite.OfficeDevCenter,
                "dev.office.com",
                out ranking);

            Trace.WriteLine(String.Format("Keywords:{0} Ranking:{1}", GetDescription(SearchSite.OfficeDevCenter), ranking + 1));
            Assert.IsTrue(isFounded,
                "{0}: The result {1} Office Dev Center production site on {2} when searching {3}.",
                time.ToString(),
                isFounded ? String.Format(" at position {0} is", ranking + 1) : "s in top 5 don't contain",
                searchEngine,
                GetDescription(SearchSite.OfficeDevCenter));

            WebDriver.Navigate().Back();
            time = DateTime.Now;
            isFounded = CanFindSiteinSearchResults(SearchSite.DevOffice,
                "dev.office.com",
                out ranking);

            Trace.WriteLine(String.Format("Keywords:{0} Ranking:{1}", GetDescription(SearchSite.DevOffice), ranking + 1));
            Assert.IsTrue(isFounded,
                "{0}: The result {1} Office Dev Center production site on {2} when searching {3}.",
                time.ToString(),
                isFounded ? String.Format(" at position {0} is", ranking + 1) : "s in top 5 don't contain",
                searchEngine,
                GetDescription(SearchSite.DevOffice));

            WebDriver.Navigate().Back();
            time = DateTime.Now;
            isFounded = CanFindSiteinSearchResults(SearchSite.OfficeAddins,
                "dev.office.com",
                out ranking);

            Trace.WriteLine(String.Format("Keywords:{0} Ranking:{1}", GetDescription(SearchSite.OfficeAddins), ranking + 1));
            Assert.IsTrue(isFounded,
                "{0}: The result {1} Office Dev Center production site on {2} when searching {3}.",
                time.ToString(),
                isFounded ? String.Format(" at position {0} is", ranking + 1) : "s in top 5 don't contain",
                searchEngine,
                GetDescription(SearchSite.OfficeAddins));

            WebDriver.Navigate().Back();
        }

        /// <summary>
        /// Verify whether the ranking of the site search result is acceptable
        /// </summary>
        /// <param name="ranking">The actual ranking</param>
        /// <returns>True if ranking is less than lowestRanking, else no.</returns>
        private static bool CanFindSiteinSearchResults(SearchSite item, string link, out int ranking)
        {
            ranking = 0;
            bool isFounded = false;
            List<SearchedResult> searchedResults = Search(item);
            foreach (SearchedResult result in searchedResults)
            {
                if (ranking >= lowestRanking) break;
                if (result.DetailLink.Contains(link))
                {
                    isFounded = true;
                    break;
                }
                ranking++;
            }
            return isFounded;
        }

        /// <summary>
        /// Search production site
        /// </summary>
        /// <param name="searchSite">Production Site to search</param>
        /// <returns>Searched results</returns>
        public static List<SearchedResult> Search(SearchSite searchSite)
        {
            string keyWord = GetDescription(searchSite);
            int waitTime = Int32.Parse(ConfigurationManager.AppSettings["WaitTime"]);
            var wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(waitTime));
            int inputCount = WebDriver.FindElements(By.TagName("input")).Count;
            for (int i = 0; i < inputCount; i++)
            {
                IWebElement element = WebDriver.FindElements(By.TagName("input"))[i];

                if (element.Enabled && element.Displayed)
                {
                    element.Clear();
                    element.SendKeys(keyWord + "\n");
                    break;
                }
            }

            List<SearchedResult> searchedResults = new List<SearchedResult>();

            switch (searchEngine)
            {
                case "google":
                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div>h3>a")));
                    searchedResults = GetSearchResults("div>h3>a");
                    break;
                case "bing":
                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("li>h2>a")));
                    searchedResults = GetSearchResults("li>h2>a");
                    break;
                default:
                    searchedResults = null;
                    break;
            }
            return searchedResults;
        }

        /// <summary>
        /// Get search results on result page
        /// </summary>
        /// <param name="selectorString">The Css selector string of result elements</param>
        /// <returns>Searched results</returns>
        private static List<SearchedResult> GetSearchResults(string selectorString)
        {
            List<SearchedResult> searchedResults = new List<SearchedResult>();

            int resultCount = WebDriver.FindElements(By.CssSelector(selectorString)).Count;
            int range = resultCount > lowestRanking ? lowestRanking : resultCount;
            for (int i = 0; i < range; i++)
            {
                SearchedResult result = new SearchedResult();
                result.Name = WebDriver.FindElements(By.CssSelector(selectorString))[i].GetAttribute("innerHTML");
                result.DetailLink = WebDriver.FindElements(By.CssSelector(selectorString))[i].GetAttribute("href");
                searchedResults.Add(result);
            }
            return searchedResults;
        }

        /// <summary>
        /// Get the Description attribute of an enum value
        /// </summary>
        /// <param name="value">The enum value</param>
        /// <returns>Description attribute</returns>
        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            System.ComponentModel.DescriptionAttribute[] attributes =
                  (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(
                  typeof(System.ComponentModel.DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
    }
}

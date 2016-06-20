using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestFramework;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MSGraphTest
{
    [TestClass]
    public class MSGraphSearchTest
    {
        private string hostName = "http://msgraph-staging-localization.azurewebsites.net";
        private string path = "Search/localfiles?q=authentication&target=docs-local";
        private string[] languages = { "/en-us/", "/de-de/", "/ja-jp/", "/zh-cn/" };

        #region Initiaize/cleanup
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

        #endregion

        #region Test cases
        //[TestMethod]
        //public void BVT_Graph_S08_TC01_VerifyLocalDocsSearchService()
        //{
        //    foreach (string language in this.languages)
        //    {
        //        string url = this.hostName + language + path;
        //        GraphBrowser.Goto(url);
        //        JArray searchResults = this.GetRequestResult(url);
        //        Assert.IsTrue(searchResults.Count > 0, "Search result didnt return any result", url);

        //        foreach (var searchResult in searchResults)
        //        {
        //            url = (string)searchResult["Url"];
        //            Assert.IsTrue(url.StartsWith(language), "The searched results should be culture-specific");
        //            // get the url from the search result and invoke, to make sure that search links are workng fine
        //            Assert.IsTrue(GraphUtility.CheckUrl(this.hostName + url), "The url in search result is not valid");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Verify whether input of the search keyword on Home page can get the correct results
        ///// </summary>
        //[TestMethod]
        //public void BVT_Graph_S08_TC02_CanSearchLocalDocs()
        //{
        //    foreach (string language in this.languages)
        //    {
        //        GraphBrowser.Goto(this.hostName + language);
        //        List<SearchedResult> searchResults = GraphUtility.SearchText("Authorization");
        //        if (searchResults.Count == 0)
        //        {
        //            Assert.Inconclusive("Search on {0} didn't return any result", this.hostName + language);
        //        }

        //        foreach (var searchResult in searchResults)
        //        {
        //            string docLink = (string)searchResult.DetailLink;
        //            Assert.IsTrue(docLink.StartsWith(this.hostName + language), "The searched results should be culture-specific");
        //            // get the url from the search result and invoke, to make sure that search links are workng fine
        //            Assert.IsTrue(GraphUtility.CheckUrl(docLink), "The url in search result is not valid");
        //        }
        //    }
        //}
        #endregion

        private Newtonsoft.Json.Linq.JArray GetRequestResult(string url)
        {
            JArray data = new JArray();
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.SendAsync(request).Result;
                Assert.IsTrue(response.IsSuccessStatusCode, "service didn't return success", url);
                var resMsg = response.Content.ReadAsStringAsync().Result;

                data = (JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(resMsg);
            }
            return data;
        }
    }
}

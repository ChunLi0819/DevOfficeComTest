using System;
using System.Collections.Generic;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace TestFramework
{
    public class GraphHomePage : GraphBasePage
    {
        public GraphHomePage()
        { }
        private static string PageTitle = "Microsoft Graph - Home";

        [FindsBy(How = How.XPath, Using = "//ul[@class='nav navbar-nav']/li/a[contains(@href,'graphexplorer2.azurewebsites.net')]")]
        private IWebElement exploreLink;

        public bool CanLoadImages(GraphPageImages image)
        {
            string prefix = GraphUtility.RemoveRedundantPartsfromExtractBaseAddress();
            switch (image)
            {
                case (GraphPageImages.MainBanner):
                    var element = GraphBrowser.FindElement(By.CssSelector("#layout-featured > div > article > div > div > div > div"));
                    //The div in Home page does not have id attribute
                    if (element == null)
                    {
                        element = GraphBrowser.FindElement(By.CssSelector("div#layout-featured>div>article>div>div>div>div"));
                    }
                    string Url = ((string)(GraphBrowser.webDriver as IJavaScriptExecutor).ExecuteScript(@"return getComputedStyle(arguments[0])['background-image'];", element)).Replace(@"url(""", "").Replace(@""")", "");
                    return GraphUtility.FileExist(Url);
               case (GraphPageImages.Others):
                    var elements = GraphBrowser.Driver.FindElements(By.CssSelector("img"));
                    foreach (IWebElement item in elements)
                    {
                        Url = item.GetAttribute("src");
                        if (!GraphUtility.FileExist(Url))
                        {
                            return false;
                        }
                    }

                    return true;
                default:
                    return false;
            }
        }
    }
}
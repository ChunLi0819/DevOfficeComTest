using System;
using System.Collections.Generic;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace TestFramework
{
    public class HomePage : BasePage
    {
        private static string PageTitle = "Office Dev Center - Office Dev Center";

        public void SlideToRightMenuItem()
        {
            var element = Browser.Driver.FindElement(By.CssSelector("#carousel > a.right.carousel-control"));
            Browser.Click(element);
            Browser.Wait(TimeSpan.FromSeconds(1));
        }

        public bool IsAt()
        {
            return Browser.Title == PageTitle;
        }

        public bool CanLoadImages(HomePageImages image)
        {
            switch (image)
            {
                case (HomePageImages.Banner):
                    var elements = Browser.Driver.FindElements(By.CssSelector("#carousel>div>div.item"));
                    foreach (IWebElement item in elements)
                    {
                        string Url = item.GetAttribute("style");
                        Url = Browser.BaseAddress + Url.Substring(Url.IndexOf('/'), Url.LastIndexOf('"') - Url.IndexOf('/'));
                        if (!Utility.FileExist(Url))
                        {
                            return false;
                        }
                    }

                    return true;
                case (HomePageImages.Icons):
                    elements = Browser.Driver.FindElements(By.CssSelector("#quarter1 > div > article > div > div.quicklinks-hidden-xs > div > ol > li"));
                    foreach (IWebElement item in elements)
                    {
                        IWebElement subItem = item.FindElement(By.CssSelector("a>div>div>div"));
                        string Url = subItem.GetAttribute("style");
                        Url = Browser.BaseAddress + Url.Substring(Url.IndexOf('/'), Url.LastIndexOf('"') - Url.IndexOf('/'));
                        if (!Utility.FileExist(Url))
                        {
                            return false;
                        }
                    }

                    return true;
                default:
                    return false;
            }
        }

        public void SelectSignup()
        {
            Browser.Wait(By.CssSelector("div.white-box>div>a.sign-up-link"));
            var element = Browser.FindElement(By.CssSelector("div.white-box>div>a.sign-up-link"));
            Browser.Click(element);
        }

        public bool CanDisplayCorrectTradeMark()
        {
            IWebElement element = Browser.Driver.FindElement(By.CssSelector("#layout-footer > div > div > div > div.clearfix > div > div > div.col-xs-6.col-md-12.col-lg-12.visible-md.visible-lg.privacy-links > ul > li"));
            return element.Text.Contains(DateTime.Now.Year.ToString());
        }

        /// <summary>
        /// Select a product on home page to get started
        /// </summary>
        /// <param name="item">The product to select</param>
        public void SelectGetStartedProduct(OfficeAppItem item)
        {
            Browser.Wait(By.XPath("//span[contains(@class,'devoffice-product')]/span[text()='" + item.ToString() + "']/ancestor::a"));
            var element = Browser.FindElement(By.XPath("//span[contains(@class,'devoffice-product')]/span[text()='" + item.ToString() + "']/ancestor::a"));
            Browser.Click(element);
        }

        /// <summary>
        /// Select Graph link on the home page
        /// </summary>
        public void SelectMSGraph()
        {
            Browser.Wait(By.CssSelector("a#graph-logo"));
            var element = Browser.FindElement(By.CssSelector("a#graph-logo"));
            Browser.Click(element);
        }

        /// <summary>
        /// Select Build link on the home page
        /// </summary>
        /// <param name="scheduledEventTime">The string indicates event's Time</param>
        /// <param name="selectBuildEvent">True if select the special build event link,false if select the Build icon </param>
        public void SelectBuild(out string scheduledEventTime,bool selectBuildEvent = false)
        {
            if (selectBuildEvent)
            {
                Browser.Wait(By.CssSelector("div.build-banner-content>div>a.event-title.primary"));
                var element = Browser.FindElement(By.CssSelector("div.build-banner-content>div>a.event-title.primary"));
                scheduledEventTime = element.Text;
                Browser.Click(element);
            }
            else
            {
                Browser.Wait(By.CssSelector("div.build-banner-content>a.build-logo"));
                var element = Browser.FindElement(By.CssSelector("div.build-banner-content>a.build-logo"));
                scheduledEventTime = string.Empty;
                Browser.Click(element);
            }
        }
    }

    public enum HomePageImages
    {
        Banner,
        Icons
    }
}
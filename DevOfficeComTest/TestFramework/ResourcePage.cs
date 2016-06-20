using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFramework
{
    public class ResourcePage : BasePage
    {
        private IWebElement resourceName;
        private string resourceTitle;
        public string ResourceName
        {
            get { return resourceTitle; }
        }

        public bool CanLoadImage(ResourcePageImages image)
        {
            switch (image)
            {
                case (ResourcePageImages.Banner):
                    IWebElement element = Browser.Driver.FindElement(By.Id("banner-image"));
                    string imageUrl = element.GetAttribute("style");
                    int urlStartIndex = imageUrl.IndexOf("http");
                    int urlEndIndex = imageUrl.LastIndexOf(")");
                    //1 is for character ' or "
                    int urlLength = urlEndIndex - urlStartIndex - 1;
                    imageUrl = imageUrl.Substring(urlStartIndex, urlLength);
                    return Utility.FileExist(imageUrl);
                case (ResourcePageImages.Responsive):
                    var elements = Browser.Driver.FindElements(By.CssSelector("img.img-responsive"));
                    if (elements.Count != 0)
                    {
                        foreach (IWebElement item in elements)
                        {
                            imageUrl = item.GetAttribute("src");
                            if (!Utility.FileExist(imageUrl))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                case (ResourcePageImages.Background):
                    elements = Browser.Driver.FindElements(By.CssSelector("div.background-img"));
                    if (elements.Count != 0)
                    {
                        foreach (IWebElement item in elements)
                        {
                            imageUrl = item.FindElement(By.TagName("img")).GetAttribute("src");
                            if (!Utility.FileExist(imageUrl))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                default:
                    return false;
            }
        }

        public ResourcePage()
        {
            Browser.SetWaitTime(TimeSpan.FromSeconds(5));
            try
            {
                resourceName = Browser.webDriver.FindElement(By.CssSelector("div.banner-text>h1"));
            }
            catch (NoSuchElementException)
            {
                resourceName = Browser.FindElement(By.CssSelector("div.banner-text>h2"));
                if (resourceName==null)
                {
                    //For Office 365 app registration tool
                    resourceName = Browser.FindElement(By.CssSelector("div#register-app > h1"));
                }
            }
            resourceTitle = resourceName!=null?resourceName.Text:Browser.webDriver.Title;
            Browser.SetWaitTime(TimeSpan.FromSeconds(Utility.DefaultWaitTime));
        }
    }

    public enum ResourcePageImages
    {
        Banner,
        Responsive,
        Background
    }
}

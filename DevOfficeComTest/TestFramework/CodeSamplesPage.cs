using OpenQA.Selenium;

namespace TestFramework
{
    public class CodeSamplesPage : BasePage
    {
        private OpenQA.Selenium.Remote.RemoteWebElement codeSamplesTitle;
        public string CodeSamplesPageTitle
        {
            get { return codeSamplesTitle.WrappedDriver.Title; }
        }
        public bool CanLoadImage()
        {
            IWebElement element = Browser.Driver.FindElement(By.Id("banner-image"));
            string Url = element.GetAttribute("style");
            Url = Browser.BaseAddress + Url.Substring(Url.IndexOf('/'), Url.LastIndexOf('"') - Url.IndexOf('/'));
            return Utility.FileExist(Url);
        }

        public CodeSamplesPage()
        {
            Browser.Wait(By.CssSelector("head>title"));
            codeSamplesTitle = (OpenQA.Selenium.Remote.RemoteWebElement)Browser.Driver.FindElement(By.CssSelector("head>title"));
        }

        public bool CanLoadImages(CodeSamplePageImages image)
        {
            switch (image)
            {
                case (CodeSamplePageImages.Banner):
                    IWebElement element = Browser.Driver.FindElement(By.Id("banner-image"));
                    string imageUrl = element.GetAttribute("style");
                    int urlStartIndex = imageUrl.IndexOf("http");
                    int urlEndIndex = imageUrl.LastIndexOf(")");
                    //1 is for character ' or "
                    int urlLength = urlEndIndex - urlStartIndex - 1;
                    imageUrl = imageUrl.Substring(urlStartIndex, urlLength);
                    return Utility.FileExist(imageUrl);
                case (CodeSamplePageImages.Icons):
                    var elements = Browser.Driver.FindElements(By.CssSelector("img.img-responsive"));
                    foreach (IWebElement item in elements)
                    {
                        imageUrl = item.GetAttribute("src");
                        if (!Utility.FileExist(imageUrl))
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

    public enum CodeSamplePageImages
    {
        Banner,
        Icons
    }
}
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TestFramework
{
    public class DocumentationPage : BasePage
    {
        private OpenQA.Selenium.Remote.RemoteWebElement documentationTitle;
        public string DocumentationTitle
        {
            get { return documentationTitle.WrappedDriver.Title; }
        }

        public DocumentationPage()
        {
            documentationTitle = (OpenQA.Selenium.Remote.RemoteWebElement)Browser.Driver.FindElement(By.CssSelector("head>title"));
        }

        /// <summary>
        /// Verify if the mobile menu-content is found on the page
        /// </summary>
        /// <returns>Trye if yes, else no.</returns>
        public static bool IsToggleMenuContentDisplayed()
        {
            Browser.Wait(TimeSpan.FromSeconds(2));
            return Browser.FindElement(By.CssSelector("div#menu-content")).Displayed;
        }

        /// <summary>
        /// Verify if the toggle menu icon is found on the page 
        /// </summary>
        /// <returns>Trye if yes, else no.</returns>
        public static bool IsToggleMenuIconDisplayed()
        {
            return Browser.FindElement(By.CssSelector("span#toggleLeftPanel")).Displayed;
        }

        /// <summary>
        /// Execute the menu display toggle
        /// </summary>
        public static void ToggleMobileMenu()
        {
            var element = Browser.FindElement(By.CssSelector("span#toggleLeftPanel"));
            Browser.Click(element);
            Browser.Wait(TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Check whether a doc header exists
        /// </summary>
        /// <param name="docHeader">The expected doc header</param>
        /// <returns>True if yes, else no.</returns>
        public static bool HasDocHeader(string docHeader)
        {
            var element = Browser.FindElement(By.XPath("//table[@id='OfficeDocHeader']/tbody/tr/td/a[text()='" + docHeader + "']"));
            return element != null;
        }

        /// <summary>
        /// Check whether can edit Office add-in docs in github
        /// </summary>
        /// <returns>True if yes, else no.</returns>
        public static bool CanEditInGitHub()
        {
            var element = Browser.FindElement(By.XPath("//div[@id='GitHubInfo']/a[text()='Edit in GitHub']"));
            Browser.Click(element);
            Browser.SwitchToNewWindow();
            bool isPageCorrect = Browser.webDriver.Url.Contains("github.com/OfficeDev") && Browser.webDriver.Url.Contains("docs/overview/office-add-ins.md");
            Browser.SwitchBack();
            return isPageCorrect;
        }

        /// <summary>
        /// Get The layer count of TOC items
        /// </summary>
        /// <returns>The layer count</returns>
        public static int GetTOCLayer()
        {
            string xpath = "//nav[@id='home-nav-blade']";
            var menuElement = Browser.FindElement(By.XPath(xpath));
            int layer = 0;
            try
            {
                do
                {
                    layer++;
                    xpath += "/ul/li";
                    var element = menuElement.FindElement(By.XPath(xpath + "/a"));
                } while (true);
            }
            catch (NoSuchElementException)
            {
            }
            return layer - 1;
        }

        /// <summary>
        /// Return a random selection of items at TOC's specific level
        /// </summary>
        /// <param name="index">The specific level index. Starts from 0</param>
        /// <param name="hasDoc">Indicates whether only returns the items each of which has the related documents</param>
        /// <returns>TOC Items' title-and-links(separated by ,). The title part contains the item's whole path in TOC</returns>
        public static string GetTOCItem(int index, bool hasDoc = false)
        {
            string xPath = "//nav[@id='home-nav-blade']";
            for (int i = 0; i <= index; i++)
            {
                xPath += "/ul/li";
            }
            //Find all the toc items at the specific level
            IReadOnlyList<IWebElement> links = Browser.webDriver.FindElements(By.XPath(xPath + "/a"));
            string item = string.Empty;

            int randomIndex;
            do
            {
                randomIndex = new Random().Next(links.Count);

                string path = string.Empty;
                var ancestorElements = links[randomIndex].FindElements(By.XPath("ancestor::li/a")); //parent relative to current element
                for (int j = 0; j < ancestorElements.Count - 1; j++)
                {
                    string ancestorTitle = ancestorElements[j].GetAttribute("innerHTML");
                    if (ancestorElements[j].GetAttribute("style").Contains("text-transform: uppercase"))
                    {
                        ancestorTitle = ancestorTitle.ToUpper();
                    }
                    path += ancestorTitle + ">";
                }
                string title = links[randomIndex].GetAttribute("innerHTML");
                if (links[randomIndex].GetAttribute("style").Contains("text-transform: uppercase"))
                {
                    title = title.ToUpper();
                }
                if (hasDoc)
                {
                    if (!links[randomIndex].GetAttribute("href").EndsWith("/"))
                    {
                        item = path + title + "," + links[randomIndex].GetAttribute("href");
                    }
                }
                else
                {
                    item = path + title + "," + links[randomIndex].GetAttribute("href");
                }
            } while (links[randomIndex].GetAttribute("href").EndsWith("/"));
            return item;
        }

        /// <summary>
        /// Verify whether a TOC item's related sub layer is shown 
        /// </summary>
        /// <param name="item">The TOC item</param>
        /// <returns>True if yes, else no.</returns>
        public static bool SubLayerDisplayed(string item)
        {
            string xpath = @"//nav[@id='home-nav-blade']";
            var element = Browser.FindElement(By.XPath(xpath));
            var menuItem = element.FindElement(By.LinkText(item));
            string subMenuId = menuItem.GetAttribute("data-target");
            if (subMenuId != null && subMenuId != string.Empty)
            {
                var subMenu = element.FindElement(By.XPath("//ul[@id='" + subMenuId.Replace("#", string.Empty) + "']"));
                return subMenu.Displayed;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get the document title in the current doc page
        /// </summary>
        /// <returns>The title of document</returns>
        public static string GetDocTitle()
        {
            string docTitle = Browser.FindElement(By.CssSelector("div#docContent>div#OfficeDocDiv>div#holder>div#body>div>h1")).Text;
            return docTitle;
        }

        //public static void RightClickToOpenInNewTab(string docItem)
        //{
        //    var navListHead = Browser.FindElement(By.LinkText("OVERVIEW"));
        //    int headY = navListHead.Location.Y;
        //    var element = Browser.FindElement(By.LinkText(docItem));
        //    int elementY = element.Location.Y;
        //    if (element != null && element.Displayed)
        //    {
        //        Actions action = new Actions(Browser.webDriver);
        //        action.MoveToElement(navListHead, 0, elementY - headY).ContextClick(element).SendKeys(Keys.ArrowDown).SendKeys(Keys.Return).Build().Perform();
        //    }
        //}

        /// <summary>
        /// Get all the available products
        /// </summary>
        /// <returns>a string array contains all the products</returns>
        public static string[] GetProducts()
        {
            string[] products=new string[Browser.webDriver.FindElements(By.CssSelector("div#productFilter>select>option")).Count];
            for (int i = 0; i < products.Length;i++ )
            {
                products[i] = Browser.FindElement(By.CssSelector("div#productFilter > select > option:nth-child("+(int)(i+1)+")")).Text;
            }
            return products;
        }

        /// <summary>
        /// Change the selected product
        /// </summary>
        /// <param name="product">the expected product to select</param>
        public static void ChangeProduct(string product)
        {
            var element = Browser.FindElement(By.XPath("//div[@id='productFilter']/select/option[@value='"+product+"']"));
            SelectElement productList = new SelectElement(Browser.FindElement(By.CssSelector("div#productFilter > select")));
            productList.SelectByValue(product);
        }
    }
}
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;

namespace TestFramework
{
    public class FabricPage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//div[contains(@class,'Carousel-staticContent')]")]
        private IWebElement fabricPageTitle;

        public IReadOnlyList<IWebElement> LeftNavItems
        {
            get
            {
                return Browser.webDriver.FindElements(By.CssSelector("ul.LeftNav-links > li> a"));
            }
        }

        public string FabricPageTitle
        {
            get { return fabricPageTitle.Text; }
        }

        /// <summary>
        /// Select an item on Fabric page's own top nav bar.
        /// </summary>
        public void SelectTopNavItem(FabricNavItem item)
        {
            int waitTime = Int32.Parse(Utility.GetConfigurationValue("WaitTime"));
            string itemToSelect = EnumExtension.GetDescription(item);
            Browser.Wait(By.XPath("//div[@class='docs-Nav']/div/a[text()='" + itemToSelect + "']"));
            var element = Browser.FindElement(By.XPath("//div[@class='docs-Nav']/div/a[text()='" + itemToSelect + "']"));
            Browser.Click(element);
            Browser.Wait(TimeSpan.FromSeconds(waitTime));
        }

        /// <summary>
        /// Check whether select a fabric nav item can navigate to the correct page
        /// </summary>
        /// <param name="item">The item selected</param>
        /// <returns>True if yes, else no.</returns>
        public bool CanSwitchCorrectPage(FabricNavItem item)
        {
            string itemSelected = EnumExtension.GetDescription(item);
            if (itemSelected.Equals("Overview"))
            {
                return Browser.webDriver.Title.EndsWith("Fabric Home Page");
            }
            else if (itemSelected.Equals("Get Started"))
            {
                return Browser.webDriver.Title.EndsWith("Getting Started");
            }
            else
            {
                return Browser.webDriver.Title.EndsWith(itemSelected);
            }
        }

        /// <summary>
        /// Check whether a left nav item refers to a valid doc or doc part
        /// </summary>
        /// <param name="index">The index of the lfet nav item</param>
        /// <param name="itemText">The item text</param>
        /// <returns>True if yes, else no.</returns>
        public bool IsValidLeftNavItem(int index, out string itemText)
        {
            Browser.Wait(By.CssSelector("div.LeftNav-links > div:nth-child(" + (int)(index + 1) + ") > a"));
            var element = Browser.FindElement(By.CssSelector("div.LeftNav-links > div:nth-child(" + (int)(index + 1) + ") > a"));
            itemText = element.Text;
            string href = element.GetAttribute("href");
            return Utility.FileExist(href);
        }

        /// <summary>
        /// Verify if the mobile menu-content is found on the page
        /// </summary>
        /// <returns>Trye if yes, else no.</returns>
        public static bool IsMobileMenuContentDisplayed()
        {
            Browser.Wait(TimeSpan.FromSeconds(2));
            return Browser.FindElement(By.CssSelector("div.ms-Panel-main")).Displayed;
        }

        /// <summary>
        /// Verify if the toggle menu icon is found on the page 
        /// </summary>
        /// <returns>Trye if yes, else no.</returns>
        public static bool IsToggleMenuIconDisplayed()
        {
            return Browser.FindElement(By.CssSelector("div.docs-MobileNav")).Displayed;
        }

        /// <summary>
        /// Execute the mobile menu display toggle
        /// </summary>
        public static void ToggleMobileMenu()
        {
            var element = Browser.FindElement(By.CssSelector("div.docs-MobileNav>a.docs-MobileNav-menuButton"));
            var panelElement = Browser.FindElement(By.CssSelector("div.ms-Panel-main"));
            if (element.Displayed && !panelElement.Displayed)
            {
                Browser.Click(element);
                Browser.Wait(TimeSpan.FromSeconds(2));
            }
            else
            {
                //Click at any position outside the menu to hide it
                Actions action = new Actions(Browser.webDriver);
                int offX = panelElement.Size.Width + 100;
                int offY = panelElement.Size.Height / 2;
                action.MoveToElement(panelElement,offX,offY);
                action.Click().Build().Perform();
            }
        }
    }
}
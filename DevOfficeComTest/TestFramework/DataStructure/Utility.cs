using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace TestFramework
{
    /// <summary>
    /// Static class for common functions
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Some typical search text
        /// </summary>
        public static readonly string[] TypicalSearchText = new string[] { "SharePoint", "Add-in", "Graph", "ios", "OneDrive" };

        public static readonly int MinWidthToShowParam = 895;

        public static int DefaultWaitTime = int.Parse(GetConfigurationValue("DefaultWaitTime"));

        /// <summary>
        /// Verify if none of the filters is checked
        /// </summary>
        /// <param name="unclearedFilters">The name of the uncleared filters</param>
        /// <returns>True if yes, else no.</returns>
        public static bool areFiltersCleared(out List<string> unclearedFilters)
        {
            IReadOnlyList<IWebElement> elements = Browser.Driver.FindElements(By.XPath(@"//*[@ng-model=""selectedTypes""]"));
            unclearedFilters = new List<string>();
            foreach (IWebElement element in elements)
            {
                if (element.GetAttribute("type").Equals("checkbox") && element.GetAttribute("checked") != null && element.GetAttribute("checked").Equals("checked"))
                {
                    unclearedFilters.Add(element.GetAttribute("value"));
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verify whether the url contains the chosen filters
        /// </summary>
        /// <param name="filterNames">The chosen filters</param>
        /// <returns>True if yes, else no.</returns>
        public static bool AreFiltersInURL(List<string> filterNames, out List<string> unContainedFilters)
        {
            bool allContained = true;
            unContainedFilters = new List<string>();
            foreach (string filterName in filterNames)
            {
                if (!Browser.Url.Replace("%20", " ").ToLower().Contains(filterName.ToLower()))
                {
                    allContained = false;
                    unContainedFilters.Add(filterName);
                }
            }
            return allContained;
        }

        public static bool IsAtProfileCenterPage()
        {
            bool isValid = false;
            if (Browser.webDriver.WindowHandles.Count > 1)
            {
                Browser.SwitchToNewWindow();
                isValid = Browser.Url.Contains("profile.microsoft.com/RegSysProfileCenter");
                Browser.SwitchBack();
            }
            else
            {
                isValid = Browser.Url.Contains("profile.microsoft.com/RegSysProfileCenter");
                Browser.GoBack();
            }
            return isValid;
        }

        /// <summary>
        /// Get the count of filters
        /// </summary>
        public static int GetFilterCount()
        {
            return Browser.Driver.FindElements(By.XPath(@"//*[@ng-model=""selectedTypes""]")).Count;
        }

        /// <summary>
        /// Check whether a filter has an "ng-click" attribute with an updating seleted result function's name.
        /// </summary>
        /// <param name="index">The index of the filter type to select</param>
        /// <returns>True if yes, else no.</returns>
        public static bool isFilterWorkable(int index)
        {
            var element = Browser.Driver.FindElements(By.XPath(@"//*[@ng-model=""selectedTypes""]"))[index];
            return element.GetAttribute("ng-click").Contains("updateSelectedTypes(");
        }

        /// <summary>
        /// Set the displayed results' sort order
        /// </summary>
        /// <param name="sortType">Specifies which sort type to use</param>
        /// <param name="isDescendent">Specifies whether the results are sorted descendently. True means yes, False means no</param>
        public static void SetSortOrder(SortType sortType, bool isDescendent)
        {
            string typeString;
            if (sortType.Equals(SortType.ViewCount))
            {
                typeString = "orderByViews()";
            }
            else
            {
                typeString = "orderByDate()";
            }

            var sortElement = Browser.Driver.FindElement(By.XPath("//a[@ng-click='" + typeString + "']"));
            if (!sortElement.Selected)
            {
                sortElement.Click();
            }

            string orderString = isDescendent ? "sort_down" : "sort_up";
            if (!sortElement.FindElement(By.ClassName("sort-icon")).GetAttribute("src").Contains(orderString))
            {
                sortElement.Click();
            }
        }

        /// <summary>
        /// Clear the filters 
        /// </summary>
        public static void ExecuteClearFilters()
        {
            var element = Browser.FindElement(By.CssSelector(".clearfilters.filter-button"));
            Browser.Click(element);
        }

        /// <summary>
        /// Set a search text
        /// </summary>
        /// <param name="value">The value to set</param>
        public static void InputSearchString(string value)
        {
            var inputElement = Browser.Driver.FindElement(By.XPath(@"//input[@ng-model=""searchText""]"));
            inputElement.Clear();
            inputElement.SendKeys(value);
        }

        /// <summary>
        /// Returns the filtered traings
        /// </summary>
        /// <param name="searchString">The search text to use</param>
        /// <returns>The search result list. Each result contains the training title and description</returns>
        public static List<SearchedResult> GetFilterResults(string searchString = "")
        {
            InputSearchString(searchString);
            List<SearchedResult> resultList = new List<SearchedResult>();

            var nextElement = Browser.FindElement(By.ClassName("next-link"));
            bool shouldReadFirstPage = true;
            do
            {
                if (shouldReadFirstPage)
                {
                    shouldReadFirstPage = false;
                }
                else
                {
                    Browser.Click(nextElement);
                }
                var uList = Browser.FindElement(By.CssSelector(@"#OrderedResults+ul"));
                IReadOnlyList<IWebElement> listItems = uList.FindElements(By.XPath("li"));
                for (int i = 0; i < listItems.Count; i++)
                {
                    SearchedResult resultInfo = new SearchedResult();
                    resultInfo.Name = listItems[i].GetAttribute("aria-label");

                    var descriptionElement = listItems[i].FindElement(By.ClassName("description"));
                    resultInfo.Description = descriptionElement.Text;

                    resultInfo.ViewCount = Convert.ToInt64((listItems[i].FindElement(By.XPath("//span[contains(text(),' views')]")).GetAttribute("innerHTML").Split(' '))[0]);

                    // Add if() here to reduce the time cost searching for non-existent element of class date-updated
                    if (!Browser.Url.Contains("/training"))
                    {
                        IWebElement updatedDateElement;
                        try
                        {
                            updatedDateElement = listItems[i].FindElement(By.CssSelector(".date-updated"));
                        }
                        catch (NoSuchElementException)
                        {
                            updatedDateElement = null;
                        }
                        if (updatedDateElement != null)
                        {
                            resultInfo.UpdatedDate = DateTime.Parse(updatedDateElement.Text.Replace("Updated ", null));
                        }
                    }

                    resultInfo.DetailLink = listItems[i].FindElement(By.XPath("//a[@role='link']")).GetAttribute("href");

                    resultList.Add(resultInfo);
                }
            } while (nextElement.Displayed);

            return resultList;
        }

        /// <summary>
        /// Choose a filter type
        /// </summary>
        /// <param name="index">The index of the filter type to select</param>
        /// <returns>The selected filter name</returns>
        public static string SelectFilter(int index)
        {
            var element = Browser.Driver.FindElements(By.XPath(@"//*[@ng-model=""selectedTypes""]"))[index];
            Browser.Click(element);

            return element.Text;
        }

        /// <summary>
        /// Choose a filter type
        /// </summary>
        /// <param name="filterName">The filter name to select</param>
        public static void SelectFilter(string filterName)
        {
            IReadOnlyList<IWebElement> elements = Browser.Driver.FindElements(By.XPath(@"//*[@ng-model=""selectedTypes""]"));
            foreach (IWebElement element in elements)
            {
                if (element.Text.Equals(filterName) || element.GetAttribute("value").Equals(filterName))
                {
                    Browser.Click(element);
                    break;
                }
            }
        }

        /// <summary>
        /// Verify whether a link of a specific source type can be found on the page 
        /// </summary>
        /// <param name="sourcePart">A string that contains the part of source link</param>
        /// <returns>True if yes, else no</returns>
        public static bool CanFindSourceLink(string sourcePart)
        {
            var element = Browser.FindElement(By.XPath("//a[contains(@href,'" + sourcePart + "')]"));
            if (element != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get a property's value from App.config
        /// </summary>
        /// <param name="propertyName">The property's key</param>
        /// <returns>The property's value</returns>
        public static string GetConfigurationValue(string propertyName)
        {
            return ConfigurationManager.AppSettings[propertyName];
        }

        /// <summary>
        /// Verify whether a url refer to a valid image
        /// </summary>
        /// <param name="Url">The image url</param>
        /// <returns>True if yes, else no</returns>
        public static bool FileExist(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Timeout = 15000;
            request.Method = "HEAD";

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotModified);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Verify whether a url is redirected.
        /// </summary>
        /// <param name="Url">The url to be verified</param>
        /// <returns>True if yes, else no</returns>
        public static bool IsUrlRedirected(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Timeout = 15000;
            request.Method = "GET";
            request.AllowAutoRedirect = false;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return (response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.Redirect);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Click the branding image on the page
        /// </summary>
        public static void ClickBranding()
        {
            var element = Browser.FindElement(By.CssSelector("#branding>a"));
            Browser.Click(element);
        }

        /// <summary>
        /// Get all the links in a NavItem's sub menu
        /// </summary>
        /// <param name="itemIndex">The nav item index in nav bar</param>
        public static string[] GetNavSubItems(int itemIndex)
        {
            var element = Browser.FindElement(By.CssSelector("#navbar-collapse-1 > ul > li:nth-child(" + ((int)itemIndex + 1) + ") > a"));
            try
            {
                var subMenu = element.FindElement(By.XPath("parent::li/div"));
                IReadOnlyList<IWebElement> subItems = subMenu.FindElements(By.TagName("a"));

                string[] items = new string[subItems.Count];
                for (int i = 0; i < subItems.Count; i++)
                {
                    if (subItems[i].Displayed)
                    {
                        items[i] = subItems[i].Text;
                    }
                    else
                    {
                        items = null;
                        break;
                    }
                }
                return items;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }

        /// <summary>
        /// Verify whether the branding of dev center exists on the current page
        /// </summary>
        /// <returns></returns>
        public static bool BrandingExists()
        {
            var element = Browser.FindElement(By.XPath("//div[@id='branding']/a/img[@alt='Office Dev Office Center logo']"));
            if (element != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Input search text into iwidget input field and search it
        /// </summary>
        /// <param name="searchText">The text to input</param>
        /// <returns>The names of the search results</returns>
        public static string[] SearchWidget(string searchText)
        {
            var element = Browser.FindElement(By.XPath("//fieldset/input[@id='q']"));
            element.SendKeys(searchText);
            var searchElement = Browser.FindElement(By.XPath("//fieldset/button[@type='submit']"));
            Browser.Click(searchElement);

            Browser.Wait(TimeSpan.FromSeconds(5));

            IReadOnlyList<IWebElement> results = Browser.webDriver.FindElements(By.CssSelector("div.col-xs-8.name.cp1"));
            string[] resultNames = new string[results.Count];
            for (int i = 0; i < results.Count; i++)
            {
                resultNames[i] = results[i].Text;
            }

            return resultNames;
        }

        public static void ClickIcon(string iconType)
        {
            switch (iconType)
            {
                //case "Facebook":
                //    var element = Browser.FindElement(By.CssSelector("#atstbx > a.at-custom-share-anchor.at-share-btn.at-svc-facebook > span > span > svg"));
                //    Browser.Click(element);
                //    break;
                case "RSS":
                    var element = Browser.FindElement(By.CssSelector("#header-social > a"));
                    Browser.Click(element);
                    break;
            }
        }

        public static SliderMenuItem GetRightMenuItem(SliderMenuItem item)
        {
            if ((int)item == Enum.GetNames(item.GetType()).Count() - 1)
            {
                return (SliderMenuItem)(0);
            }
            else
            {
                return (SliderMenuItem)(item + 1);
            }
        }

        /// <summary>
        /// Select a doc item on Office Developer Documentation page and click it
        /// </summary>
        /// <param name="item">The item to click</param>
        public static void SelectDocItem(Enum item)
        {
            Browser.Wait(By.XPath("//span[text()='" + item.ToString() + "']/ancestor::a"));
            IWebElement element = Browser.FindElement(By.XPath("//span[text()='" + item.ToString() + "']/ancestor::a"));
            Browser.Click(element);
        }

        /// <summary>
        /// Check whether the current Office Add-in document page is correct
        /// </summary>
        /// <param name="item">The selected Office Add-in doc item</param>
        /// <returns>True if yes, else no</returns>
        public static bool IsAtOfficeAddinDocPage(ItemOfOfficeAddinDoc item)
        {
            if (item.Equals(ItemOfOfficeAddinDoc.SharePoint))
            {
                return Browser.Title.Equals("SharePoint general development");
            }
            else
            {
                //if (!Browser.Title.StartsWith("Office Dev Center - Add-ins Docs and References"))
                //{
                //    return false;
                //}
                Browser.Wait(By.XPath("//select[@aria-label='ProductFilter']/option[@selected]"));
                var element = Browser.FindElement(By.XPath("//select[@aria-label='ProductFilter']/option[@selected]"));
                return element.GetAttribute("value").Equals(item.ToString());
            }
        }

        /// <summary>
        /// Check whether the current MS Graph document page is correct
        /// </summary>
        /// <param name="item">The selected MS Graph doc item</param>
        /// <returns>True if yes, else no</returns>
        public static bool IsAtMSGraphDocPage(ItemOfMSGraphDoc item)
        {
            Browser.SwitchToNewWindow();
            return Browser.Title.Equals("Microsoft Graph - Documentation - " + item.ToString().ToLower());
        }

        /// <summary>
        /// Randomly select an available Office Add-in's details on Office Add-in Availability Page.
        /// </summary>
        /// <param name="detailItem">The selected details' subject</param>
        public static void SelectRandomOfficeAddInDetail(out string detailItem)
        {
            int detailCount = Browser.webDriver.FindElements(By.CssSelector("span.ms-Table-cell.availible")).Count;
            int index = new Random().Next(detailCount);
            var detailElement = Browser.webDriver.FindElements(By.CssSelector("span.ms-Table-cell.availible>button"))[index];
            detailItem = detailElement.GetAttribute("id");
            Browser.Click(detailElement);
        }

        /// <summary>
        /// Check whether a detail area is popped up
        /// </summary>
        /// <param name="detailItem">The expected detail subject</param>
        /// <returns>True if yes, else no.</returns>
        public static bool DetailExist(string detailItem)
        {
            var element = Browser.FindElement(By.CssSelector("div#" + detailItem + "_Details"));
            return element != null;
        }

        public static bool CanSelectOfficeAddInRequirementSets()
        {
            var topElement = Browser.FindElement(By.XPath("//a[text()='requirement sets']"));
            Browser.Click(topElement);
            Browser.SwitchToNewWindow();
            var head = Browser.FindElement(By.CssSelector("h1#office-add-in-requirement-sets"));
            bool isTopLinkValid = head != null;
            Browser.SwitchBack();

            var bottomElement = Browser.FindElement(By.XPath("//a[text()='Office add-in requirement sets']"));
            Browser.Click(bottomElement);
            Browser.SwitchToNewWindow();
            head = Browser.FindElement(By.CssSelector("h1#office-add-in-requirement-sets"));
            bool isBottomLinkValid = head != null;
            Browser.SwitchBack();

            return isTopLinkValid && isBottomLinkValid;
        }

        public static bool CanSelectOfficeAddinsPlatformOverview()
        {
            var element = Browser.FindElement(By.XPath("//a[text()='Office Add-ins platform overview']"));
            Browser.Click(element);
            Browser.SwitchToNewWindow();
            var head = Browser.FindElement(By.CssSelector("h1#office-add-ins-platform-overview"));
            bool isValid = head != null;
            Browser.SwitchBack();
            return isValid;
        }

        public static bool CanSelectJavaScriptAPIforOfficereference()
        {
            var element = Browser.FindElement(By.XPath("//a[text()='JavaScript API for Office reference']"));
            Browser.Click(element);
            Browser.SwitchToNewWindow();
            var head = Browser.FindElement(By.CssSelector("h1#javascript-api-for-office-reference"));
            bool isValid = head != null;
            Browser.SwitchBack();
            return isValid;
        }

        public static bool IsAtAppDevPage(OfficeAppItem item)
        {
            Product productItem;
            OtherProduct otherProductItem;
            bool isValid = false;
            if (Enum.TryParse(item.ToString(), out productItem))
            {
                isValid = Pages.Navigation.IsAtProductPage(item.ToString());

            }
            else if (Enum.TryParse(item.ToString(), out otherProductItem))
            {
                isValid = Browser.webDriver.Title.ToLower().Contains(item.ToString().ToLower());
            }
            Browser.GoBack();
            return isValid;
        }

        /// <summary>
        /// Check whether the current page is the expected MS Build page
        /// </summary>
        /// <param name="eventTime">The expected event time</param>
        /// <returns>True if yes, else no.</returns>
        public static bool IsAtBuildPage(string eventTime)
        {
            if (eventTime != string.Empty)
            {
                var element = Browser.FindElement(By.CssSelector("body > header > time"));
                bool isValid = eventTime.Replace("–", "-").Trim().EndsWith(element.Text.Replace("–", "-").Trim());
                Browser.GoBack();
                return isValid;
            }
            else
            {
                bool isValid = Browser.webDriver.Title.StartsWith("Microsoft Build Developer Conference");
                Browser.GoBack();
                return isValid;
            }
        }

        /// <summary>
        /// Find an link or a button role span according to the specific text and click it
        /// </summary>
        /// <param name="text">The text of the element</param>
        public static void Click(string text)
        {
            var element = Browser.FindElement(By.LinkText(text));
            //a link
            if (element != null && element.Displayed)
            {
                Browser.Click(element);
            }
            else
            {
                IReadOnlyList<IWebElement> elements = Browser.webDriver.FindElements(By.XPath("//*[@role='button']"));
                foreach (IWebElement elementToClick in elements)
                {
                    if (elementToClick.GetAttribute("innerHTML").Equals(text) && (elementToClick.Displayed))
                    {
                        Browser.Click(elementToClick);
                        break;
                    }
                }
            }
        }
    }
}

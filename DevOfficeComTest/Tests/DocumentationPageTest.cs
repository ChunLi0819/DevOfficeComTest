using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using TestFramework;

namespace Tests
{
    /// <summary>
    /// Test class for pages under nav item Documentation
    /// </summary>
    [TestClass]
    public class DocumentationPageTest
    {
        static int currentWidth;
        static int currentHeight;

        #region Additional test attributes
        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
            Browser.Initialize();
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            Browser.Close();
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            Browser.GetWindowSize(out currentWidth, out currentHeight);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            Browser.SetWindowSize(currentWidth, currentHeight);
            Browser.Goto(Utility.GetConfigurationValue("BaseAddress"));
        }

        #endregion

        /// <summary>
        /// Verify whether all links to Office add-in docs work
        /// </summary>
        [TestMethod]
        public void BVT_S18_TC01_CanGotoOfficeAddinDocPage()
        {
            Pages.Navigation.Select("Documentation", MenuItemOfDocumentation.AllDocumentation.ToString());
            foreach (ItemOfOfficeAddinDoc item in Enum.GetValues(typeof(ItemOfOfficeAddinDoc)))
            {
                Utility.SelectDocItem(item);
                Assert.IsTrue(Utility.IsAtOfficeAddinDocPage(item),
                    "Clicking {0} should refer to the related document page",
                    item.ToString());
                Browser.GoBack();
            }
        }

        /// <summary>
        /// Verify whether all links to Office add-in docs work
        /// </summary>
        [TestMethod]
        public void BVT_S18_TC02_CanGotoMSGraphDocPage()
        {
            Pages.Navigation.Select("Documentation", MenuItemOfDocumentation.AllDocumentation.ToString());
            foreach (ItemOfMSGraphDoc item in Enum.GetValues(typeof(ItemOfMSGraphDoc)))
            {
                Utility.SelectDocItem(item);
                Assert.IsTrue(Utility.IsAtMSGraphDocPage(item),
                    "Clicking {0} should refer to the related document page",
                    item.ToString());
                Browser.SwitchBack();
            }
        }

        /// <summary>
        /// Verify whether selecting an Office add-in's details can see the expected info
        /// on Office Add-in Availability Page
        /// </summary>
        [TestMethod]
        public void BVT_S18_TC03_CanFindOfficeAddInDetail()
        {
            Pages.Navigation.Select("Documentation", MenuItemOfDocumentation.OfficeAddinAvailability.ToString());
            string detailItem = string.Empty;
            Utility.SelectRandomOfficeAddInDetail(out detailItem);
            Assert.IsTrue(Utility.DetailExist(detailItem), "Details for {0} should appear", detailItem);
        }

        [TestMethod]
        public void Comps_S18_TC04_CheckOfficeAddInAvailabilityPageLinks()
        {
            Pages.Navigation.Select("Documentation", MenuItemOfDocumentation.OfficeAddinAvailability.ToString());
            Assert.IsTrue(Utility.CanSelectOfficeAddInRequirementSets(), "Office add-in requirement sets page should be available");

            Assert.IsTrue(Utility.CanSelectOfficeAddinsPlatformOverview(), "Office Add-ins platform overview page should be available");

            Assert.IsTrue(Utility.CanSelectJavaScriptAPIforOfficereference(), "JavaScript API for Office reference page should be available");
        }

        /// <summary>
        /// Verify whether there is a toggle arrow which work correctly when the window is small.
        /// </summary>
        [TestMethod]
        public void Comps_S18_TC05_CanToggleArrowWorkInOfficeAddInDocPage()
        {
            Pages.Navigation.Select("Documentation", MenuItemOfDocumentation.OfficeAddin.ToString());

            Size windowSize;
            //Set as the screen size of IPad2
            double deviceScreenSize = double.Parse(Utility.GetConfigurationValue("IPad2Size"));
            Browser.TransferPhysicalSizeToPixelSize(
                deviceScreenSize,
                new Size
                {
                    Width = int.Parse(Utility.GetConfigurationValue("IPad2ScreenResolutionWidth")),
                    Height = int.Parse(Utility.GetConfigurationValue("IPad2ScreenResolutionHeight"))
                },
                out windowSize);
            Browser.SetWindowSize(windowSize.Width, windowSize.Height);

            Assert.IsTrue(
                DocumentationPage.IsToggleMenuIconDisplayed(),
                "An IPad2 window size ({0} inches) can make menu icon appear.",
                deviceScreenSize);
            Assert.IsFalse(DocumentationPage.IsToggleMenuContentDisplayed(),
                "When the menu icon exists, menu should be hidden.");

            DocumentationPage.ToggleMobileMenu();
            Assert.IsTrue(DocumentationPage.IsToggleMenuContentDisplayed(),
                "When the menu icon exists and menu is hidden,clicking the menu icon should show menu.");

            DocumentationPage.ToggleMobileMenu();
            Assert.IsFalse(DocumentationPage.IsToggleMenuContentDisplayed(),
                "When the menu is shown,clicking the menu icon should hide the menu.");

            //Set as the screen size of IPhone6 plus
            deviceScreenSize = double.Parse(Utility.GetConfigurationValue("IPhone6PlusSize"));
            //Since mobile phone width<Height, invert the output values
            Browser.TransferPhysicalSizeToPixelSize(
               deviceScreenSize,
               new Size
               {
                   Width = int.Parse(Utility.GetConfigurationValue("IPhone6PlusScreenResolutionWidth")),
                   Height = int.Parse(Utility.GetConfigurationValue("IPhone6PlusScreenResolutionHeight"))
               },
               out windowSize);
            //Since mobile phone widh<height, invert height and width
            Browser.SetWindowSize(windowSize.Height, windowSize.Width);

            Assert.IsTrue(
                DocumentationPage.IsToggleMenuIconDisplayed(),
                "An IPhone6 Plus window size ({0} inches) can make menu icon appear.",
                deviceScreenSize);
        }

        /// <summary>
        /// Verify whether toggle mobile menu icon hides when the window is large.
        /// </summary>
        [TestMethod]
        public void Acceptance_S18_TC06_CanToggleArrowHideInLargeOfficeAddInDocPage()
        {
            Pages.Navigation.Select("Documentation", MenuItemOfDocumentation.OfficeAddin.ToString());

            int actualWidth = 0;
            int actualHeight = 0;
            //Maxsize the window to see if it is possible to hide the arrow
            Browser.SetWindowSize(actualWidth, actualHeight, true);
            Browser.GetWindowSize(out actualWidth, out actualHeight);
            if (DocumentationPage.IsToggleMenuIconDisplayed())
            {
                Assert.Inconclusive(
                    "A window size ({0}*{1}) is not big enough to hide menu icon",
                    actualWidth,
                    actualHeight);
            }
            else
            {
                //Set a common laptop size: 17.3 and a common screen resolution:1024*768
                double deviceScreenSize = 17.3;
                Size windowSize;
                Browser.TransferPhysicalSizeToPixelSize(
                    deviceScreenSize,
                    new Size
                    {
                        Width = 1024,
                        Height = 768
                    },
                    out windowSize);
                Browser.SetWindowSize(windowSize.Width, windowSize.Height);

                Assert.IsFalse(
                    DocumentationPage.IsToggleMenuIconDisplayed(),
                    "An large window size ({0} inches) can make menu icon hide.",
                    deviceScreenSize);
            }
        }

        /// <summary>
        /// Check Office add-in doc page
        /// </summary>
        [TestMethod]
        public void BVT_S18_TC07_IsOfficeAddInDocPageCorrect()
        {
            Pages.Navigation.Select("Documentation", MenuItemOfDocumentation.OfficeAddin.ToString());

            Assert.IsTrue(
            DocumentationPage.HasDocHeader("Docs"),
            "Office add in page should have 'Docs' header");

            Assert.IsTrue(
            DocumentationPage.HasDocHeader("API Reference"),
            "Office add in page should have 'API Reference' header");

            Assert.IsTrue(DocumentationPage.CanEditInGitHub(),
                "Should be able to edit Office add-in page in github");
        }

        /// <summary>
        /// Check whether the nav list item can refer to the correct doc
        /// </summary>
        [TestMethod]
        public void BVT_S18_TC08_CanNavListItemReferToCorrectDoc()
        {
            Pages.Navigation.Select("Documentation", MenuItemOfDocumentation.OfficeAddin.ToString());

            int layerCount = DocumentationPage.GetTOCLayer();
            int randomIndex = new Random().Next(layerCount);
            string levelItem = DocumentationPage.GetTOCItem(randomIndex, true);
            string[] parts = levelItem.Split(new char[] { ',' });
            string[] tocPath = parts[0].Split(new char[] { '>' });
            string title = tocPath[tocPath.Length - 1];
            string link = parts[1];
            // Click the item from its toppest layer item to itself
            for (int j = 0; j < tocPath.Length; j++)
            {
                //Avoid to fold up the sublayer
                if (!DocumentationPage.SubLayerDisplayed(tocPath[j]))
                {
                    Utility.Click(tocPath[j]);
                }
            }
            Browser.Wait(TimeSpan.FromSeconds(int.Parse(Utility.GetConfigurationValue("WaitTime"))));
            string docTitle = DocumentationPage.GetDocTitle();

            string urlString = Browser.Url;
            //Remove the part "?products=" if there is any.
            if (Browser.Url.Contains("?product="))
            {
                int indexOfdocTitleEndInUrl = Browser.Url.IndexOf("?product=");
                urlString = Browser.Url.Substring(0, indexOfdocTitleEndInUrl);
            }
            Assert.IsTrue(
               urlString.Contains(docTitle.ToLower().Replace(" ", "-"))
               || urlString.Contains(tocPath[tocPath.Length - 1].ToLower().Replace(" ", "-")),
               @"when {0} is chosen in doc nav list, the shown content {1} should be consistent with the actual url {2}",
               parts[0],
               docTitle,
               Browser.Url);
        }

        /// <summary>
        /// Verify whether can add-in doc page's product can be changed
        /// </summary>
        [TestMethod]
        public void BVT_S18_TC09_CanChangeDocProduct()
        {
            Pages.Navigation.Select("Documentation", MenuItemOfDocumentation.OfficeAddin.ToString());
            string[] products = DocumentationPage.GetProducts();
            int randomIndex = new Random().Next(products.Length);
            DocumentationPage.ChangeProduct(products[randomIndex]);

            Assert.IsTrue(Browser.Url.EndsWith("office-add-ins?product=" + products[randomIndex].ToLower()),
                "The doc page's corresponding product should be {0}",
                products[randomIndex]);
        }

        //[TestMethod]
        //public void BVT_S18_TC10_CanRightClickToOpenDocInNewTab()
        //{
        //    Pages.Navigation.Select("Documentation", MenuItemOfDocumentation.OfficeAddin.ToString());

        //    int layerCount = DocumentationPage.GetTOCLayer();
        //    int randomIndex = new Random().Next(layerCount);
        //    string levelItem = DocumentationPage.GetTOCItem(randomIndex, true);
        //    string[] parts = levelItem.Split(new char[] { ',' });
        //    string[] tocPath = parts[0].Split(new char[] { '>' });
        //    string title = tocPath[tocPath.Length - 1];
        //    string link = parts[1];

        //    for (int j = 0; j < tocPath.Length-1; j++)
        //    {
        //        //Avoid to fold the sublayer
        //        if (!DocumentationPage.SubLayerDisplayed(tocPath[j]))
        //        {
        //            Utility.Click(tocPath[j]);
        //        }
        //    }
        //    DocumentationPage.RightClickToOpenInNewTab(tocPath[tocPath.Length - 1]);
        //    Assert.IsTrue(true);
        //}
    }
}

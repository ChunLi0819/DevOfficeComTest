using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestFramework;
using System.Drawing;
using System.Collections.Generic;

namespace MSGraphTest
{
    /// <summary>
    /// Summary description for DocumentationTest
    /// </summary>
    [TestClass]
    public class MSGraphDocumentationTest
    {
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

        /// <summary>
        /// Verify whether there is a toggle arrow which work correctly when the window is small.
        /// </summary>
        [TestMethod]
        public void Comps_Graph_S04_TC01_CanToggleArrowWorkInSmallDocumentaionPage()
        {
            int currentWidth = 0;
            int currentHeight = 0;
            GraphBrowser.GetWindowSize(out currentWidth, out currentHeight);
            GraphPages.Navigation.Select("Documentation");

            Size windowSize;
            //Set as the screen size of IPad2
            double deviceScreenSize = double.Parse(GraphUtility.GetConfigurationValue("IPad2Size"));
            GraphBrowser.TransferPhysicalSizeToPixelSize(
                deviceScreenSize,
                new Size
                {
                    Width = int.Parse(GraphUtility.GetConfigurationValue("IPad2ScreenResolutionWidth")),
                    Height = int.Parse(GraphUtility.GetConfigurationValue("IPad2ScreenResolutionHeight"))
                },
                out windowSize);
            GraphBrowser.SetWindowSize(windowSize.Width, windowSize.Height);

            Assert.IsTrue(
                GraphUtility.IsToggleArrowDisplayed(),
                "An IPad2 window size ({0} inches) can make table of content arrow appear.",
                deviceScreenSize);
            Assert.IsFalse(GraphUtility.IsMenuContentDisplayed(),
                "When the arrows exists, table of content should be hidden.");

            GraphUtility.ToggleMenu();
            Assert.IsTrue(GraphUtility.IsMenuContentDisplayed(),
                "When the arrows exists and table of content is hidden,clicking the arrow should show table of content.");

            GraphUtility.ToggleMenu();
            Assert.IsFalse(GraphUtility.IsMenuContentDisplayed(),
                "When the arrows exists and table of content is shown,clicking the arrow should hide table of content.");

            //Set as the screen size of IPhone6 plus
            deviceScreenSize = double.Parse(GraphUtility.GetConfigurationValue("IPhone6PlusSize"));
            //Since mobile phone width<Height, invert the output values
            GraphBrowser.TransferPhysicalSizeToPixelSize(
               deviceScreenSize,
               new Size
               {
                   Width = int.Parse(GraphUtility.GetConfigurationValue("IPhone6PlusScreenResolutionWidth")),
                   Height = int.Parse(GraphUtility.GetConfigurationValue("IPhone6PlusScreenResolutionHeight"))
               },
               out windowSize);
            //Since mobile phone widh<height, invert height and width
            GraphBrowser.SetWindowSize(windowSize.Height, windowSize.Width);

            Assert.IsTrue(
                GraphUtility.IsToggleArrowDisplayed(),
                "An IPhone6 Plus window size ({0} inches) can make table of content arrow appear.",
                deviceScreenSize);

            //Recover the window size
            GraphBrowser.SetWindowSize(currentWidth, currentHeight);
        }

        /// <summary>
        /// Verify whether toggle arrow hides when the window is large.
        /// </summary>
        [TestMethod]
        public void Acceptance_Graph_S04_TC02_CanToggleArrowHideInLargeDocumentaionPage()
        {
            int currentWidth = 0;
            int currentHeight = 0;
            GraphBrowser.GetWindowSize(out currentWidth, out currentHeight);
            GraphPages.Navigation.Select("Documentation");

            int actualWidth = 0;
            int actualHeight = 0;
            //Maxsize the window to see if it is possible to hide the arrow
            GraphBrowser.SetWindowSize(actualWidth, actualHeight, true);
            GraphBrowser.GetWindowSize(out actualWidth, out actualHeight);
            if (GraphUtility.IsToggleArrowDisplayed())
            {
                GraphBrowser.SetWindowSize(currentWidth, currentHeight);
                Assert.Inconclusive(
                    "A window size ({0}*{1}) is not big enough to hide table of content arrow",
                    actualWidth,
                    actualHeight);
            }
            else
            {
                //Set a common laptop size: 17.3 and a common screen resolution:1024*768
                double deviceScreenSize = 17.3;
                Size windowSize;
                GraphBrowser.TransferPhysicalSizeToPixelSize(
                    deviceScreenSize,
                    new Size
                    {
                        Width = 1024,
                        Height = 768
                    },
                    out windowSize);
                GraphBrowser.SetWindowSize(windowSize.Width, windowSize.Height);

                Assert.IsFalse(
                    GraphUtility.IsToggleArrowDisplayed(),
                    "An large window size ({0} inches) can make table of content arrow hide.",
                    deviceScreenSize);
            }
            //Recover the window size
            GraphBrowser.SetWindowSize(currentWidth, currentHeight);
        }

        /// <summary>
        /// Verify whether clicking different subject on Documentation page's
        /// table of content will show the correct doc content.
        /// </summary>
        [TestMethod]
        public void Comps_Graph_S04_TC03_CanDisplayCorrectContentOnDocumentaionPage()
        {
            GraphPages.Navigation.Select("Documentation");
            //If the table of content is replaced by the toggle arrow, click the arrow to display table of content
            if (GraphUtility.IsToggleArrowDisplayed())
            {
                GraphUtility.ToggleMenu();
            }
            int layerCount = GraphUtility.GetTOCLayer();
            int randomIndex = new Random().Next(layerCount);
            string levelItem = GraphUtility.GetTOCItem(randomIndex, true);
            string[] parts = levelItem.Split(new char[] { ',' });
            string[] tocPath = parts[0].Split(new char[] { '>' });
            string title = tocPath[tocPath.Length - 1];
            string link = parts[1];
            for (int j = 0; j < tocPath.Length; j++)
            {
                //Avoid to fold the sublayer
                if (!GraphUtility.SubLayerDisplayed(tocPath[j]))
                {
                    GraphUtility.Click(tocPath[j]);
                }
            }
            GraphBrowser.Wait(TimeSpan.FromSeconds(int.Parse(GraphUtility.GetConfigurationValue("WaitTime"))));
            bool isCorrectDoc = GraphUtility.ValidateDocument(link); 
            string docTitle = GraphUtility.GetDocTitle();
            
            Assert.IsTrue(
               isCorrectDoc,
               @"The shown content is {0} when {1} is chosen in the table of content on Documentation page",
               docTitle,
               parts[0]);
            for (int k = tocPath.Length - 1; k >= 0; k--)
            {
                GraphUtility.Click(tocPath[k]);
            }

        }

        /// <summary>
        /// Verify whether a sub menu can appear by clicking its parent layer.
        /// </summary>
        [TestMethod]
        public void Acceptance_Graph_S04_TC04_CanShowTOCSubLayer()
        {
            GraphPages.Navigation.Select("Documentation");
            if (!GraphUtility.IsMenuContentDisplayed())
            {
                GraphUtility.ToggleMenu();
            }

            int tocLayerCount = GraphUtility.GetTOCLayer();
            //Random generate a layer index, check a menu item at this layer from its top layers          
            //Because the last layer menu item doesn't have sub menu, use tocLayerCount-1 as the max value
            int index = new Random().Next(tocLayerCount - 1);

            List<string> tocPath = GraphUtility.FindTOCParentItems(index);
            string itemPath = string.Empty;
            for (int j = 0; j < tocPath.Count; j++)
            {
                GraphUtility.Click(tocPath[j]);
                itemPath += (j == 0 ? string.Empty : "->");
                itemPath += tocPath[j];
                Assert.IsTrue(GraphUtility.SubLayerDisplayed(tocPath[j]), "Clicking {0} can display its sub layer");
            }
            //Unfold the menu back
            for (int k = tocPath.Count - 1; k >= 0; k--)
            {
                GraphUtility.Click(tocPath[k]);
            }
        }

        /// <summary>
        /// Verify whether the arrow hide table of content, 
        /// and whether clicking it can hide/show table of content alternatively
        /// </summary>
        private void VerifyArrowAvailability()
        {
            Assert.IsFalse(
                 GraphUtility.IsMenuContentDisplayed(),
                 "Without clicking the toogle arrow, the menu content should not appear.");
            //Click the arrow to show the menu content
            GraphUtility.ToggleMenu();
            Assert.IsTrue(
                 GraphUtility.IsMenuContentDisplayed(),
                 "After clicking the toogle arrow, the menu content should appear.");

            //Click the arrow again to hide the menu content
            GraphUtility.ToggleMenu();
            Assert.IsFalse(
                 GraphUtility.IsMenuContentDisplayed(),
                 "After clicking the toogle arrow for the second time, the menu content should disappear.");
        }

    }
}

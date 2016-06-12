using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestFramework;

namespace Tests
{
    /// <summary>
    /// Test class for build videos page
    /// </summary>
    [TestClass]
    public class BuildVideoPageTest
    {
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

        [TestCleanup()]
        public void TestCleanup()
        {
            Browser.Goto(Browser.BaseAddress);
        }

        #endregion

        /// <summary>
        /// Check videos' "Get Started" links
        /// </summary>
        [TestMethod]
        public void BVT_S19_TC01_CheckGetStartedLinks()
        {
            Pages.Navigation.Select("Resources", "BuildVideos");
            string videoWithNoLink;
            BuildVideoPage.CheckGetStartedLinks(out videoWithNoLink);
            if (!videoWithNoLink.Equals(string.Empty))
            {
                Assert.Fail("{0}'s Get Started link does not refer to the correct page",
                    videoWithNoLink);
            }
        }

        /// <summary>
        /// Check build-video' Share icon and shared link
        /// </summary>
        [TestMethod]
        public void BVT_S19_TC02_CheckShareOnTwitter()
        {
            Pages.Navigation.Select("Resources", "BuildVideos");
            string videoTitle;
            string sharedLink;
            int randomIndex = new Random().Next(BuildVideoPage.VideoCount);
            BuildVideoPage.CheckShareOnTwitter(randomIndex, out videoTitle, out sharedLink);
            if (!videoTitle.Equals(string.Empty))
            {
                Assert.Fail(@"{0}'s Twitter-shared link: ""{1}"" is incorrect",
                    videoTitle,
                    sharedLink);
            }
        }

        /// <summary>
        /// Check whether a build video can be played by clicking image or title
        /// </summary>
        [TestMethod]
        public void BVT_S19_TC03_CanPlayVideo()
        {
            Pages.Navigation.Select("Resources", "BuildVideos");
            string message;
            Assert.IsTrue(!BuildVideoPage.IsVideoPlaying(out message) && message.Equals(string.Empty),
                "No video should be playing when go to //build videos page at first");
            int randomIndex = new Random().Next(BuildVideoPage.VideoCount);

            // Click Video's title
            BuildVideoPage.PlayVideoByClickTitle(randomIndex);
            Assert.IsTrue(BuildVideoPage.IsVideoPlaying(out message), message);
            BuildVideoPage.CloseVideo();

            //Click Video's image
            BuildVideoPage.PlayVideoByClickImage(randomIndex);
            Assert.IsTrue(BuildVideoPage.IsVideoPlaying(out message), message);
            BuildVideoPage.CloseVideo();
        }

        /// <summary>
        /// Check whether can play a video by going to the video's link directly
        /// </summary>
        [TestMethod]
        public void BVT_S19_TC04_CanPlayVideo()
        {
            Pages.Navigation.Select("Resources", "BuildVideos");
            string message;
            int randomIndex = new Random().Next(BuildVideoPage.VideoCount);

            // Click Video's title
            string videoLink=BuildVideoPage.GotoVideoLink(randomIndex);
            Assert.IsTrue(BuildVideoPage.IsVideoPlaying(out message), 
                "Video {0} should be playable by going to the link {1}. Actual status: {2}", 
                BuildVideoPage.CurrentVideoTitle,
                videoLink,
                message);
            BuildVideoPage.CloseVideo();

        }
    }
}

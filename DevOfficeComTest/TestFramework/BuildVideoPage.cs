using OpenQA.Selenium;
using System;

namespace TestFramework
{
    public class BuildVideoPage : BasePage
    {
        public static int VideoCount
        {
            get
            {
                return Browser.webDriver.FindElements(By.CssSelector("div.videoText")).Count;
            }
        }

        public static string CurrentVideoTitle = string.Empty;
        
        /// <summary>
        /// Check a build video's Twitter-shared link
        /// </summary>
        /// <param name="index">The index of the build video to check </param>
        /// <param name="videoTitle">The video's title if its shared link is incorrect</param>
        /// <param name="sharedLink">The video's shared link if it is incorrect</param>
        public static void CheckShareOnTwitter(int index, out string videoTitle, out string sharedLink)
        {
            var element = Browser.webDriver.FindElements(By.CssSelector("div.videoText>i.shareButton"))[index];
            string currentVideoTitle = element.FindElement(By.XPath("preceding-sibling::p[@class='video-title js-play']")).GetAttribute("id").Replace("-title", "");
            Browser.Click(element);
            var twitterLink = Browser.FindElement(By.XPath("//li[@class='at4m-listitem']/a[@title='Twitter']"));
            Browser.Click(twitterLink);
            Browser.SwitchToNewWindow();
            string sharedText = Browser.FindElement(By.CssSelector("textarea#status")).Text;
            string expectedSharedlink = Browser.BaseAddress + "/build-videos#?" + currentVideoTitle.Replace(" ", "-").ToLower();
            if (sharedText.EndsWith(expectedSharedlink))
            {
                videoTitle = string.Empty;
                sharedLink = string.Empty;
            }
            else
            {
                videoTitle = currentVideoTitle;
                sharedLink = sharedText.Replace("Office Dev Center - //build Videos ", "");
            }
            Browser.SwitchBack();
        }

        /// <summary>
        /// Check videos' GetStarted links
        /// </summary>
        /// <param name="videoTitle">The title of the first video whose Get Started link is incorrect</param>
        public static void CheckGetStartedLinks(out string videoTitle)
        {
            for (int i = 0; i < BuildVideoPage.VideoCount; i++)
            {
                var element = Browser.webDriver.FindElements(By.CssSelector("div.videoText>a"))[i];
                if (!element.GetAttribute("href").Equals(Browser.BaseAddress + "/getting-started"))
                {
                    videoTitle = element.FindElement(By.XPath("preceding-sibling::p[@class='video-title js-play']")).GetAttribute("id").Replace("-title", "");
                }
            }
            videoTitle = string.Empty;
        }

        /// <summary>
        /// Go to a video's link
        /// </summary>
        /// <param name="videoIndex">Video index</param>
        /// <returns>The video's expected link</returns>
        public static string GotoVideoLink(int videoIndex)
        {
            var element = Browser.webDriver.FindElements(By.CssSelector("div.hasDuration.video-image.js-play"))[videoIndex];
            string videoLink = Browser.BaseAddress + "/build-videos#?" + element.GetAttribute("id");
            CurrentVideoTitle = Browser.webDriver.FindElements(By.XPath("//div[@class='videoText']/p[@class='video-title js-play']"))[videoIndex].GetAttribute("title");
            Browser.Goto(videoLink);
            Browser.webDriver.Navigate().Refresh();
            return videoLink;
        }

        /// <summary>
        /// Click a video's title to play it
        /// </summary>
        /// <param name="videoIndex">Video index</param>
        public static void PlayVideoByClickTitle(int videoIndex)
        {
            var element = Browser.webDriver.FindElements(By.XPath("//div[@class='videoText']/p[@class='video-title js-play']"))[videoIndex];
            CurrentVideoTitle = element.GetAttribute("title");
            Browser.Click(element);

        }

        /// <summary>
        /// Click a video's image to play it
        /// </summary>
        /// <param name="videoIndex">Video index</param>
        public static void PlayVideoByClickImage(int videoIndex)
        {
            var element = Browser.webDriver.FindElements(By.CssSelector("div.video-container>div > img.playButtonImage"))[videoIndex];
            CurrentVideoTitle = element.FindElement(By.XPath("ancestor::div[@class='video-container']/parent::div/div[@class='videoText']/p[@class='video-title js-play']")).GetAttribute("title");
            Browser.Click(element);
        }

        /// <summary>
        /// Check whether any video is playing
        /// </summary>
        /// <param name="message">The check result. An empty string means no video is playing</param>
        /// <returns>True if yes, else false</returns>
        public static bool IsVideoPlaying(out string message)
        {
            var element = Browser.FindElement(By.CssSelector("div#videoModal"));
            if (element.Displayed)
            {
                var frameElement = Browser.FindElement(By.TagName("iframe"));
                Browser.webDriver.SwitchTo().Frame(frameElement);

                //Check whether the popped up video is correct
                string playedVideoTitle = Browser.FindElement(By.CssSelector("a.video")).GetAttribute("ms.cmpgrp").Replace(":controls", "");
                if (!playedVideoTitle.Equals(CurrentVideoTitle))
                {
                    message= string.Format("Expected played video: {0};Actual: {1}",
                        CurrentVideoTitle,
                        playedVideoTitle);
                    return false;
                }

                // Check whether the video's progressbar is working
                Browser.Wait(By.CssSelector("div.seek.control"));
                double oldTime = Convert.ToDouble(Browser.FindElement(By.CssSelector("div.seek.control")).GetAttribute("aria-valuenow"));
                Browser.Wait(TimeSpan.FromSeconds(Convert.ToInt32(int.Parse(Utility.GetConfigurationValue("WaitTime")))));
                double newTime = Convert.ToDouble(Browser.FindElement(By.CssSelector("div.seek.control")).GetAttribute("aria-valuenow"));

                Browser.webDriver.SwitchTo().DefaultContent();
                if (newTime - oldTime > 0)
                {
                    message = string.Format("Video {0} can be played",
                        playedVideoTitle);
                    return true;
                }
                else
                {
                    message= string.Format("Video {0} is not playable",
                        CurrentVideoTitle);
                    return false;
                }
            }
            message= string.Empty;
            return false;
        }

        /// <summary>
        /// Close the video frame
        /// </summary>
        public static void CloseVideo()
        {
            var closeEle = Browser.FindElement(By.CssSelector("a#videoClose"));
            Browser.Click(closeEle);
        }
    }
}

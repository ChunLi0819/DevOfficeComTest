using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TestFramework;

namespace Tests
{
    /// <summary>
    /// Test class for Code Sample Page
    /// </summary>
    [TestClass]
    public class CodeSamplePageTest
    {

        #region Additional test attributes
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Browser.Initialize();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Browser.Close();
        }
        [TestInitialize]
        public void TestInitialize()
        {
            Browser.SetWaitTime(TimeSpan.FromSeconds(Utility.DefaultWaitTime*3));
            Browser.Goto(Browser.BaseAddress + "/code-samples");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Browser.Goto(Browser.BaseAddress + "/code-samples");
            Utility.ExecuteClearFilters();
            Browser.SetWaitTime(TimeSpan.FromSeconds(Utility.DefaultWaitTime));
        }
        #endregion

        /// <summary>
        /// Verify if choosing the filters can cause the displayed results updated 
        /// </summary>
        [TestMethod]
        public void BVT_S13_TC01_FilterCheckedWithCorrectEvent()
        {
            int filterCount = Utility.GetFilterCount();
            for (int i = 0; i < filterCount; i++)
            {
                string filterName = Utility.SelectFilter(i);
                Assert.IsTrue(Utility.isFilterWorkable(i),
                    "The {0} filter should be valid to change displayed code samples",
                    filterName);
            }
        }

        /// <summary>
        /// Verify if all the checked filters can be cleared at one time
        /// </summary>
        [TestMethod]
        public void Acceptance_S13_TC02_CanFilterCleared()
        {
            int filterCount = Utility.GetFilterCount();

            //Generate the count of filters to check
            int checkedRandomFilterCount = new Random().Next(filterCount);
            //Generate the indexes of filters to check
            List<int> indexList = new List<int>();
            while (indexList.Count < checkedRandomFilterCount)
            {
                int randomIndex = new Random().Next(filterCount);
                if (!indexList.Contains(randomIndex))
                {
                    indexList.Add(randomIndex);
                    Utility.SelectFilter(randomIndex);
                }
            }

            Utility.ExecuteClearFilters();
            List<string> unclearedFilters;
            if (!Utility.areFiltersCleared(out unclearedFilters))
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (string unClearedFilter in unclearedFilters)
                {
                    stringBuilder.Append(unClearedFilter + ";");
                }
                Assert.Fail("The following filters are not cleared: {0}",
                    stringBuilder.ToString());
            }
        }

        /// <summary>
        /// Verify whether the URL can be updated accordingly when some filters are chosen
        /// </summary>
        [TestMethod]
        public void Acceptance_S13_TC03_CanURLUpdatedByCodeSampleFilters()
        {
            int filterCount = Utility.GetFilterCount();
            List<string> filterNames = new List<string>();

            //Generate the count of filters to select
            int checkedRandomFilterCount = new Random().Next(filterCount);
            //Generate the indexes of filters to select
            List<int> indexList = new List<int>();
            while (indexList.Count < checkedRandomFilterCount)
            {
                int randomIndex = new Random().Next(filterCount);
                if (!indexList.Contains(randomIndex))
                {
                    indexList.Add(randomIndex);
                    filterNames.Add(Utility.SelectFilter(randomIndex));
                }
            }

            List<string> unContainedFilters;
            if (!Utility.AreFiltersInURL(filterNames, out unContainedFilters))
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (string unContainedFilter in unContainedFilters)
                {
                    stringBuilder.Append(unContainedFilter + ";");
                }
                Assert.Fail("The following filters are not contained: {0}",
                    stringBuilder.ToString());
            }
        }

        /// <summary>
        /// Verify whether the code samples can be sorted by view count correctly
        /// </summary>
        [TestMethod]
        public void Acceptance_S13_TC04_CanSortCodeSamplesByViewCount()
        {
            int filterCount = Utility.GetFilterCount();

            //Randomly choose two filters to check
            int randomIndex;
            int usedIndex = filterCount;
            for (int i = 0; i < 2; i++)
            {
                do
                {
                    randomIndex = new Random().Next(filterCount);
                } while (randomIndex == usedIndex);
                string filterName = Utility.SelectFilter(randomIndex);

                // Set the sort order as descendent
                Utility.SetSortOrder(SortType.ViewCount, true);
                List<SearchedResult> resultList = Utility.GetFilterResults();
                for (int j = 0; j < resultList.Count - 1; j++)
                {
                    Assert.IsTrue(resultList[j].ViewCount >= resultList[j + 1].ViewCount,
                        @"When {0} filter is chosen and the sort order is descendent, the view count of ""{1}"" should be larger than or equal to its sibling ""{2}""'s",
                        filterName,
                        resultList[j].Name,
                        resultList[j + 1].Name);
                }

                // Set the sort order as ascendent
                Utility.SetSortOrder(SortType.ViewCount, false);
                resultList = Utility.GetFilterResults();
                for (int j = 0; j < resultList.Count - 1; j++)
                {
                    Assert.IsTrue(resultList[j].ViewCount <= resultList[j + 1].ViewCount,
                        @"When {0} filter is chosen and the sort order is ascendent, the view count of ""{1}"" should be smaller than or equal to its sibling ""{2}""'s",
                        filterName,
                        resultList[j].Name,
                        resultList[j + 1].Name);
                }

                usedIndex = randomIndex;
                Utility.ExecuteClearFilters();
            }
        }

        /// <summary>
        /// Verify whether the code samples can be sorted by the updated date correctly
        /// </summary>
        [TestMethod]
        public void Acceptance_S13_TC05_CanSortCodeSamplesByUpdatedDate()
        {
            int filterCount = Utility.GetFilterCount();
            int usedIndex = filterCount;

            int randomIndex = new Random().Next(filterCount);
            string filterName = Utility.SelectFilter(randomIndex);

            // Set the sort order as descendent
            Utility.SetSortOrder(SortType.Date, true);
            List<SearchedResult> resultList = Utility.GetFilterResults();

            for (int j = 0; j < resultList.Count - 1; j++)
            {
                Assert.IsTrue(resultList[j].UpdatedDate >= resultList[j + 1].UpdatedDate,
                    @"When {0} filter is chosen and the sort order is descendent, the updated date of ""{1}"" should be later than or equal to its sibling ""{2}""'s",
                    filterName,
                    resultList[j].Name,
                    resultList[j + 1].Name);
            }

            // Set the sort order as ascendent
            Utility.SetSortOrder(SortType.Date, false);
            resultList = Utility.GetFilterResults();
            for (int j = 0; j < resultList.Count - 1; j++)
            {
                Assert.IsTrue(resultList[j].UpdatedDate <= resultList[j + 1].UpdatedDate,
                    @"When {0} filter is chosen and the sort order is ascendent, the updated date of ""{1}"" should be earlier than or equal to its sibling ""{2}""'s",
                    filterName,
                    resultList[j].Name,
                    resultList[j + 1].Name);
            }
            Utility.ExecuteClearFilters();
        }

        /// <summary>
        /// Verify if choosing the filter SharePoint Add-ins can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC06_CanFindSharePointAddInSamples()
        {
            Utility.SelectFilter("SharePoint Add-ins");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("sharepoint add-ins");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("sharepoint add-ins");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter SharePoint Add-ins");
        }

        /// <summary>
        /// Verify if choosing the filter Office Add-ins can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC07_CanFindOfficeAddInSamples()
        {
            Utility.SelectFilter("Office Add-ins");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("office add-in");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("office add-in");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Office Add-in");
        }

        /// <summary>
        /// Verify if choosing the filter Office 365 App can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC08_CanFindOffice365AppSamples()
        {
            Utility.SelectFilter("Office 365 App");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("office 365");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("office 365");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Office 365 App");
        }

        /// <summary>
        /// Verify if choosing the filter AngularJS can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC09_CanFindAngularJSSamples()
        {
            Utility.SelectFilter("AngularJS");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("angularjs");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("angularjs");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter AngularJS");
        }

        /// <summary>
        /// Verify if choosing the filter C# can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC10_CanFindCSharpSamples()
        {
            Utility.SelectFilter("C#");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains(".net");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains(".net");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter C#");
        }

        /// <summary>
        /// Verify if choosing the filter Java can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC11_CanFindJavaSamples()
        {
            Utility.SelectFilter("Java");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("android");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("android");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Java");
        }

        /// <summary>
        /// Verify if choosing the filter node.js can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC12_CanFindNodejsSamples()
        {
            Utility.SelectFilter("node.js");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("node.js");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("node.js");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter node.js");
        }

        /// <summary>
        /// Verify if choosing the filter Objective C can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC13_CanFindObjectiveCSamples()
        {
            Utility.SelectFilter("Objective C");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("ios");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("ios");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Objective C");
        }

        /// <summary>
        /// Verify if choosing the filter PHP can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC14_CanFindPHPSamples()
        {
            Utility.SelectFilter("PHP");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("php");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("php");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter PHP");
        }

        /// <summary>
        /// Verify if choosing the filter Python can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC15_CanFindPythonSamples()
        {
            Utility.SelectFilter("Python");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("python");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("python");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Python");
        }

        /// <summary>
        /// Verify if choosing the filter Ruby on Rails can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC16_CanFindRubyonRailsSamples()
        {
            Utility.SelectFilter("Ruby on Rails");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("ruby on rails");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("ruby on rails");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Ruby on Rails");
        }

        /// <summary>
        /// Verify if choosing the filter Swift can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC17_CanFindSwiftSamples()
        {
            Utility.SelectFilter("Swift");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("ios app");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("ios");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Swift");
        }

        /// <summary>
        /// Verify if choosing the filter TypeScript can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC18_CanFindTypeScriptSamples()
        {
            Utility.SelectFilter("TypeScript");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("typescript");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("typescript");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter TypeScript");
        }

        /// <summary>
        /// Verify if choosing the filter XAML/C# can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC19_CanFindXAMLOrCSharpSamples()
        {
            Utility.SelectFilter("XAML/C#");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("api");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("api");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter XAML/C#");
        }

        /// <summary>
        /// Verify if choosing the filter ASP.NET MVC can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC20_CanFindMVCSamples()
        {
            Utility.SelectFilter("ASP.NET MVC");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("mvc");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("mvc");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter ASP.NET MVC");
        }

        /// <summary>
        /// Verify if choosing the filter HTML / JS can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC21_CanFindHTMLOrJSSamples()
        {
            Utility.SelectFilter("HTML / JS");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("javascript");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("javascript");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter HTML / JS");
        }

        /// <summary>
        /// Verify if choosing the filter ASP.NET Forms can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC22_CanFindFormSamples()
        {
            Utility.SelectFilter("ASP.NET Forms");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("web page");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("web page");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter ASP.NET Forms");
        }

        ///// <summary>
        ///// Verify if choosing the filter Silverlight can get any correct sample. 
        ///// </summary>
        //[TestMethod]
        //public void Comps_S13_TC23_CanFindSilverlightSamples()
        //{
        //    Pages.Navigation.Select("Code Samples");
        //    Utility.SelectFilter("Silverlight");
        //    //Currently no samples for Silverlight
        //}

        /// <summary>
        /// Verify if choosing the filter Azure AD Users and Groups can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC24_CanFindAzureADSamples()
        {
            Utility.SelectFilter("Azure AD Users and Groups");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            if (resultList.Count > 0)
            {
                foreach (SearchedResult resultInfo in resultList)
                {
                    bool isNameMatched = resultInfo.Name.ToLower().Contains("azure ad");
                    bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("azure ad");
                    if (isNameMatched || isDescriptionMatched)
                    {
                        hasFounded = true;
                        break;
                    }
                }
                if (!hasFounded)
                {
                    Assert.Inconclusive(@"Under ""Azure AD Users and Groups"" type, none of the searched results explicitly contain the searh text in title or description");
                }
            }
            else
            {
                Assert.Inconclusive(@"No code sample is found under ""Azure AD Users and Groups"" type");
            }
        }

        /// <summary>
        /// Verify if choosing the filter Office 365 Groups can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC25_CanFindOffice365GroupsSamples()
        {
            Utility.SelectFilter("Office 365 Groups");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("office 365 groups");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("office 365 groups");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Office 365 Groups");
        }

        /// <summary>
        /// Verify if choosing the filter Office Graph can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC26_CanFindOfficeGraphSamples()
        {
            Utility.SelectFilter("Office Graph");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("graph");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("graph");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Office Graph");
        }

        /// <summary>
        /// Verify if choosing the filter OneNote can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC27_CanFindOneNoteSamples()
        {
            Utility.SelectFilter("OneNote");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("onenote");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("onenote");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter OneNote");
        }

        /// <summary>
        /// Verify if choosing the filter Video Portal can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC28_CanFindVideoPortalSamples()
        {
            Utility.SelectFilter("Video Portal");
            List<SearchedResult> resultList = Utility.GetFilterResults();

            //The specific sample Property Manager contains Video Portal part
            Assert.IsTrue(resultList[0].Name.Equals("Property Manager"), "There should be at least one sample which meets the filter Video Portal");
        }

        ///// <summary>
        ///// Verify if choosing the filter SharePoint Taxonomy can get any correct sample. 
        ///// </summary>
        //[TestMethod]
        //public void Comps_S13_TC29_CanFindSharePointTaxonomySamples()
        //{
        //    Pages.Navigation.Select("Code Samples");
        //    Utility.SelectFilter("SharePoint Taxonomy");
        //    //Currently no samples for SharePoint Taxonomy
        //}

        ///// <summary>
        ///// Verify if choosing the filter Search can get any correct sample. 
        ///// </summary>
        //[TestMethod]
        //public void S13_TC30_CanFindSearchSamples()
        //{
        //    Utility.SelectFilter("Search");
        //    //Currently no samples for Search
        //}

        ///// <summary>
        ///// Verify if choosing the filter SharePoint User Profiles can get any correct sample. 
        ///// </summary>
        //[TestMethod]
        //public void Comps_S13_TC31_CanFindSharePointUserProfilesSamples()
        //{
        //    Utility.SelectFilter("SharePoint User Profiles");
        //    //Currently no samples for SharePoint User Profiles
        //}

        ///// <summary>
        ///// Verify if choosing the filter Business Connectivity Services can get any correct sample. 
        ///// </summary>
        //[TestMethod]
        //public void Comps_S13_TC32_CanFindBusinessConnectivityServicesSamples()
        //{
        //    Utility.SelectFilter("Business Connectivity Services");
        //    //Currently no samples for Business Connectivity Services
        //}

        /// <summary>
        /// Verify if choosing the filter SharePoint Workflow Services can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC33_CanFindSharePointWorkflowSamples()
        {
            Utility.SelectFilter("SharePoint Workflow");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                //The specifc sample Learning Path Manager AngularJS SharePoint Provider Hosted App includes workflow part
                bool isNameMatched = resultInfo.Name.Equals("Learning Path Manager AngularJS SharePoint Provider Hosted App");
                if (isNameMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter SharePoint Workflow");
        }

        // Don't have any result for Console Application platform filter now. Comment out this case for now.
        /// <summary>
        /// Verify if choosing the filter Console application can get any correct sample. 
        /// </summary>
        //[TestMethod]
        //public void Comps_S13_TC34_CanFindVideoPortalSamples()
        //{
        //    Utility.SelectFilter("Console application");
        //    List<SearchedResult> resultList = Utility.GetFilterResults();
        //    if (resultList.Count==0)
        //    {
        //        Assert.Inconclusive("Currently there are no code samples for Console application filter");
        //    }
        //}

        /// <summary>
        /// Verify if choosing the filter Web can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC35_CanFindWebSamples()
        {
            Utility.SelectFilter("Web");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("web");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("web");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Web");
        }

        /// <summary>
        /// Verify if choosing the filter Windows can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC36_CanFindWindowsSamples()
        {
            Utility.SelectFilter("Windows");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("windows");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("windows");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Windows");
        }

        /// <summary>
        /// Verify if choosing the filter Windows Phone can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC37_CanFindWindowsPhoneSamples()
        {
            Utility.SelectFilter("Windows Phone");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("phone");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("phone");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Windows Phone");
        }

        /// <summary>
        /// Verify if choosing the filter Android can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC38_CanFindAndroidSamples()
        {
            Utility.SelectFilter("Android");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("android");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("android");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Android");
        }

        /// <summary>
        /// Verify if choosing the filter IOS can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC39_CanFindIOSSamples()
        {
            Utility.SelectFilter("IOS");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("ios");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("ios");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter IOS");
        }

        /// <summary>
        /// Verify if choosing the filter Cordova can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC40_CanFindCordovaSamples()
        {
            Utility.SelectFilter("Cordova");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("cordova");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("cordova");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Cordova");
        }

        /// <summary>
        /// Verify if choosing the filter Xamarin can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC41_CanFindXamarinSamples()
        {
            Utility.SelectFilter("Xamarin");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("xamarin");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("xamarin");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Xamarin");
        }

        /// <summary>
        /// Verify if choosing the filter GitHub can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC42_CanFindGitHubSamples()
        {
            Utility.SelectFilter("GitHub");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                Browser.Goto(resultInfo.DetailLink);
                if (Utility.CanFindSourceLink("github.com"))
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter GitHub");
        }

        /// <summary>
        /// Verify if choosing the filter MSDN Code Gallery can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC43_CanFindMSDNCodeGallerySamples()
        {
            Utility.SelectFilter("MSDN Code Gallery");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {

                if (resultInfo.DetailLink.Contains("code.msdn.microsoft.com"))
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter MSDN Code Gallery");
        }

        /// <summary>
        /// Verify if choosing the filter CodePlex can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC44_CanFindCodePlexSamples()
        {
            Utility.SelectFilter("CodePlex");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {

                if (resultInfo.DetailLink.Contains("codeplex"))
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter CodePlex");
        }

        ///// <summary>
        ///// Verify if choosing the filter Other can get any correct sample. 
        ///// </summary>
        //[TestMethod]
        //public void Comps_S13_TC45_CanFindOtherSourceSamples()
        //{
        //    Utility.SelectFilter("Other");
        //    //Currently no samples for Other
        //}

        /// <summary>
        /// Verify if choosing the filter Azure Active Directory can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC46_CanFindAzureADSamples()
        {
            Utility.SelectFilter("Azure Active Directory");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("azure ad");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("azure ad");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Azure Active Directory");
        }

        /// <summary>
        /// Verify if choosing the filter OneDrive can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC47_CanFindOneDriveSamples()
        {
            Utility.SelectFilter("OneDrive");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                // The specific sample Office 365 Profile Angular sample involves OneDrive
                if (resultInfo.Name.Equals("Office 365 Profile Angular sample"))
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter OneDrive");
        }

        /// <summary>
        /// Verify if choosing the filter SharePoint can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC48_CanFindSharePointSamples()
        {
            Utility.SelectFilter("SharePoint");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("sharepoint");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("sharepoint");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter SharePoint");
        }

        /// <summary>
        /// Verify if choosing the filter Exchange can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC49_CanFindExchangeSamples()
        {
            Utility.SelectFilter("Exchange");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("mail");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("mail");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Exchange");
        }

        ///// <summary>
        ///// Verify if choosing the filter Lync can get any correct sample. 
        ///// </summary>
        //[TestMethod]
        //public void Comps_S13_TC50_CanFindLyncSamples()
        //{
        //    Utility.SelectFilter("Lync");
        //    //Currently no samples for Lync
        //}

        ///// <summary>
        ///// Verify if choosing the filter Skype can get any correct sample. 
        ///// </summary>
        //[TestMethod]
        //public void Comps_S13_TC51_CanFindSkypeSamples()
        //{
        //    Utility.SelectFilter("Skype");
        //    //Currently no samples for Skype
        //}

        /// <summary>
        /// Verify if choosing the filter Word can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC52_CanFindWordSamples()
        {
            Utility.SelectFilter("Word");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("word");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("word");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Word");
        }

        /// <summary>
        /// Verify if choosing the filter PowerPoint can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC53_CanFindPowerPointSamples()
        {
            Utility.SelectFilter("PowerPoint");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("powerpoint");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("powerpoint");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter PowerPoint");
        }

        /// <summary>
        /// Verify if choosing the filter Excel can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC54_CanFindExcelSamples()
        {
            Utility.SelectFilter("Excel");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("excel");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("excel");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Excel");
        }

        /// <summary>
        /// Verify if choosing the filter Outlook can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC55_CanFindOutlookSamples()
        {
            Utility.SelectFilter("Outlook");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("outlook");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("outlook");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Outlook");
        }

        /// <summary>
        /// Verify if choosing the filter Yammer can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC56_CanFindYammerSamples()
        {
            Utility.SelectFilter("Yammer");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                bool isNameMatched = resultInfo.Name.ToLower().Contains("yammer");
                bool isDescriptionMatched = resultInfo.Description.ToLower().Contains("yammer");
                if (isNameMatched || isDescriptionMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Yammer");
        }

        /// <summary>
        /// Verify if choosing the filter Delve can get any correct sample. 
        /// </summary>
        [TestMethod]
        public void Comps_S13_TC57_CanFindDelveSamples()
        {
            Utility.SelectFilter("Delve");
            bool hasFounded = false;

            List<SearchedResult> resultList = Utility.GetFilterResults();
            foreach (SearchedResult resultInfo in resultList)
            {
                //The specifc sample Graph Organization Explorer Windows 10 (UWP) includes Delve part
                bool isNameMatched = resultInfo.Name.Equals("Graph Organization Explorer Windows 10 (UWP)");
                if (isNameMatched)
                {
                    hasFounded = true;
                    break;
                }
            }
            Assert.IsTrue(hasFounded, "There should be at least one sample which meets the filter Delve");
        }
    }
}

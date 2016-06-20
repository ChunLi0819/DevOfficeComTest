using System;
using System.ComponentModel;
using System.Configuration;

namespace SearchEngineTest
{
    public enum SearchSite
    {
        [Description("Graph Microsoft")]
        GraphMS,
        [Description("Microsoft Graph")]
        MSGraph,
        [Description("Microsoft Graph API")]
        MSGraphAPI,
        [Description("dev office")]
        DevOffice,
        [Description("Office Dev Center")]
        OfficeDevCenter,
        [Description("Office Add-ins")]
        OfficeAddins
    }

    /// <summary>
    /// This struct contains the searched result's information
    /// </summary>
    public struct SearchedResult
    {
        /// <summary>
        /// The result name
        /// </summary>
        public string Name;

        /// <summary>
        /// The detail link
        /// </summary>
        public string DetailLink;
    }
}
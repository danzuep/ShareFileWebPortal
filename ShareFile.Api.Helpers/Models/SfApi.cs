using System;
using System.Collections.Generic;
using System.Text;

namespace ShareFile.Api.Helpers.Models
{
    internal class SfApi
    {
        //https://api.sharefile.com/gettingstarted/odata
        public static string Groups = "Groups({0})";
        public static string GroupContacts = "Groups({0})/Contacts";
        public static string Users = "Users({0})";
        public static string UserGroups = "Users({0})/Groups";
        public static string ItemAccessControls = "Items({0})/AccessControls";
        public static string FolderAccessControlDomains = "Accounts/FolderAccessControlDomains";
    }
}

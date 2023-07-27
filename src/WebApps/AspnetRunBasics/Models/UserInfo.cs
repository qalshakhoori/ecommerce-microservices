using System.Collections.Generic;

namespace AspnetRunBasics.Models
{
    public class UserInfo
    {
        public Dictionary<string,string> UserInfoDictionary { get; set; }

        public UserInfo (Dictionary<string, string> userInfoDictionary)
        {
            UserInfoDictionary = userInfoDictionary;
        }
    }
}

#region License Information
// LearningStudio RESTful API Libraries 
// These libraries make it easier to use the LearningStudio Course APIs.
// Full Documentation is provided with the library. 
// 
// Need Help or Have Questions? 
// Please use the PDN Developer Community at https://community.pdn.pearson.com
//
// @category   LearningStudio Course APIs
// @author     Wes Williams <wes.williams@pearson.com>
// @author     Pearson Developer Services Team <apisupport@pearson.com>
// @copyright  2014 Pearson Education Inc.
// @license    http://www.apache.org/licenses/LICENSE-2.0  Apache 2.0
// @version    1.0
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion License Information

using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using Com.Pearson.Pdn.Learningstudio.OAuth.Config;
using Com.Pearson.Pdn.Learningstudio.OAuth.Request;
using Com.Pearson.Pdn.Learningstudio.Utility;
using log4net;

namespace Com.Pearson.Pdn.Learningstudio.OAuth
{
    public class OAuth2PasswordService : OAuthService
    {
        private const string API_DOMAIN = "https://api.learningstudio.com";
        private OAuth2PasswordConfig oAuth2PasswordConfig;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OAuth2PasswordService));

        #region Public methods

        public OAuth2PasswordService(OAuth2PasswordConfig oAuth2PasswordConfig)
        {
            this.oAuth2PasswordConfig = oAuth2PasswordConfig;
        }
        
        /// <summary>
        /// Method to generate OAuth2 Request for the given username and password
        /// </summary>
        /// <param name="username">string</param>
        /// <param name="password">string</param>
        /// <returns><see cref="OAuth2Request"/></returns>
        public virtual OAuth2Request GenerateOAuth2PasswordRequest(string username, string password)
        {
            OAuth2Request oAuth2Request = null;
            string grantType = "password";
           
            string applicationId = oAuth2PasswordConfig.ApplicationId;
            string clientString = oAuth2PasswordConfig.ClientString;

            try
            {
                // Create the data to send
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("grant_type=" + Uri.EscapeDataString(grantType));
                stringBuilder.Append("&client_id=" + Uri.EscapeDataString(applicationId));
                stringBuilder.Append("&username=" + Uri.EscapeDataString(clientString + "\\" + username));
                stringBuilder.Append("&password=" + Uri.EscapeDataString(password));

                // Create a byte array of the data to be sent
                byte[] byteArray = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                oAuth2Request = OAuth2AssertionService.CreateOAuth2Request(byteArray);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GenerateOAuth2PasswordRequest: ", ex);
                throw;
            }

            return oAuth2Request;
        }

        /// <summary>
        /// Method to refresh the token for given OAuth2 Request
        /// </summary>
        /// <param name="inputOAuth2Request"><see cref="OAuth2Request"/></param>
        /// <returns><see cref="OAuth2Request"/></returns>
        public virtual OAuth2Request RefreshOAuth2PasswordRequest(OAuth2Request inputOAuth2Request)
        {
            string grantType = "refresh_token";
            string applicationId = oAuth2PasswordConfig.ApplicationId;

            OAuth2Request oAuth2Request = null;

            try
            {
                // Create the data to send
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("grant_type=" + Uri.EscapeDataString(grantType));
                stringBuilder.Append("&client_id=" + Uri.EscapeDataString(applicationId));
                stringBuilder.Append("&refresh_token=" + Uri.EscapeDataString(inputOAuth2Request.RefreshToken));

                byte[] byteArray = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                oAuth2Request = OAuth2AssertionService.CreateOAuth2Request(byteArray);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from RefreshOAuth2PasswordRequest: ", ex);
                throw;
            }

            return oAuth2Request;
        }

        #endregion
    }
}

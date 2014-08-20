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
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Text;
using Com.Pearson.Pdn.Learningstudio.OAuth.Config;
using Com.Pearson.Pdn.Learningstudio.OAuth.Request;
using Com.Pearson.Pdn.Learningstudio.Utility;
using log4net;

namespace Com.Pearson.Pdn.Learningstudio.OAuth
{
    /// <summary>
    /// OAuth2AssertionService
    /// </summary>
    public class OAuth2AssertionService : OAuthService
    {
        private const string API_DOMAIN = "https://api.learningstudio.com";
        private OAuth2AssertionConfig oAuth2AssertionConfig;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OAuth2AssertionService));

        #region Public methods

        /// <summary>
        /// Constructor takes OAuth2AssertionConfig
        /// </summary>
        /// <param name="configuration"><s</param>
        public OAuth2AssertionService(OAuth2AssertionConfig configuration)
        {
            this.oAuth2AssertionConfig = configuration;
        }

        /// <summary>
        /// Demonstrates how to make an access token request on behalf of a LearningStudio user via the OAuth 2.0 Assertion grant type
        /// </summary>
        /// <param name="username">string</param>
        /// <returns><see cref="OAuth2Request"/></returns>
        public OAuth2Request GenerateOAuth2AssertionRequest(string username)
        {
            string grantType = "assertion";
            string assertionType = "urn:ecollege:names:moauth:1.0:assertion";

            string applicationId = oAuth2AssertionConfig.ApplicationId;
            string applicationName = oAuth2AssertionConfig.ApplicationName;
            string consumerKey = oAuth2AssertionConfig.ConsumerKey;
            string clientString = oAuth2AssertionConfig.ClientString;
            string consumerSecret = oAuth2AssertionConfig.ConsumerSecret;

            OAuth2Request oAuth2Request = null;

            try
            {
                // Create the Assertion String
                string assertion = BuildAssertion(applicationName, consumerKey, applicationId, clientString, username, consumerSecret);

                // Generate data to be posted
                byte[] byteArray = GenerateStreamDataToWrite(grantType, assertionType, assertion);

                // Create OAuth2 request
                oAuth2Request = CreateOAuth2Request(byteArray);        
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GenerateOAuth2AssertionRequest: ", ex);
                throw;
            }

            return oAuth2Request;
        }

        /// <summary>
        /// Creates OAuth2 Request using given byte array collection
        /// </summary>
        /// <param name="byteArray">byte[]</param>
        /// <returns><see cref="OAuth2Request"/></returns>
        internal static OAuth2Request CreateOAuth2Request(byte[] byteArray)
        {
            string url = API_DOMAIN + "/token";
            string accessToken = string.Empty;
            string refreshToken = string.Empty;
            int expiresIn = 0;

            HttpWebResponse httpWebResponse = null;
            OAuth2Request oAuth2Request = null;

            try
            {
                // Setup the request
                string httpResponse = DoHTTPPost(url, ref httpWebResponse, byteArray);

                // Parse Access token, expires in values from Http Response
                ParseValuesFromHttpResponse(httpResponse, out accessToken, out expiresIn, out refreshToken);

                if (string.IsNullOrEmpty(accessToken))
                    throw new IOException("Missing Access Token");

                // Setup OAuth2 Request
                oAuth2Request = GenerateOAuth2Request(accessToken, expiresIn, refreshToken);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from CreateOAuth2Request: ", ex);
                throw;
            }
            finally
            {
                if (httpWebResponse != null)
                    httpWebResponse.Close();
            }

            return oAuth2Request;
        }

        #endregion 

        #region Private methods

        /// <summary>
        /// Generates OAuth2 request
        /// </summary>
        /// <param name="accessToken">string</param>
        /// <param name="expiresIn">int</param>
        /// <returns><see cref="OAuth2Request"/></returns>
        private static OAuth2Request GenerateOAuth2Request(string accessToken, int expiresIn, string refreshToken)
        {
            OAuth2Request oAuth2Request = new OAuth2Request();
            oAuth2Request.AccessToken = accessToken;
            oAuth2Request.ExpiresInSeconds = expiresIn;
            oAuth2Request.RefreshToken = refreshToken;
            oAuth2Request.CreationTime = DateTimeHelper.CurrentUnixTimeMilliSeconds();
            oAuth2Request.Headers = GenerateOAuth2RequestHeaders(accessToken);
            
            return oAuth2Request;
        }

        /// <summary>
        /// Parse the access token and expires in values from output response 
        /// </summary>
        /// <param name="jsonResponse">string</param>
        /// <param name="accessToken">string</param>
        /// <param name="expiresIn">int</param>
        private static void ParseValuesFromHttpResponse(string jsonResponse, out string accessToken, out int expiresIn, out string refreshToken)
        {
            refreshToken = null;

            // Retrieve and Return the Access Token
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            Dictionary<string, object> dictionary = javaScriptSerializer.Deserialize<Dictionary<string, object>>(jsonResponse);
            accessToken = dictionary["access_token"].ToString();
            expiresIn = Convert.ToInt32(dictionary["expires_in"]);
            if (dictionary.ContainsKey("refresh_token"))
                refreshToken = dictionary["refresh_token"].ToString();
        }

        private static string DoHTTPPost(string url, ref HttpWebResponse httpWebResponse, byte[] byteArray)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.UserAgent = "LS-Library-OAuth-CSharp-V1";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            // Write data to the stream
            Stream postStream = request.GetRequestStream();
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();

            // Send Request 
            httpWebResponse = (HttpWebResponse)request.GetResponse();

            // Get Response
            StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream());
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Generate the byte[] based on grantType, assertionType and assertion
        /// </summary>
        /// <param name="grantType">string</param>
        /// <param name="assertionType">string</param>
        /// <param name="assertion">string</param>
        /// <returns>byte[]</returns>
        private byte[] GenerateStreamDataToWrite(string grantType, string assertionType, string assertion)
        {
            // Create the data to send
            StringBuilder data = new StringBuilder();
            data.Append("grant_type=" + Uri.EscapeDataString(grantType));
            data.Append("&assertion_type=" + Uri.EscapeDataString(assertionType));
            data.Append("&assertion=" + Uri.EscapeDataString(assertion));

            // Create a byte array of the data to be sent
            byte[] byteArray = Encoding.UTF8.GetBytes(data.ToString());
            return byteArray;
        }

        /// <summary>
        /// Generate OAuth2 Request headers
        /// </summary>
        /// <param name="accessToken">string</param>
        /// <returns>Dictionary of <string, string></returns>
        private static IDictionary<string, string> GenerateOAuth2RequestHeaders(string accessToken)
        {
            IDictionary<string, string> headers = new SortedDictionary<string, string>();
            headers["X-Authorization"] = "Access_Token access_token=" + accessToken;
            return headers;
        }

        /// <summary>
        /// Builds a signed OAuth 2.0 assertion
        /// </summary>
        /// <param name="applicationName">string</param>
        /// <param name="keyMoniker">string</param>
        /// <param name="applicationID">string</param> 
        /// <param name="clientString">string</param>
        /// <param name="username">string</param>
        /// <param name="secret">string</param>
        /// <returns>string</returns>
        private string BuildAssertion(string applicationName, string keyMoniker, string applicationID, string clientString, string username, string secret)
        {
            // Get the UTC Date Timestamp
            string timestamp = DateTime.UtcNow.ToString("s") + "Z";

            // Setup the Assertion String
            string assertion = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", applicationName, keyMoniker, applicationID,
              clientString, username, timestamp);

            return String.Format("{0}|{1}", assertion, AuthHelper.GenerateCmac(secret, assertion, EncodingType.HEX));
        }

        #endregion
    }
}

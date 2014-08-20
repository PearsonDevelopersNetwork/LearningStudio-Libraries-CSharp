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
using System.Linq;
using System.Text;
using System.IO;
using Com.Pearson.Pdn.Learningstudio.OAuth.Config;
using Com.Pearson.Pdn.Learningstudio.OAuth.Request;
using Com.Pearson.Pdn.Learningstudio.Utility;
using log4net;

namespace Com.Pearson.Pdn.Learningstudio.OAuth
{
    public class OAuth1SignatureService : OAuthService
    {
        private const string SIGNATURE_METHOD = "CMAC-AES";
        private OAuth1SignatureConfig oAuth1SignatureConfig;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OAuth1SignatureService));

        #region Public Methods

        public OAuth1SignatureService(OAuth1SignatureConfig oAuth1SignatureConfig)
        {
            this.oAuth1SignatureConfig = oAuth1SignatureConfig;
        }

        /// <summary>
        /// This method generates <see cref="OAuth1Request">object</see> for the given HttpMethod, URL and the body
        /// </summary>
        /// <param name="httpMethod">HttpMethod</param>
        /// <param name="url">Uri</param>
        /// <param name="body">string</param>
        /// <returns><see cref="OAuth1Request"/>object</returns>
        public OAuth1Request GenerateOAuth1Request(HttpMethod httpMethod, Uri url, string body)
        {
            string applicationId = oAuth1SignatureConfig.ApplicationId;
            string consumerKey = oAuth1SignatureConfig.ConsumerKey;
            string consumerSecret = oAuth1SignatureConfig.ConsumerSecret;
            MemoryStream memoryStream = null;
            OAuth1Request oAuth1Request = null;

            try
            {
                // Set the Nonce and Timestamp parameters
                string nonce = AuthHelper.GetNonce();
                string timeStamp = DateTimeHelper.GetTimestamp();

                // Set the request body if making a POST or PUT request
                if (httpMethod == HttpMethod.POST || httpMethod == HttpMethod.PUT)
                    memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(body));

                // Create the OAuth parameter name/value pair
                Dictionary<string, string> oauthParams =
                    SetOAuthParameters(httpMethod, url, applicationId, consumerKey, consumerSecret, memoryStream, nonce, timeStamp);

                // Create the OAuth parameter comma delimited string
                string oauthParamsString = GenerateOAuthParameterString(oauthParams);

                // Create new OAuth1Request
                oAuth1Request = new OAuth1Request();
                oAuth1Request.Signature = oauthParams["oauth_signature"];
                oAuth1Request.Headers = GenerateOAuth1RequestHeaders(url, oauthParamsString); ;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GenerateOAuth1Request: ", ex);
                throw;
            }

            return oAuth1Request;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates OAuth1 Request headers
        /// </summary>
        /// <param name="url">Uri</param>
        /// <param name="oAuth1Request"><see cref="OAuth1Request">object</see></param>
        /// <returns>IDictionary of <string, string></returns>
        private IDictionary<string, string> GenerateOAuth1RequestHeaders(Uri url, string oauthParamsString)
        {
            // Omit the queryString from the url
            string urlString = url.ToString();
            int startOfQueryString = urlString.IndexOf('?');
            if (startOfQueryString != -1)
                urlString = urlString.Substring(0, startOfQueryString);

            // Build the X-Authorization request header
            string xAuthorization = string.Format("OAuth realm=\"{0}\",{1}", urlString, oauthParamsString);
            IDictionary<string, string> headers = new SortedDictionary<string, string>();
            headers["X-Authorization"] = xAuthorization;

            return headers;
        }

        /// <summary>
        /// Generates OAuth1 Request signature
        /// </summary>
        /// <param name="oauthParams">Dictionary of <string, string></param>
        /// <returns>string</returns>
        private string GenerateOAuthParameterString(Dictionary<string, string> oauthParams)
        {
            // Defines a query that produces a set of: keyname="URL-encoded(value)"
            IEnumerable<string> encodedParams =
                    from param in oauthParams
                    select string.Concat(param.Key, "=\"", Uri.EscapeDataString(param.Value), "\"");

            // Join all encoded parameters with a comma delimiter and convert to a string
            return String.Join(",", encodedParams);
        }

        /// <summary>
        /// Method sets the OAuth Parameters 
        /// </summary>
        /// <param name="httpMethod"><see cref="HttpMethod"/></param>
        /// <param name="url">Uri</param>
        /// <param name="applicationId">string</param>
        /// <param name="consumerKey">string</param>
        /// <param name="consumerSecret">string</param>
        /// <param name="memoryStream"><see cref="MemoryStream"/></param>
        /// <param name="nonce">string</param>
        /// <param name="timeStamp">string</param>
        /// <returns>Dictionary of <string, string></returns>
        private Dictionary<string, string> SetOAuthParameters(HttpMethod httpMethod, Uri url, string applicationId, string consumerKey, string consumerSecret, MemoryStream memoryStream, string nonce, string timeStamp)
        {
            Dictionary<string, string> oauthParams = new Dictionary<string, string>();
            oauthParams["oauth_consumer_key"] = consumerKey;
            oauthParams["application_id"] = applicationId;
            oauthParams["oauth_signature_method"] = SIGNATURE_METHOD;
            oauthParams["oauth_timestamp"] = timeStamp;
            oauthParams["oauth_nonce"] = nonce;
            oauthParams["oauth_signature"] = GenerateSignature(httpMethod, url, oauthParams, memoryStream, consumerSecret);

            return oauthParams;
        }

        /// <summary>
        /// Normalizes all oauth signable parameters and url query parameters according to OAuth 1.0.
        /// </summary>
        /// <param name="httpMethod"><see cref="HttpMethod"/></param>
        /// <param name="url">Uri</param>
        /// <param name="oauthParams">Dictionary of <string, string></param>
        /// <param name="requestBody">Stream</param>
        /// <returns>string</returns>        
        private string NormalizeParams(HttpMethod httpMethod, Uri url, IDictionary<string, string> oauthParams, Stream requestBody)
        {
            // Sort the parameters in lexicographical order, 1st by Key then by Value
            IEnumerable<KeyValuePair<string, string>> keyValueParams = oauthParams;
 
            // Place any Query String parameters into a key value pair using equals ("=") to mark
            // the key/value relationship and join each paramter with an ampersand ("&")
            if (!String.IsNullOrWhiteSpace(url.Query))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams =
                      from p in url.Query.Substring(1).Split('&').AsEnumerable()
                      let key = Uri.EscapeDataString(p.Substring(0, p.IndexOf("=")))
                      let value = Uri.EscapeDataString(p.Substring(p.IndexOf("=") + 1))
                      select new KeyValuePair<string, string>(key, value);

                keyValueParams = keyValueParams.Union(queryParams);
            }

            // Include the body parameter if dealing with a POST or PUT request
            if (httpMethod == HttpMethod.POST || httpMethod == HttpMethod.PUT)
            {
                MemoryStream memoryStream = new MemoryStream();
                requestBody.CopyTo(memoryStream);
                byte[] bodyBytes = memoryStream.ToArray();

                string body = Convert.ToBase64String(bodyBytes, Base64FormattingOptions.None);
                body = Uri.EscapeDataString(body);

                keyValueParams = keyValueParams.Union(new[]
                        {
                          new KeyValuePair<string, string>("body", Uri.EscapeDataString(body))
                        });
            }

            // Sort the parameters in lexicographical order, 1st by Key then by Value; separate with ("=")
            IEnumerable<string> sortedParams =
                  from p in keyValueParams
                  orderby p.Key ascending, p.Value ascending
                  select string.Concat(p.Key, "=",p.Value);

            // Add the ampersand delimiter and then URL-encode the equals ("%3D") and ampersand ("%26")
            return Uri.EscapeDataString(String.Join("&", sortedParams));
        }

        /// <summary>
        /// Generates an OAuth 1.0 signature.
        /// </summary>
        /// <param name="httpMethod"><see cref="HttpMethod"/></param>
        /// <param name="url">Uri</param>
        /// <param name="oauthParams">Dictionary of <string, string></param>
        /// <param name="requestBody">Stream</param>
        /// <param name="secret">string</param>
        /// <returns>string</returns>        
        private string GenerateSignature(HttpMethod httpMethod, Uri url, IDictionary<string, string> oauthParams, Stream requestBody, string secret)
        {
            // Construct the URL-encoded OAuth parameter portion of the signature base string
            string encodedParams = NormalizeParams(httpMethod, url, oauthParams, requestBody);

            // URL-encode the relative URL
            string encodedUri = Uri.EscapeDataString(url.AbsolutePath);

            // Build the signature base string to be signed with the Consumer Secret
            string baseString = String.Format("{0}&{1}&{2}", httpMethod, encodedUri, encodedParams);

            return AuthHelper.GenerateCmac(secret, baseString, EncodingType.BASE64);
        }

        #endregion
    }
}

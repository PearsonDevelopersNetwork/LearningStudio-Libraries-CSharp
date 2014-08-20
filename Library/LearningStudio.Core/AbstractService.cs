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
using System.Text;
using log4net;
using Com.Pearson.Pdn.Learningstudio.OAuth;
using Com.Pearson.Pdn.Learningstudio.OAuth.Request;
using Com.Pearson.Pdn.Learningstudio.Utility;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace Com.Pearson.Pdn.Learningstudio.Core
{
    /// <summary>
    /// Abstract Service Class
    /// </summary>
    public abstract class AbstractService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AbstractService));
        private const string API_DOMAIN = "https://api.learningstudio.com";
        private const string PATH_SYSTEMDATETIME = "/systemDateTime";
        public string NO_CONTENT = string.Empty;
        private OAuthServiceFactory oAuthServiceFactory;
        private AuthMethod authMethod;
        private string username;
        private string password;
        public DataFormat? dataFormat;
        private OAuthRequest currentOAuthRequest;

        /// <summary>
        /// The format that data is accepted and returned
        /// </summary>
        public enum DataFormat
        {
            JSON,
            XML
        }

        /// <summary>
        /// The method of authentication
        /// </summary>
        public enum AuthMethod
        {
            OAUTH1_SIGNATURE,
            OAUTH2_ASSERTION,
            OAUTH2_PASSWORD
        }

        #region Public methods

        /// <summary>
        /// Constructs an AbstractService
        /// </summary>
        /// <param name="oAuthServiceFactory">Service provider for OAuth operations</param>
        public AbstractService(OAuthServiceFactory oAuthServiceFactory)
        {
            this.oAuthServiceFactory = oAuthServiceFactory;
            this.dataFormat = DataFormat.JSON;            
        }

        /// <summary>
        /// Get name of service for identification purposes
        /// </summary>
        public abstract string ServiceIdentifier
        {
            get;
        }

        /// <summary>
        /// Get DataFormat preferred by operations
        /// </summary>
        public DataFormat PreferredFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Makes all future request use OAuth1 security
        /// </summary>
        public void UseOAuth1()
        {
            this.authMethod = AuthMethod.OAUTH1_SIGNATURE;
            this.username = null;
            this.password = null;
            this.currentOAuthRequest = null;
        }

        /// <summary>
        ///  Makes all future request use OAuth2 assertion security
        /// </summary>
        /// <param name="username"> </param>
        public void UseOAuth2(string username)
        {
            this.authMethod = AuthMethod.OAUTH2_ASSERTION;
            this.username = username;
            this.password = null;
            this.currentOAuthRequest = null;
        }

        /// <summary>
        /// Makes all future request use OAuth2 password security
        /// </summary>
        /// <param name="username"> </param>
        /// <param name="password"> </param>
        public void UseOAuth2(string username, string password)
        {
            this.authMethod = AuthMethod.OAUTH2_PASSWORD;
            this.username = username;
            this.password = password;
            this.currentOAuthRequest = null;
        }

        /// <summary>
        /// Performs HTTP operations using the selected authentication method
        /// </summary>
        /// <param name="method" value="HTTP Method to use">The HTTP Method to user</param>
        /// <param name="relativeUrl">The URL after .com (/me)</param>
        /// <param name="body">The body of the message</param>
        /// <returns>Output in the preferred data format</returns>        
        public virtual Response DoMethod(HttpMethod method, string relativeUrl, string body)
        {
            return DoMethod(null, method, relativeUrl, body);
        }

        /// <summary>
        /// Performs HTTP operations using the selected authentication method
        /// </summary>
        /// <param name="extraHeaders">Extra headers to include in the request</param>
        /// <param name="method">The HTTP Method to user</param>
        /// <param name="relativeUrl">The URL after .com (/me)</param>
        /// <param name="body">The body of the message</param>
        /// <returns>Output in the preferred data format</returns>
        public virtual Response DoMethod(IDictionary<string, string> extraHeaders, HttpMethod method, string relativeUrl, string body)
        {
            Uri url = null;
            Stream stream = null;
            Response response = null;
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            IDictionary<string, string> oauthHeaders = null;

            // Build OAuth Headers and add extra headers 
            InitializeHeaders(extraHeaders, method, ref relativeUrl, body, ref url, ref oauthHeaders);

            try
            {
                // Initialize Web Request
                httpWebRequest = InitializeWebRequest(method, url, oauthHeaders);
                httpWebRequest.UserAgent = ServiceIdentifier;

                // Write data to stream
                if ((method == HttpMethod.POST || method == HttpMethod.PUT) && body.Length > 0)
                    stream = WriteDataToStream(body, httpWebRequest, stream);

                // Build Response object from Http Web response
                response = ReadHttpResponse(method, url, httpWebRequest, ref httpWebResponse);

                if (response.StatusCode >= HttpStatusCode.BadRequest) return response;

                // Read data from response stream
                SetResponseContent(response, httpWebResponse);

                // Read binary data from stream, if exist
                ReadBinaryContent(stream, response, httpWebRequest);
            }

            catch (Exception ex)
            {
                Logger.Error("Exception from DoMethod: ", ex);
                throw;
            }
            finally
            {
                if (httpWebResponse != null)
                    httpWebResponse.Close();
            }

            return response;
        }
        
        /// <summary>
        /// Performs time lookup with /systemDateTime using OAuth1 or OAuth2
        /// </summary>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>
        public Response SystemDateTime
        {
            get
            {
                return DoMethod(HttpMethod.GET, PATH_SYSTEMDATETIME, NO_CONTENT);
            }
        }

        /// <summary>
        /// Performs time lookup with /systemDateTime using OAuth1 or OAuth2
        /// </summary>
        /// <returns> The milliseconds since the unix epoch </returns>
        /// <exception cref="IOException"> </exception>
        public long GetSystemDateTimeMillis()
        {
            long ticks = 0;
            string timeValue = string.Empty;
            
            try
            {
                Response response = SystemDateTime;
                if (response.Error)
                    throw new IOException(string.Format("Time lookup failed: {0} - {1}", response.StatusCode, response.StatusMessage));

                if (PreferredFormat == DataFormat.XML)
                    timeValue = ParseTimeValue(response.Content);
                else
                {
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    var dictionary = javaScriptSerializer.Deserialize<Dictionary<string, object>>(response.Content);
                    Dictionary<string, object> dictionaryValues = (Dictionary<string, object>)dictionary["systemDateTime"];
                    timeValue = dictionaryValues["value"].ToString();
                }

                TimeZone zone = TimeZone.CurrentTimeZone;
                ticks = zone.ToUniversalTime(Convert.ToDateTime(DateTime.Parse(timeValue))).Ticks;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetSystemDateTimeMillis: ", ex);
                throw;
            }

            return ticks;
        }

        /// <summary>
        /// Release all handles to objects 
        /// </summary>
        public virtual void Destroy()
        {
            this.oAuthServiceFactory = null;
            this.username = null;
            this.password = null;
            this.dataFormat = null;            
            this.currentOAuthRequest = null;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Read the binary data from Stream
        /// </summary>
        /// <param name="stream"><see cref="Stream"/>object</param>
        /// <param name="response"><see cref="Response"/>object</param>
        /// <param name="httpWebRequest">HttpWebRequest</param>
        private void ReadBinaryContent(Stream stream, Response response, HttpWebRequest httpWebRequest)
        {
            string contentType = string.Empty;
            
            //if (stream == null) return;

            if (httpWebRequest.ContentType == null)
                contentType = response.ContentType;
            else
                contentType = httpWebRequest.ContentType;

            if (contentType == null) return;

            if (!contentType.StartsWith("text/", StringComparison.Ordinal)
                && !contentType.StartsWith("application/xml", StringComparison.Ordinal) 
                && !contentType.StartsWith("application/json", StringComparison.Ordinal))
            {
                if (response.Content == null || response.Content.Length == 0)
                {
                    response.BinaryContent = new sbyte[0];
                    return;
                }

                string result = response.Content;
                byte[] bytes = Encoding.ASCII.GetBytes(result);
                string output = Convert.ToBase64String(bytes);
                byte[] finalOutput = Convert.FromBase64String(output);
                sbyte[] sbyteOutput = (sbyte[])(Array)finalOutput;


                response.BinaryContent = sbyteOutput;
            }
        }

        /// <summary>
        /// Build OAuth Headers and add additional headers
        /// </summary>
        /// <param name="extraHeaders">IDictionary<string, string></param>
        /// <param name="method">HttpMethod</param>
        /// <param name="relativeUrl">string</param>
        /// <param name="body">string</param>
        /// <param name="url">Uri</param>
        /// <param name="oauthHeaders">IDictionary<string, string></param>
        private void InitializeHeaders(IDictionary<string, string> extraHeaders, HttpMethod method, ref string relativeUrl, string body, ref Uri url, ref IDictionary<string, string> oauthHeaders)
        {
            // Append .xml extension when XML data format enabled
            if (PreferredFormat == DataFormat.XML)
                GenerateRelativeUrlForXMLDataFormat(ref relativeUrl);

            // Build OAuth Headers
            BuildOAuthHeaders(method, relativeUrl, body, out url, out oauthHeaders);

            // Add Extra Headers
            AddExtraHeaders(extraHeaders, oauthHeaders);
        }

        /// <summary>
        /// Read data from Response stream
        /// </summary>
        /// <param name="response"><see cref="Response"/>object</param>
        /// <param name="httpWebResponse"><see cref="HttpWebResponse"/>object</param>
        private void SetResponseContent(Response response, HttpWebResponse httpWebResponse)
        {
            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response.Content = streamReader.ReadToEnd().Trim();

                if (string.IsNullOrWhiteSpace(response.Content))
                    response.Content = null;
            }
        }

        /// <summary>
        /// Build response object from Http Response
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/>object</param>
        /// <param name="url">Uri</param>
        /// <param name="httpWebRequest"><see cref="HttpWebRequest"/>object</param>
        /// <param name="httpWebResponse"><see cref="HttpWebResponse"/>object</param>
        /// <returns><see cref="Response"/>object</returns>
        private Response ReadHttpResponse(HttpMethod method, Uri url, HttpWebRequest httpWebRequest, ref HttpWebResponse httpWebResponse)
        {
            Response response = new Response();
            try
            {
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                
                response.Method = method.ToString();
                response.Url = url.ToString();
                response.StatusCode = httpWebResponse.StatusCode;
                response.StatusMessage = httpWebResponse.StatusDescription;
                response.ContentType = httpWebResponse.ContentType;
                response.Headers = httpWebRequest.Headers; 
            }
            catch (WebException ex)
            {
                if (httpWebResponse == null)
                {
                    response.Method = method.ToString();
                    response.Url = url.ToString();
                    response.StatusCode = ((HttpWebResponse)ex.Response).StatusCode;
                    response.StatusMessage = ((HttpWebResponse)ex.Response).StatusDescription;
                    response.ContentType = ((HttpWebResponse)ex.Response).ContentType;
                    response.Headers = httpWebRequest.Headers; 
                    
                }
            }               

            return response;
        }

        /// <summary>
        /// Create new Web Request and assign OAuth Headers
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/>object</param>
        /// <param name="url">Uri</param>
        /// <param name="oauthHeaders">IDictionary<string, string></param>
        /// <returns><see cref="HttpWebRequest"/>object</returns>
        private HttpWebRequest InitializeWebRequest(HttpMethod method, Uri url, IDictionary<string, string> oauthHeaders)
        {
            // Create Web Request
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            // Set WebRequest Method
            httpWebRequest.Method = method.ToString();

            // Set OAuth Headers
            foreach (string oauthHeaderKey in oauthHeaders.Keys)
                httpWebRequest.Headers[oauthHeaderKey] = oauthHeaders[oauthHeaderKey];

            return httpWebRequest;
        }

        /// <summary>
        /// Write data to request stream
        /// </summary>
        /// <param name="body">string</param>
        /// <param name="httpWebRequest"><see cref="HttpWebRequest"/>object</param>
        /// <param name="stream"><see cref="Stream"/>object</param>
        /// <returns><see cref="Stream"/>object</returns>
        private Stream WriteDataToStream(string body, HttpWebRequest httpWebRequest, Stream stream)
        {
            if (PreferredFormat == DataFormat.XML)
                httpWebRequest.ContentType = "application/xml";
            else
                httpWebRequest.ContentType = "application/json";

            // Set content length for Web request
            httpWebRequest.ContentLength = Encoding.UTF8.GetBytes(body).Length;
            httpWebRequest.KeepAlive = true;

            // Write data to the stream
            stream = httpWebRequest.GetRequestStream();
            stream.Write(Encoding.UTF8.GetBytes(body), 0, Encoding.UTF8.GetBytes(body).Length);
            stream.Close();

            return stream;
        }

        /// <summary>
        /// Initialize URL and build the OAuth headers
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/>object</param>
        /// <param name="relativeUrl">string</param>
        /// <param name="body">string</param>
        /// <param name="url">Uri</param>
        /// <param name="oauthHeaders">IDictionary<string, string></param>
        private void BuildOAuthHeaders(HttpMethod method, string relativeUrl, string body, out Uri url, out IDictionary<string, string> oauthHeaders)
        {
            string fullUrl = API_DOMAIN + relativeUrl;

            if (Logger.IsDebugEnabled)
                Logger.Debug(string.Format("REQUEST - Method: {0}, URL: {1}, Body: {2}", method, fullUrl, body));

            url = new Uri(fullUrl);

            oauthHeaders = GetOAuthHeaders(method, url, body);

            if (oauthHeaders == null)
                throw new Exception("Authentication method not selected. Refer useOAuth# methods");
        }

        /// <summary>
        /// Add extra headers to OAuthHeaders collection
        /// </summary>
        /// <param name="extraHeaders">IDictionary<string, string></param>
        /// <param name="oAuthHeaders">IDictionary<string, string></param>
        private void AddExtraHeaders(IDictionary<string, string> extraHeaders, IDictionary<string, string> oAuthHeaders)
        {
            if (extraHeaders != null)
            {
                foreach (string key in extraHeaders.Keys)
                {
                    if (!oAuthHeaders.ContainsKey(key))
                        oAuthHeaders[key] = extraHeaders[key];
                    else
                        throw new Exception("Extra headers can not include OAuth headers");
                }
            }
        }

        /// <summary>
        /// Generate RelativeUrl for XML DataFormat
        /// </summary>
        /// <param name="relativeUrl">string</param>
        private void GenerateRelativeUrlForXMLDataFormat(ref string relativeUrl)
        {
            string queryString = string.Empty;

            Logger.Debug("Using XML extension on route");

            int queryStringIndex = relativeUrl.IndexOf('?');
            if (queryStringIndex != -1)
            {
                queryString = relativeUrl.Substring(queryStringIndex);
                relativeUrl = relativeUrl.Substring(0, queryStringIndex);
            }

            if (!relativeUrl.ToLower().EndsWith(".xml", StringComparison.Ordinal))
                relativeUrl += ".xml";

            if (queryStringIndex != -1)
                relativeUrl += queryString;
        }

        /// <summary>
        /// Returns the headers required for the selected authentication of the given request
        /// </summary>
        /// <param name="method">HTTP method of the request</param>
        /// <param name="url">URL of the request</param>
        /// <param name="body">Body of the request</param>
        /// <returns></returns>
        /// <exception cref="IOException"> </exception>
        private IDictionary<string, string> GetOAuthHeaders(HttpMethod method, Uri url, string body)
        {
            IDictionary<string, string> oauthHeaders = null;

            // OAuth1 Signature
            if (authMethod == AuthMethod.OAUTH1_SIGNATURE)
                return GenerateHeadersForOAuth1Signature(method, url, body, ref oauthHeaders);

            // Check to see if a Previous oauth2 request has been made
            VerifyForPreviousOAuth2Request();

            // Use the existing headers
            if (currentOAuthRequest != null)
            {
                if (Logger.IsDebugEnabled) Logger.Debug("Reusing previous OAuth2 headers");
                oauthHeaders = currentOAuthRequest.Headers; // use the current headers
                return oauthHeaders;
            }
            
            if (Logger.IsDebugEnabled) Logger.Debug("Generating new OAuth2 headers");
            OAuth2Request oauthRequest = null;

            // OAuth2 Assertion
            GenerateHeadersForOAuth2Assertion(ref oauthHeaders, ref oauthRequest);

            // OAuth2 Password
            GenerateHeadersForOAuth2Password(ref oauthHeaders, ref oauthRequest);

            // Save the new oauth request 
            currentOAuthRequest = oauthRequest;            

            return oauthHeaders;
        }

        /// <summary>
        /// Generate Headers for OAuth2 Password
        /// </summary>
        /// <param name="oauthHeaders">IDictionary<string, string></param>
        /// <param name="oauthRequest"><see cref="OAuth2Request"/>object</param>
        private void GenerateHeadersForOAuth2Password(ref IDictionary<string, string> oauthHeaders, ref OAuth2Request oauthRequest)
        {
            if (authMethod == AuthMethod.OAUTH2_PASSWORD)
            {
                OAuth2PasswordService oAuth2PasswordService = oAuthServiceFactory.Build<OAuth2PasswordService>(typeof(OAuth2PasswordService));
                oauthRequest = oAuth2PasswordService.GenerateOAuth2PasswordRequest(username, password);
                oauthHeaders = oauthRequest.Headers;
            }
        }

        /// <summary>
        /// Generate Headers for OAuth2 Assertion
        /// </summary>
        /// <param name="oauthHeaders">IDictionary<string, string></param>
        /// <param name="oauthRequest"><see cref="OAuth2Request"/>object</param>
        private void GenerateHeadersForOAuth2Assertion(ref IDictionary<string, string> oauthHeaders, ref OAuth2Request oauthRequest)
        {
            if (authMethod == AuthMethod.OAUTH2_ASSERTION)
            {
                OAuth2AssertionService oAuth2AssertionService = oAuthServiceFactory.Build<OAuth2AssertionService>(typeof(OAuth2AssertionService));
                oauthRequest = oAuth2AssertionService.GenerateOAuth2AssertionRequest(username);
                oauthHeaders = oauthRequest.Headers;
            }
        }

        /// <summary>
        /// Check to see if a Previous oauth2 request has been made
        /// </summary>
        private void VerifyForPreviousOAuth2Request()
        {
            if (currentOAuthRequest == null) return;

            long now = DateTimeHelper.CurrentUnixTimeMilliSeconds();
            long? expirationTime = ((OAuth2Request)currentOAuthRequest).ExpirationTime;

            // Check to see if current ones are expired
            if (now >= expirationTime)
            {
                if (Logger.IsDebugEnabled) Logger.Debug("Previous OAuth2 headers have expired");

                if (authMethod == AuthMethod.OAUTH2_PASSWORD)
                    RefreshOAuth2PasswordRequest();
                else
                    currentOAuthRequest = null; // Forget the previous oauth2 request
            }
        }

        /// <summary>
        /// Refresh OAuth2 Password Request
        /// </summary>
        private void RefreshOAuth2PasswordRequest()
        {
            if (Logger.IsDebugEnabled) Logger.Debug("Refreshing oauth2 token");

            OAuth2Request oAuth2Request = (OAuth2Request)currentOAuthRequest;
            currentOAuthRequest = null; // Forget the previous oauth2 request

            // Attempt to use the refresh token
            try
            {
                OAuth2PasswordService oAuth2PasswordService = oAuthServiceFactory.Build<OAuth2PasswordService>(typeof(OAuth2PasswordService));
                currentOAuthRequest = oAuth2PasswordService.RefreshOAuth2PasswordRequest(oAuth2Request);
            }
            catch (Exception ex)
            {
                Logger.Debug("Failed to refresh oauth2 token", ex);
                throw;
            }
        }

        /// <summary>
        /// Generate Headers for OAuth1 Signature
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/>object</param>
        /// <param name="url">Uri</param>
        /// <param name="body">string</param>
        /// <param name="oauthHeaders">IDictionary<string, string></param>
        /// <returns>IDictionary<string, string></returns>
        private IDictionary<string, string> GenerateHeadersForOAuth1Signature(HttpMethod method, Uri url, string body, ref IDictionary<string, string> oauthHeaders)
        {
            if (Logger.IsDebugEnabled) Logger.Debug("Generating OAuth1 headers");

            OAuth1SignatureService oAuth1SignatureService = oAuthServiceFactory.Build<OAuth1SignatureService>(typeof(OAuth1SignatureService));
            OAuth1Request oAuth1Request = oAuth1SignatureService.GenerateOAuth1Request(method, url, body);
            oauthHeaders = oAuth1Request.Headers;

            return oauthHeaders;
        }

        /// <summary>
        /// Parse the Time value from output
        /// </summary>
        /// <param name="xml">string</param>
        /// <returns>string</returns>
        private string ParseTimeValue(string xml)
        {
            XElement xElement = XElement.Parse(xml);
            return xElement.Element("value").Value;
        }
        
        #endregion
    }
}

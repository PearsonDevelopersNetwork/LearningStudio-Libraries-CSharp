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

using System.Collections.Generic;
using Com.Pearson.Pdn.Learningstudio.OAuth;
using Com.Pearson.Pdn.Learningstudio.Utility;

namespace Com.Pearson.Pdn.Learningstudio.Core
{
    /// <summary>
    /// Class implements AbstractService
    /// </summary>
    public class BasicService : AbstractService
    {
        #region Public methods

        public BasicService(OAuthServiceFactory oauthServiceFactory) : base(oauthServiceFactory)
        {
        }

        /// <summary>
        /// Provides name of service for identification purposes
        /// </summary>
        /// <returns>Unique identifier for service </returns>
        public override string ServiceIdentifier
        {
            get
            {
                return "LS-Library-Core-CSharp-V1";
            }
        }

        /// <summary>
        /// Performs HTTP operations using the selected authentication method
        /// </summary>
        /// <param name="method">The HTTP Method to user </param>
        /// <param name="relativeUrl">The URL after .com (/me)</param>
        /// <param name="body">The body of the message</param>
        /// <returns>Output in the preferred data format </returns>
        public override Response DoMethod(HttpMethod method, string relativeUrl, string body)
        {
            return base.DoMethod(method, relativeUrl, body);
        }

        /// <summary>
        /// Performs HTTP operations using the selected authentication method
        /// </summary>
        /// <param name="extraHeaders">Extra headers to include in the request</param>
        /// <param name="method">The HTTP Method to user</param>
        /// <param name="relativeUrl">The URL after .com (/me)</param>
        /// <param name="body">The body of the message</param>
        /// <returns>Output in the preferred data format</returns>
        public override Response DoMethod(IDictionary<string, string> extraHeaders, HttpMethod method, string relativeUrl, string body)
        {
            return base.DoMethod(extraHeaders, method, relativeUrl, body);
        }

        #endregion
    }
}

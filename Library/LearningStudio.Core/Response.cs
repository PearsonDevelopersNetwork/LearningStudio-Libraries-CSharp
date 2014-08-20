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
using System.Net;
using System.Text;

namespace Com.Pearson.Pdn.Learningstudio.Core
{
    /// <summary>
    /// Class holds properties related to Response request
    /// </summary>
    public class Response
    {
        private sbyte[] binaryContent;

        public Response()
        {
        }

        public string Method
        {
            get; set;
        }

        public string Url
        {
            get; set;
        }

        public string Content
        {
            get; set;
        }

        public string ContentType
        {
            get; set;
        }

        public HttpStatusCode StatusCode
        {
            get; set;
        }

        public string StatusMessage
        {
            get; set;
        }

        public sbyte[] BinaryContent
        {
            get; set;
        }

        public WebHeaderCollection Headers
        {
            get; set;
        }

        /// <summary>
        /// Indicates if an error occurred during last operation
        /// False indicates the operation completed successfully. True indicates the operation was aborted.
        /// </summary>
        public bool Error
        {
            get
            {
                return (int)StatusCode < 200 || (int)StatusCode >= 300; // only if outside 200 range
            }
        }

        /// <summary>
        /// Implements the toString method for use in debugging
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Method: ").Append(Method).Append(", ");
            sb.Append("URL: ").Append(Url).Append(", ");
            sb.Append("Code: ").Append(StatusCode).Append(", ");
            sb.Append("Message: ").Append(StatusMessage).Append(", ");
            sb.Append("Content-Type: ").Append(ContentType).Append(", ");
            sb.Append("Content: ").Append(Content);

            return sb.ToString();
        }
    }
}

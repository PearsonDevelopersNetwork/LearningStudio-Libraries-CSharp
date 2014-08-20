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

namespace Com.Pearson.Pdn.Learningstudio.Utility
{
    public static class DateTimeHelper
    {
        private static readonly DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        
        /// <summary>
        /// Generates total milliseconds since the unix epoch using the UTC date/time of the request
        /// </summary>
        /// <returns></returns>
        public static long CurrentUnixTimeMilliSeconds()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        /// <summary>
        /// Generates an integer representing the number of seconds since the unix epoch using the UTC date/time of the request
        /// </summary>
        /// <returns>Timestamp for the request</returns>
        public static string GetTimestamp()
        {
            return Convert.ToString((DateTimeHelper.CurrentUnixTimeMilliSeconds() / 1000));
        }
    }
}

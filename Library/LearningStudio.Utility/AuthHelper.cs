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
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;

namespace Com.Pearson.Pdn.Learningstudio.Utility
{
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public enum EncodingType
    {
        BASE64,
        HEX
    }

    public static class AuthHelper
    {
        /// <summary>
        /// Generates a random nonce
        /// </summary>
        /// <returns>Random string</returns>
        public static string GetNonce()
        {
            string randomFileName = Path.GetRandomFileName() + Path.GetRandomFileName() + Path.GetRandomFileName();
            randomFileName = randomFileName.Replace(".", "");

            if (randomFileName.Length > 32)
                return randomFileName.Substring(0, 32);

            return randomFileName;
        }

        /// <summary>
        /// Generates a BASE64-encoded or HEX-encoded CMAC-AES digest.
        /// </summary>
        /// <param name="key">string</param>
        /// <param name="message">string</param>
        /// <returns>string</returns>
        public static string GenerateCmac(string key, string message, EncodingType encodingType)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] msgBytes = Encoding.UTF8.GetBytes(message);

            CMac cMac = new CMac(new AesFastEngine());
            cMac.Init(new KeyParameter(keyBytes));
            cMac.Reset();

            cMac.BlockUpdate(msgBytes, 0, msgBytes.Length);
            byte[] output = new byte[cMac.GetMacSize()];
            cMac.DoFinal(output, 0);

            if (encodingType == EncodingType.BASE64)
                return Convert.ToBase64String(output);

            return Encoding.ASCII.GetString(Hex.Encode(output));
        }
    }
}

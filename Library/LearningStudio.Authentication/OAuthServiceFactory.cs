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
using Com.Pearson.Pdn.Learningstudio.OAuth.Config;
using Com.Pearson.Pdn.Learningstudio.OAuth.Request;

namespace Com.Pearson.Pdn.Learningstudio.OAuth
{
    public class OAuthServiceFactory
    {
        public OAuthConfig configuration;
        public OAuth1SignatureService oauth1SignatureService;
        public OAuth2AssertionService oauth2AssertionService;
        public OAuth2PasswordService oauth2PasswordService;

        #region Public methods
        public OAuthServiceFactory(OAuthConfig config)
        {
            this.configuration = config;
        }
        
        public T Build<T>(Type serviceClass) where T : OAuthService
        {
            if (serviceClass == typeof(OAuth1SignatureService))
                return GenerateOAuth1SignatureService<T>();
            
            if (serviceClass == typeof(OAuth2AssertionService))
                return GenerateOAuth2AssertionService<T>();
            
            if (serviceClass == typeof(OAuth2PasswordService))
                return GenerateOAuth2PasswordService<T>();

            throw new Exception("Not implemented: " + serviceClass);
        }

        #endregion

        #region Private methods

        private T GenerateOAuth2PasswordService<T>() where T : OAuthService
        {
            if (oauth2PasswordService == null)
            {
                OAuth2PasswordConfig config = new OAuth2PasswordConfig
                {
                    ApplicationId = configuration.ApplicationId,
                    ClientString = configuration.ClientString
                };

                oauth2PasswordService = new OAuth2PasswordService(config);
            }
            return (T)Convert.ChangeType(oauth2PasswordService, typeof(T));
        }

        private T GenerateOAuth2AssertionService<T>() where T : OAuthService
        {
            if (oauth2AssertionService == null)
            {
                OAuth2AssertionConfig config = new OAuth2AssertionConfig()
                {
                    ApplicationId = configuration.ApplicationId,
                    ApplicationName = configuration.ApplicationName,
                    ClientString = configuration.ClientString,
                    ConsumerKey = configuration.ConsumerKey,
                    ConsumerSecret = configuration.ConsumerSecret
                };

                oauth2AssertionService = new OAuth2AssertionService(config);
            }
            return (T)Convert.ChangeType(oauth2AssertionService, typeof(T));
        }

        private T GenerateOAuth1SignatureService<T>() where T : OAuthService
        {
            if (oauth1SignatureService == null)
            {
                OAuth1SignatureConfig config = new OAuth1SignatureConfig
                {
                    ApplicationId = configuration.ApplicationId,
                    ConsumerKey = configuration.ConsumerKey,
                    ConsumerSecret = configuration.ConsumerSecret
                };

                oauth1SignatureService = new OAuth1SignatureService(config);
            }
            return (T)Convert.ChangeType(oauth1SignatureService, typeof(T));
        }

        #endregion
    }
}

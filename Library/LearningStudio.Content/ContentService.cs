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
using Com.Pearson.Pdn.Learningstudio.Core;
using Com.Pearson.Pdn.Learningstudio.OAuth;
using Com.Pearson.Pdn.Learningstudio.Utility;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using log4net;

namespace Com.Pearson.Pdn.Learningstudio.Content
{
    public class ContentService : AbstractService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ContentService));
        
        // PATH CONSTANTS
        private const string PATH_COURSES_ITEMS = "/courses/{0}/items";
        private const string PATH_COURSES_ITEMS_ = "/courses/{0}/items/{1}";
        private const string PATH_USERS_COURSES_ITEMS = "/users/{0}/courses/{1}/items";
        private const string PATH_COURSES_ITEMHIERARCHY = "/courses/{0}/itemHierarchy";
        private const string PATH_COURSES_ITEMHIERARCHY__EXPAND = "/courses/{0}/itemHierarchy?expand={1}";
        private const string PATH_USERS_COURSES_ITEMHIERARCHY = "/users/{0}/courses/{1}/itemHierarchy";
        private const string PATH_USERS_COURSES_ITEMHIERARCHY__EXPAND = "/users/{0}/courses/{1}/itemHierarchy?expand={2}";
        private const string PATH_COURSES_TEXTMULTIMEDIAS = "/courses/{0}/textMultimedias";
        private const string PATH_COURSES_TEXTMULTIMEDIAS__CONTENTPATH_ = "/courses/{0}/textMultimedias/{1}/{2}";
        private const string PATH_COURSES_TEXTMULTIMEDIAS__CONTENTPATH__USESOURCEDOMAIN = "/courses/{0}/textMultimedias/{1}/{2}?useSourceDomain=true";
        private const string PATH_COURSES_TEXTMULTIMEDIAS_ = "/courses/{0}/textMultimedias/{1}";
        private const string PATH_COURSES_MSOFFICEDOCUMENTS = "/courses/{0}/msOfficeDocuments";
        private const string PATH_COURSES_MSOFFICEDOCUMENTS_ = "/courses/{0}/msOfficeDocuments/{1}";
        private const string PATH_COURSES_MSOFFICEDOCUMENTS_ORIGINALDOCUMENT = "/courses/{0}/msOfficeDocuments/{1}/originalDocument";
        private const string PATH_COURSES_MSOFFICEDOCUMENTS_CONTENT_ = "/courses/{0}/msOfficeDocuments/{1}/content/{2}";
        private const string PATH_COURSES_WEBCONTENTUPLOADS = "/courses/{0}/webContentUploads";
        private const string PATH_COURSES_WEBCONTENTUPLOADS_ = "/courses/{0}/webContentUploads/{1}";
        private const string PATH_COURSES_WEBCONTENTUPLOADS_ORIGINALDOCUMENT = "/courses/{0}/webContentUploads/{1}/originalDocument";
        private const string PATH_COURSES_WEBCONTENTUPLOADS_CONTENT_ = "/courses/{0}/webContentUploads/{1}/content/{2}";

        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSEHIEARCHY = "/courses/{0}/threadeddiscussions/{1}/topics/{2}/responseHierarchy";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSEHIEARCHY = "/courses/{0}/threadeddiscussions/{1}/topics/{2}/responses/{3}/responseHierarchy";
        private const string PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_USERVIEWRESPONSES_USERVIEWRESPONSES = "/users/{0}/courses/{1}/threadeddiscussions/{2}/topics/{3}/userviewresponses/{4}/userviewresponses";
        private const string PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_USERVIEWRESPONSES_USERVIEWRESPONSES__DEPTH = PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_USERVIEWRESPONSES_USERVIEWRESPONSES + "?depth={0}";
        private const string PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_USERVIEWRESPONSES = "/users/{0}/courses/{1}/threadeddiscussions/{2}/topics/{3}/userviewresponses";
        private const string PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_USERVIEWRESPONSES__DEPTH = PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_USERVIEWRESPONSES + "?depth={0}";

        private const string PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSECOUNTS = "/users/{0}/courses/{1}/threadeddiscussions/{2}/topics/{3}/responses/{4}/responseCounts";
        private const string PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSECOUNTS__DEPTH = PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSECOUNTS + "?depth={0}";
        private const string PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSECOUNTS = "/users/{0}/courses/{1}/threadeddiscussions/{2}/topics/{3}/responseCounts";
        private const string PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSECOUNTS__DEPTH = PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSECOUNTS + "?depth={0}";

        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSEBRANCH = "/courses/{0}/threadeddiscussions/{1}/topics/{2}/responses/{3}/responseBranch";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSEAUTHOR = "/courses/{0}/threadeddiscussions/{1}/topics/{2}/responses/{3}/responseAuthor";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSEANDAUTHORCOMPS = "/courses/{0}/threadeddiscussions/{1}/topics/{2}/responses/{3}/responseAndAuthorComps";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSEANDAUTHORCOMPS__DEPTH = "/courses/{0}/threadeddiscussions/{1}/topics/{2}/responses/{3}/responseAndAuthorComps?depth={4}";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSEANDAUTHORCOMPS = "/courses/{0}/threadeddiscussions/{1}/topics/{2}/responseAndAuthorComps";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSEANDAUTHORCOMPS__DEPTH = "/courses/{0}/threadeddiscussions/{1}/topics/{2}/responseAndAuthorComps?depth={3}";

        private const string PATH_USERS_COURSES_THREADEDDISCUSSIONS__LASTRESPONSE = "/users/{0}/courses/{1}/threadeddiscussions/lastResponse";
        private const string PATH_COURSES_THREADEDDISCUSSIONS = "/courses/{0}/threadeddiscussions";
        private const string PATH_COURSES_THREADEDDISCUSSIONS__USESOURCEDOMAIN = "/courses/{0}/threadeddiscussions?UseSourceDomain=true";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS = "/courses/{0}/threadeddiscussions/{1}/topics";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS__USESOURCEDOMAIN = "/courses/{0}/threadeddiscussions/{1}/topics?UseSourceDomain=true";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_ = "/courses/{0}/threadeddiscussions/{1}/topics/{2}";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_USESOURCEDOMAIN = "/courses/{0}/threadeddiscussions/{1}/topics/{2}?UseSourceDomain=true";

        private const string PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSE_READSTATUS = "/users/{0}/courses/{1}/threadeddiscussions/{2}/topics/{3}/responses/{4}/readStatus";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSES = "/courses/{0}/threadeddiscussions/{1}/topics/{2}/responses/{3}/responses";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_ = "/courses/{0}/threadeddiscussions/{1}/topics/{2}/responses/{3}";
        private const string PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES = "/courses/{0}/threadeddiscussions/{1}/topics/{2}/responses";


        /// <summary>
        /// Constructs a new ContentService
        /// </summary>
        /// <param name="oauthServiceFactory"></param>
        public ContentService(OAuthServiceFactory oauthServiceFactory)
            : base(oauthServiceFactory)
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
                return "LS-Library-Content-CSharp-V1";
            }
        }

        /// <summary>
        /// Get items for a course with
        /// Get /courses/{courseId}/items
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <returns>Response object with details of status and content </returns>
        public Response GetItems(string courseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_ITEMS, courseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetItems: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get a specific item for a course with
        /// Get /courses/{courseId}/items/{itemId}
        /// using OAuth1 or OAuth2
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetItem(string courseId, string itemId)
        {
            Response response = null;
            
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_ITEMS_, courseId, itemId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetItem: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get content for a specific item in a course with
        /// GetItem(courseId, itemId)
        /// by following the links to the item itself
        /// using OAuth1 or OAuth2 as a student, teacher, or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="itemId">   ID of the item</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetItemContent(string courseId, string itemId)
        {
            Response response = null;
            JObject json = null;
            JArray items = null;
            JArray links = null;
            string relativeUrl = string.Empty;
            string contentUrl = string.Empty;

            try
            {
                response = GetItem(courseId, itemId);
                if (response.Error) return response;

                // should only be one item here, but it is returned in an array for some reason
                json = (JObject)JsonConvert.DeserializeObject(response.Content);
                items = (JArray)json["items"];

                // again, only one element expected here...
                if (items == null || items.Count == 0)
                    throw new Exception("Unexpected condition in library: No items");

                foreach (var item in items)
                    links = (JArray)item["links"];

                foreach (var link in links)
                {
                    JValue title = (JValue)link["title"];

                    // rel on link varies, so identify self by missing title
                    if (title == null)
                    {
                        Uri uri = new Uri(link["href"].ToString());
                        relativeUrl = uri.AbsolutePath;
                        response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
                        if (response.Error) return response;

                        json = (JObject)JsonConvert.DeserializeObject(response.Content);
                        json = (JObject)json["textMultimedias"][0];
                        contentUrl = json["contentUrl"].ToString();
                        uri = new Uri(contentUrl.ToString());
                        relativeUrl = uri.AbsolutePath;

                        return DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetItem: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get items for a course with
        /// Get /users/{userId}/courses/{courseId}/items
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <returns>Response object with details of status and content </returns>
        public Response GetUserItems(string userId, string courseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_ITEMS, userId, courseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetUserItems: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get links details from a specific item for a course with
        /// Get /courses/{courseId}/items/{itemId}
        /// using OAuth2 as a student, teacher or teaching assistant
        /// 
        /// Example JSON structure: (Multimedia item)
        /// 
        /// { 
        ///   "details": { 
        ///     "access": {...}, 
        ///     "schedule": {...}, 
        ///     "self": {...},
        ///     "selfType": "textMultimedias"
        ///   }
        /// }
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="itemId">	ID of the item </param>
        /// <returns>Response object with details of status and content </returns>
        public Response GetItemLinkDetails(string courseId, string itemId)
        {
            Response response = null;
            JObject json = null;
            JArray items = null;
            JObject detail = null;
            JArray links = null;
            string relativeUrl = string.Empty;
            
            try
            {
                response = GetItem(courseId, itemId);
                if (response.Error) return response;

                // should only be one item here, but it is returned in an array for some reason

                json = (JObject)JsonConvert.DeserializeObject(response.Content);
                items = (JArray)json["items"];
                detail = new JObject();
                if (items == null || items.Count == 0)
                    throw new Exception("Unexpected condition in library: No items"); // this should never happen, but it should be detected if it does

                foreach (var item in items)
                    links = (JArray)item["links"];

                foreach (var link in links)
                {
                    relativeUrl = link["href"].ToString();
                    Uri uri = new Uri(relativeUrl);
                    response = DoMethod(HttpMethod.GET, uri.AbsolutePath, NO_CONTENT);
                    if (response.Error) return response;

                    JObject linkElement = (JObject)JsonConvert.DeserializeObject(response.Content);
                    JValue title = (JValue)link["title"];
                    if (title != null)
                    {
                        linkElement = (JObject)linkElement[title.ToString()];
                        detail.Add(title.ToString(), linkElement);
                        continue;
                    }

                    if (linkElement != null || linkElement.Count > 0)
                    {
                        foreach (var self in linkElement)
                        {
                            detail.Add("self", linkElement);
                            detail.Add("selfType", self.Key);
                        }
                    }
                }
                JObject detailWrapper = new JObject();
                detailWrapper.Add("details", detail);
                response.Content = detailWrapper.ToString();
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetItemLinkDetails: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get item hierarchy for a course with
        /// GET /courses/{courseId}/itemHierarchy
        /// using OAuth1 or OAuth2 as a teacher or teaching assistant, or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <returns>Response object with details of status and content</returns>
        /// <exception cref="IOException"> </exception>
        public Response GetItemHierarchy(string courseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_ITEMHIERARCHY, courseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetItemHierarchy", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get item hierarchy for a course with
        /// GET /courses/{courseId}/itemHierarchy?expand=item,item.access,item.schedule,item.group
        /// using OAuth1 or OAuth2 as a teacher or teaching assistant, or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="expandItems">	Comma separated list of items to expand from: 
        /// item,item.access,item.schedule,item.group </param>
        /// <returns>Response object with details of status and content</returns>
        /// <exception cref="IOException"> </exception>
        public Response GetItemHierarchy(string courseId, string expandItems)
        {
            if (expandItems == null || expandItems.Length == 0)
                return GetItemHierarchy(courseId);

            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_ITEMHIERARCHY__EXPAND, courseId, expandItems);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetItemHierarchy", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get item hierarchy for a course with
        /// GET /users/{userId}/courses/{courseId}/itemHierarchy
        /// using OAuth1 or OAuth2 as a teacher or teaching assistant, or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <returns>Response object with details of status and content</returns>
        /// <exception cref="IOException"> </exception>
        public Response GetUserItemHierarchy(string userId, string courseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_ITEMHIERARCHY, userId, courseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetUserItemHierarchy", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get item hierarchy for a course with
        /// GET /users/{userId}/courses/{courseId}/itemHierarchy?expand=item,item.access,item.schedule,item.group
        /// using OAuth1 or OAuth2 as a teacher or teaching assistant, or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="expandItems">	Comma separated list of items to expand from: 
        /// item,item.access,item.schedule,item.group </param>
        /// <returns>Response object with details of status and content</returns>
        /// <exception cref="IOException"> </exception>
        public Response GetUserItemHierarchy(string userId, string courseId, string expandItems)
        {
            if (expandItems == null || expandItems.Length == 0)
                return GetUserItemHierarchy(userId, courseId);

            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_ITEMHIERARCHY__EXPAND, userId, courseId, expandItems);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetUserItemHierarchy", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get specific text multimedia content by course with
        /// GET /courses/{courseId}/textMultimedias
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="textMediaId">ID of the text media </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetTextMultimedia(string courseId, string textMediaId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_TEXTMULTIMEDIAS_, courseId, textMediaId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetTextMultimedia: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get text multimedias by course with
        /// GET /courses/{courseId}/textMultimedias
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetTextMultimedias(string courseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_TEXTMULTIMEDIAS, courseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetTextMultimedias: ", ex);
                throw;
            }            
            return response;
        }

        /// <summary>
        /// Get specific text multimedia content by course with UseSourceDomain parameter with
        /// GET /courses/{courseId}/textMultimedias
        /// GET /courses/{courseId}/textMultimedias?UseSourceDomain=true
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="textMediaId">	ID of the text media </param>
        /// <param name="contentPath">	Path of content </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetTextMultimediasContent(string courseId, string textMediaId, string contentPath)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_TEXTMULTIMEDIAS__CONTENTPATH_, courseId, textMediaId, contentPath);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetTextMultimediasContent: ", ex);
                throw;
            } 
            
            return response;
        }

        /// <summary>
        /// Get specific text multimedia content by course with UseSourceDomain parameter with
        /// GET /courses/{courseId}/textMultimedias
        /// GET /courses/{courseId}/textMultimedias?UseSourceDomain=true
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="textMediaId">	ID of the text media </param>
        /// <param name="contentPath">	Path of content </param>
        /// <param name="useSourceDomain">	Indicator of whether to include domain in urls </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetTextMultimediasContent(string courseId, string textMediaId, string contentPath, bool useSourceDomain)
        {
            Response response = null;
            
            try
            {
                string path = useSourceDomain ? PATH_COURSES_TEXTMULTIMEDIAS__CONTENTPATH_ : PATH_COURSES_TEXTMULTIMEDIAS__CONTENTPATH__USESOURCEDOMAIN;
                string relativeUrl = string.Format(path, courseId, textMediaId, contentPath);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetTextMultimediasContent: ", ex);
                throw;
            } 
            
            return response;
        }

        /// <summary>
        /// Get specific text multimedia content by course with UseSourceDomain parameter with
        /// GET /courses/{courseId}/textMultimedias
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="textMediaId">	ID of the text media </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetTextMultimediasContent(string courseId, string textMediaId)
        {
            Response response = null;

            try
            {
                response = GetTextMultimediasContent(courseId, textMediaId, false);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetTextMultimediasContent: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get specific text multimedia content by course with UseSourceDomain parameter with
        /// GetTextMultimedia(courseId, textMediaId)'s contentUrl
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="textMediaId">	ID of the text media </param>
        /// <param name="useSourceDomain"> Indicator of whether to include domain in urls</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetTextMultimediasContent(string courseId, string textMediaId, bool useSourceDomain)
        {
            Response response = null;
            JObject json = null;
            string contentUrl = null;
            string relativeUrl = string.Empty;

            try
            {
                response = GetTextMultimedia(courseId, textMediaId);
                if (response.Error) return response;

                json = (JObject)JsonConvert.DeserializeObject(response.Content);
                json = (JObject)json["textMultimedias"][0];
                contentUrl = json["contentUrl"].ToString();

                if (useSourceDomain)
                    contentUrl += "?useSourceDomain=true";

                Uri uri = new Uri(contentUrl);
                relativeUrl = uri.AbsolutePath;

                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetTextMultimediasContent: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get a specific MS Office document in a course with
        /// GET /courses/{courseId}/msOfficeDocuments/{msOfficeDocumentId}
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="msOfficeDocumentId">	ID of the ms office document </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetMsOfficeDocument(string courseId, string msOfficeDocumentId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_MSOFFICEDOCUMENTS_, courseId, msOfficeDocumentId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetMsOfficeDocument: ", ex);
                throw;
            } 
            
            return response;
        }

        /// <summary>
        /// Get all MS Office documents in a course with
        /// GET /courses/{courseId}/msOfficeDocuments
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetMsOfficeDocuments(string courseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_MSOFFICEDOCUMENTS, courseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetMsOfficeDocuments: ", ex);
                throw;
            } 
            
            return response;
        }

        /// <summary>
        /// Get the original of a specific MS Office document in a course with
        /// GET /courses/{courseId}/msOfficeDocuments/{msOfficeDocumentId}/originalDocument
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="msOfficeDocumentId">	ID of the ms office document </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetMsOfficeDocumentOriginal(string courseId, string msOfficeDocumentId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_MSOFFICEDOCUMENTS_ORIGINALDOCUMENT, courseId, msOfficeDocumentId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetMsOfficeDocumentOriginal: ", ex);
                throw;
            } 
            
            return response;
        }

        /// <summary>
        /// Get content for a specific MS Office Document in a course with
        /// GET /courses/{courseId}/msOfficeDocuments/{msOfficeDocumentId}
        /// GET /courses/{courseId}/msOfficeDocuments/{msOfficeDocumentId}/content/{contentPath}
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="msOfficeDocumentId">	ID of the ms office document </param>
        /// <returns>Response object with details of status and content </returns>
        public Response GetMsOfficeDocumentContent(string courseId, string msOfficeDocumentId)
        {
            string relativeUrl = string.Empty;
            Response response = null;
            JObject json = null;
            JValue contentUrl = null;

            try
            {                
                response = GetMsOfficeDocument(courseId, msOfficeDocumentId);
                if (response.Error) return response;

                json = (JObject)JsonConvert.DeserializeObject(response.Content);
                json = (JObject)json["msOfficeDocuments"][0];
                contentUrl = (JValue)json["contentUrl"];
                Uri uri = new Uri(contentUrl.ToString());
                relativeUrl = uri.AbsolutePath;
                
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetMsOfficeDocumentContent: ", ex);
                throw;
            }            

            return response;
        }

        /// <summary>
        /// Get content for a specific MS Office Document in a course with
        /// GET /courses/{courseId}/msOfficeDocuments/{msOfficeDocumentId}
        /// GET /courses/{courseId}/msOfficeDocuments/{msOfficeDocumentId}/content/{contentPath}
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="msOfficeDocumentId">	ID of the ms office document </param>
        /// <param name="contentPath">	Path of the content </param>
        /// <returns>Response object with details of status and content </returns>
        public Response GetMsOfficeDocumentContent(string courseId, string msOfficeDocumentId, string contentPath)
        {
            Response response = null;
            
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_MSOFFICEDOCUMENTS_CONTENT_, courseId, msOfficeDocumentId, contentPath);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetMsOfficeDocumentContent: ", ex);
                throw;
            } 
            
            return response;
        }

        /// <summary>
        /// Get all web content uploads in a course with
        /// GET /courses/{courseId}/webContentUploads
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <returns>Response object with details of status and content </returns>
        public Response GetWebContentUploads(string courseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_WEBCONTENTUPLOADS, courseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetWebContentUploads: ", ex);
                throw;
            } 
            
            return response;
        }

        /// <summary>
        /// Get a specific MS Office document in a course with
        /// GET /courses/{courseId}/webContentUploads/{webContentUploadId}
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="webContentUploadId">	ID of the ms office document </param>
        /// <returns>Response object with details of status and content </returns>
        public Response GetWebContentUpload(string courseId, string webContentUploadId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_WEBCONTENTUPLOADS_, courseId, webContentUploadId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetWebContentUpload: ", ex);
                throw;
            } 
            
            return response;
        }

        /// <summary>
        /// Get a specific MS Office document in a course with
        /// GET /courses/{courseId}/webContentUploads/{webContentUploadId}
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="webContentUploadId">	ID of the ms office document </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetWebContentUploadOriginal(string courseId, string webContentUploadId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_WEBCONTENTUPLOADS_ORIGINALDOCUMENT, courseId, webContentUploadId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetWebContentUploadOriginal: ", ex);
                throw;
            } 
            
            return response;
        }

        /// <summary>
        /// Get content for a specific Web Content Upload in a course with
        /// GET /courses/{courseId}/webContentUpload/{webContentUploadId}
        /// GET /courses/{courseId}/webContentUpload/{webContentUploadId}/content/{contentPath}
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="webContentUploadId">	ID of the web content upload </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetWebContentUploadContent(string courseId, string webContentUploadId)
        {
            string relativeUrl = string.Empty;
            Response response = null;
            JObject json = null;
            JValue contentUrl = null;

            try
            {
                response = GetWebContentUpload(courseId, webContentUploadId);
                if (response.Error) return response;

                json = (JObject)JsonConvert.DeserializeObject(response.Content);
                json = (JObject)json["webContentUploads"][0];
                contentUrl = (JValue)json["contentUrl"];
                Uri uri = new Uri(contentUrl.ToString());
                relativeUrl = uri.AbsolutePath;

                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetWebContentUploadContent: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get content for a specific Web Content Upload in a course with
        /// GET /courses/{courseId}/webContentUpload/{webContentUploadId}
        /// GET /courses/{courseId}/webContentUpload/{webContentUploadId}/content/{contentPath}
        /// using OAuth2 as a student, teacher or teaching assistant
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="webContentUploadId">	ID of the web content upload </param>
        /// <param name="contentPath">	Path of the content </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetWebContentUploadContent(string courseId, string webContentUploadId, string contentPath)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_WEBCONTENTUPLOADS_CONTENT_, courseId, webContentUploadId, contentPath);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetWebContentUploadContent: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get hierarchy of a discussion thread response with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}/responseHierarchy
        /// using OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionResponseHierarchy(string courseId, string threadId, string topicId, string responseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSEHIEARCHY, courseId, threadId, topicId, responseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionResponseHierarchy: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get all user's view statuses of a discussion thread response with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/userviewresponses/{responseId}/userviewresponses
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response	</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionUserViewResponses(string userId, string courseId, string threadId, string topicId, string responseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_USERVIEWRESPONSES_USERVIEWRESPONSES, userId, courseId, threadId, topicId, responseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionUserViewResponses: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get all user's view statuses of a discussion thread response with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/userviewresponses/{responseId}/userviewresponses?depth={depth}
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response </param>
        /// <param name="depth">	Number of levels to traverse </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionUserViewResponses(string userId, string courseId, string threadId, string topicId, string responseId, int depth)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_USERVIEWRESPONSES_USERVIEWRESPONSES__DEPTH, userId, courseId, threadId, topicId, responseId, depth);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionUserViewResponses: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get all user's view statuses of a discussion thread topic with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/userviewresponses
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <returns>Response object with details of status and content </returns>
        public Response GetThreadedDiscussionTopicUserViewResponses(string userId, string courseId, string threadId, string topicId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_USERVIEWRESPONSES, userId, courseId, threadId, topicId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionTopicUserViewResponses: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get all user's view statuses of a discussion thread topic with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/userviewresponses?depth={depth}
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="depth">	Number of levels to traverse </param>
        /// <returns>Response object with details of status and content </returns>
        public Response GetThreadedDiscussionTopicUserViewResponses(string userId, string courseId, string threadId, string topicId, int depth)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_USERVIEWRESPONSES__DEPTH, userId, courseId, threadId, topicId, depth);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionTopicUserViewResponses: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get hierarchy of a discussion thread topic with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responseHierarchy
        /// using OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <returns>Response object with details of status and content </returns>
        public Response GetThreadedDiscussionTopicHierarchy(string courseId, string threadId, string topicId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSEHIEARCHY, courseId, threadId, topicId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionTopicHierarchy: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get count of responses for a specific response with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}/responseCounts
        /// using OAuth1 or OAuth2 as a student
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionResponseCount(string userId, string courseId, string threadId, string topicId, string responseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSECOUNTS, userId, courseId, threadId, topicId, responseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionResponseCount: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get count of responses for a specific response with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}/responseCounts?depth={depth}
        /// using OAuth1 or OAuth2 as a student
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response </param>
        /// <param name="depth">	Number of levels to traverse </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionResponseCount(string userId, string courseId, string threadId, string topicId, string responseId, int depth)
        {
            Response response = null;
            
            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSECOUNTS__DEPTH, userId, courseId, threadId, topicId, responseId, depth);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionResponseCount: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get count of responses for a specific topic with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responseCounts
        /// using OAuth1 or OAuth2 as a student
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <returns>Response object with details of status and content </returns>
        public Response GetThreadedDiscussionTopicResponseCount(string userId, string courseId, string threadId, string topicId)
        {
            Response response = null;
 
            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSECOUNTS, userId, courseId, threadId, topicId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionTopicResponseCount: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get count of responses for a specific topic with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responseCounts?depth={depth}
        /// using OAuth1 or OAuth2 as a student
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="depth">	Number of levels to traverse </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionTopicResponseCount(string userId, string courseId, string threadId, string topicId, int depth)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSECOUNTS__DEPTH, userId, courseId, threadId, topicId, depth);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionTopicResponseCount: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get branch hierarchy to a discussion thread response with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}/responseBranch
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response	</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionResponseBranch(string courseId, string threadId, string topicId, string responseId)
        {
            Response response = null;
            
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSEBRANCH, courseId, threadId, topicId, responseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionResponseBranch: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get author of a discussion thread response with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}/responseAuthor
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionResponseAuthor(string courseId, string threadId, string topicId, string responseId)
        {
            Response response = null;
            
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSEAUTHOR, courseId, threadId, topicId, responseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionResponseAuthor: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get response and author composite of a discussion thread response with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}/responseAndAuthorComps
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response	</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionResponseAndAuthorComposite(string courseId, string threadId, string topicId, string responseId)
        {
            Response response = null;
            
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSEANDAUTHORCOMPS, courseId, threadId, topicId, responseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionResponseAndAuthorComposite: ", ex);
                throw;
            }
            
            return response;
        }


        /// <summary>
        /// Get response and author composite for a discussion thread response at a specified depth with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}/responseAndAuthorComps?depth={depth}
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response </param>
        /// <param name="depth">		Max depth to traverse</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionResponseAndAuthorComposite(string courseId, string threadId, string topicId, string responseId, int depth)
        {
            Response response = null;
            
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSEANDAUTHORCOMPS__DEPTH, courseId, threadId, topicId, responseId, depth);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionResponseAndAuthorComposite: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get response and author composite for a discussion thread topic with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responseAndAuthorComps/{responseId}/responseAndAuthorComps
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionTopicResponseAndAuthorComposite(string courseId, string threadId, string topicId)
        {
            Response response = null;
           
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSEANDAUTHORCOMPS, courseId, threadId, topicId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionTopicResponseAndAuthorComposite: ", ex);
                throw;
            }
            
            return response;
        }


        /// <summary>
        /// Get response and author composite of a discussion thread topic at a specified depth with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responseAndAuthorComps/{responseId}/responseAndAuthorComps?depth={depth}
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="depth">		Max depth to traverse</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionTopicResponseAndAuthorComposite(string courseId, string threadId, string topicId, int depth)
        {
            Response response = null;
            
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSEANDAUTHORCOMPS__DEPTH, courseId, threadId, topicId, depth);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionTopicResponseAndAuthorComposite: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get a user's last threaded discussion response in a course with
        /// GET /users/{userId}/courses/{courseId}/threadeddiscussions/lastResponse
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetLastThreadedDiscussionResponse(string userId, string courseId)
        {
            Response response = null;
           
            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_THREADEDDISCUSSIONS__LASTRESPONSE, userId, courseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetLastThreadedDiscussionResponse: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get threaded dicussions for a course with
        /// GET /courses/{courseId}/threadeddiscussions
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussions(string courseId)
        {
            Response response = null;
            
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS, courseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussions: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get threaded dicussions for a course with
        /// GET /courses/{courseId}/threadeddiscussions?UseSourceDomain={useSourceDomain}
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="useSourceDomain">	Indicator of whether to use the source domain in links</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussions(string courseId, bool useSourceDomain)
        {
            Response response = null;
            try
            {
                if (!useSourceDomain) return GetThreadedDiscussions(courseId);

                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS__USESOURCEDOMAIN, courseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussions: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get threaded dicussion topics for a course with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionTopics(string courseId, string threadId)
        {
            Response response = null;
            
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS, courseId, threadId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionTopics: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get threaded dicussion topics for a course with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics?UseSourceDomain={useSourceDomain}
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="useSourceDomain">	Indicator of whether to use the source domain in links</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionTopics(string courseId, string threadId, bool useSourceDomain)
        {
            Response response = null;
            try
            {
                if (!useSourceDomain) return GetThreadedDiscussionTopics(courseId, threadId);

                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS__USESOURCEDOMAIN, courseId, threadId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionTopics: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get threaded dicussion topics for a course with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionTopic(string courseId, string threadId, string topicId)
        {
            Response response = null;
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_, courseId, threadId, topicId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionTopic: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get threaded dicussion topics for a course with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}?UseSourceDomain={useSourceDomain}
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="useSourceDomain">	Indicator of whether to use the source domain in links</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionTopic(string courseId, string threadId, string topicId, bool useSourceDomain)
        {
            Response response = null;
            try
            {
                if (!useSourceDomain) return GetThreadedDiscussionTopics(courseId, threadId);

                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_USESOURCEDOMAIN, courseId, threadId, topicId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionTopic: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get read status of a user's discussion thread response with
        /// GET /users/{userId}/courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}/readStatus
        /// using OAuth1 or OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionResponseReadStatus(string userId, string courseId, string threadId, string topicId, string responseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSE_READSTATUS, userId, courseId, threadId, topicId, responseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionResponseReadStatus: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get read status of a user's discussion thread response with
        /// PUT /users/{userId}/courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}/readStatus
        /// using OAuth1 or OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response </param>
        /// <param name="readStatus">	Read status Message</param>
        /// <returns>Response object with details of status and content</returns>
        public Response UpdateThreadedDiscussionResponseReadStatus(string userId, string courseId, string threadId, string topicId, string responseId, string readStatus)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSE_READSTATUS, userId, courseId, threadId, topicId, responseId);
                response = DoMethod(HttpMethod.PUT, relativeUrl, readStatus);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from UpdateThreadedDiscussionResponseReadStatus: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get responses to a specific discussion thread response with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}/responses
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response</param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionResponses(string courseId, string threadId, string topicId, string responseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSES, courseId, threadId, topicId, responseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionResponses: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Create a response to a specific discussion thread response with
        /// POST /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}/responses
        /// using OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseId">	ID of the response </param>
        /// <param name="responseMessage">	Response message to create</param>
        /// <returns>Response object with details of status and content</returns>
        public Response CreateThreadedDiscussionResponse(string courseId, string threadId, string topicId, string responseId, string responseMessage)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_RESPONSES, courseId, threadId, topicId, responseId);
                response = DoMethod(HttpMethod.POST, relativeUrl, responseMessage);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from CreateThreadedDiscussionResponse: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Create a response to a specific discussion thread topic with
        /// POST /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses
        /// using OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseMessage">	Response message to create</param>
        /// <returns>Response object with details of status and content</returns>
        public Response CreateThreadedDiscussionResponse(string courseId, string threadId, string topicId, string responseMessage)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES, courseId, threadId, topicId);
                response = DoMethod(HttpMethod.POST, relativeUrl, responseMessage);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from CreateThreadedDiscussionResponse: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Get a specific discussion thread response with
        /// GET /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}
        /// using OAuth2 as a student, teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseID">	ID of the response </param>
        /// <returns>Response object with details of status and content</returns>
        public Response GetThreadedDiscussionResponse(string courseId, string threadId, string topicId, string responseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_, courseId, threadId, topicId, responseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetThreadedDiscussionResponse: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Delete a specific discussion thread response with
        /// DELETE /courses/{courseId}/threadeddiscussions/{threadId}/topics/{topicId}/responses/{responseId}
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or admin
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="threadId">	ID of the thread </param>
        /// <param name="topicId">	ID of the topic </param>
        /// <param name="responseID">	ID of the response </param>
        /// <returns>Response object with details of status and content</returns>
        public Response DeleteThreadedDiscussionResponse(string courseId, string threadId, string topicId, string responseId)
        {
            Response response = null;

            try
            {
                string relativeUrl = string.Format(PATH_COURSES_THREADEDDISCUSSIONS_TOPICS_RESPONSES_, courseId, threadId, topicId, responseId);
                response = DoMethod(HttpMethod.DELETE, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from DeleteThreadedDiscussionResponse: ", ex);
                throw;
            }
            
            return response;
        }
    }
}

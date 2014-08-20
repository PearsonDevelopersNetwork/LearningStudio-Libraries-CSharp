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
using Newtonsoft.Json.Linq;
using Com.Pearson.Pdn.Learningstudio.Utility;
using Newtonsoft.Json;
using log4net;

namespace Com.Pearson.Pdn.Learningstudio.Grades
{
    public class GradeService : AbstractService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GradeService));

        private const string PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES = "/courses/{0}/gradebook/customCategories";
        private const string PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_ = "/courses/{0}/gradebook/customCategories/{1}";
        private const string PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_CUSTOMITEMS = "/courses/{0}/gradebook/customCategories/{1}/customItems";
        private const string PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_CUSTOMITEMS_ = "/courses/{0}/gradebook/customCategories/{1}/customItems/{2}";
        private const string PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_CUSTOMITEMS_GRADEBOOKITEM = "/courses/{0}/gradebook/customCategories/{1}/customItems/{2}/gradebookItem";
        private const string PATH_COURSES_GRADEBOOKITEMS = "/courses/{0}/gradebookItems";
        private const string PATH_COURSES_GRADEBOOKITEMS_ = "/courses/{0}/gradebookItems/{1}";
        private const string PATH_COURSES_GRADEBOOKITEMS_GRADES = "/courses/{0}/gradebookItems/{1}/grades";
        private const string PATH_COURSES_GRADEBOOK__GRADEBOOKITEMS_ = "/courses/{0}/gradebook/gradebookItems/{1}";
        private const string PATH_USERS_COURSES_GRADEBOOK__GRADEBOOKITEMS_GRADE = "/users/{0}/courses/{1}/gradebook/gradebookItems/{2}/grade";
        private const string PATH_USERS_COURSES_GRADEBOOK__GRADEBOOKITEMS_GRADE__USESOURCEDOMAIN = "/users/{0}/courses/{1}/gradebook/gradebookItems/{2}/grade?UseSourceDomain=true";
        private const string PATH_COURSES_GRADEBOOKITEMS_GRADES_ = "/courses/{0}/gradebookItems/{1}/grades/{2}";
        private const string PATH_USERS_COURSES_GRADEBOOKITEMS_GRADE = "/users/{0}/courses/{1}/gradebookItems/{2}/grade";
        private const string PATH_USERS_COURSES_USERGRADEBOOKITEMS = "/users/{0}/courses/{1}/userGradebookItems";
        private static string PATH_USERS_COURSES_USERGRADEBOOKITEMS__USESOURCEDOMAIN_EXPANDGRADE = PATH_USERS_COURSES_USERGRADEBOOKITEMS + "?UseSourceDomain=true&expand=grade";
        private static readonly string PATH_USERS_COURSES_USERGRADEBOOKITEMS__USESOURCEDOMAIN = PATH_USERS_COURSES_USERGRADEBOOKITEMS + "?UseSourceDomain=true";
        private static readonly string PATH_USERS_COURSES_USERGRADEBOOKITEMS__EXPANDGRADE = PATH_USERS_COURSES_USERGRADEBOOKITEMS + "?expand=grade";
        private const string PATH_USERS_COURSES_GRADEBOOK__USERGRADEBOOKITEMS = "/users/{0}/courses/{1}/gradebook/userGradebookItems";
        private const string PATH_USERS_COURSES_GRADEBOOK__USERGRADEBOOKITEMS_ = "/users/{0}/courses/{1}/gradebook/userGradebookItems/{2}";
        private static readonly string PATH_USERS_COURSES_GRADEBOOK__USERGRADEBOOKITEMS_EXPANDGRADE = PATH_USERS_COURSES_GRADEBOOK__USERGRADEBOOKITEMS_ + "?expand=grade";
        private const string PATH_USERS_COURSES_GRADEBOOK__USERGRADEBOOKITEMSTOTAL = "/users/{0}/courses/{1}/gradebook/userGradebookItemsTotals";
        private const string PATH_USERS_COURSES_COURSEGRADETODATE = "/users/{0}/courses/{1}/coursegradetodate";
        private const string PATH_COURSES_GRADEBOOK__ROSTERCOURSEGRADESTODATE__OFFSET_LIMIT_ = "/courses/{0}/gradebook/rostercoursegradestodate?offset={1}&limit={2}";
        private const string PATH_COURSES_GRADEBOOK__ROSTERCOURSEGRADESTODATE__STUDENTIDS_OFFSET_LIMIT_ = "/courses/{0}/gradebook/rostercoursegradestodate?Student.ID={1}&offset={2}&limit={3}";
        private const string PATH_COURSES_GRADEBOOK__ROSTERCOURSEGRADESTODATE__STUDENTIDS_ = "/courses/{0}/gradebook/rostercoursegradestodate?Student.ID={1}";

        #region Public methods

        /// <summary>
        /// Constructs a new GradeService
        /// </summary>
        /// <param name="oauthServiceFactory"> Service provider for OAuth operations </param>
        public GradeService(OAuthServiceFactory oauthServiceFactory)
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
                return "LS-Library-Grade-CSharp-V1";
            }
        }

        /// <summary>
        /// Create custom category and item with
        /// POST /courses/{courseId}/gradebook/customCategories
        /// POST /courses/{courseId}/gradebook/customCategories/{customCategoryId}/customItems
        /// using OAuth1 or OAuth2 as a teacher, teaching assistance or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="customCategory">	Custom category to create </param>
        /// <param name="customItem">	Custom item to create
        /// @return	Response object with details of status and content </param>
        public Response CreateCustomGradebookCategoryAndItem(string courseId, string customCategory, string customItem)
        {
            Response response = null;
            JObject customCategoryObject = null;
            JObject wrapper = null;
            JObject customItemObject = null;
            string customCategoryId = string.Empty;

            try
            {
                customCategoryObject = (JObject)JsonConvert.DeserializeObject(customCategory);

                response = CreateCustomGradebookCategory(courseId, customCategoryObject.ToString());
                if (response.Error) return response;

                customCategoryObject = JObject.Parse(response.Content);
                customCategoryObject = (JObject)customCategoryObject["customCategory"];
                JValue value = (JValue)customCategoryObject["guid"];
                customCategoryId = value.ToString();

                response = CreateCustomGradebookItem(courseId, customCategoryId, customItem);
                if (response.Error) return response;

                customItemObject = JObject.Parse(response.Content);
                customItemObject = (JObject)customItemObject["customItem"];

                wrapper = new JObject();
                wrapper.Add("customCategory", customCategoryObject);
                wrapper.Add("customItem", customItemObject);
                response.Content = wrapper.ToString();
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from CreateCustomGradebookCategoryAndItem: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Create custom gradebook category for a course with
        /// POST /courses/{courseId}/gradebook/customCategories
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="customCategory">	Custom category to create
        /// @return	Response object with details of status and content </param>
        public Response CreateCustomGradebookCategory(string courseId, string customCategory)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES, courseId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.POST, relativeUrl, customCategory);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from CreateCustomGradebookCategory: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Create custom gradebook item in a custom category for a course with
        /// POST /courses/{courseId}/gradebook/customCategories/{customCategoryId}/customItems
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="customCategoryId">	ID of the custom category </param>
        /// <param name="customItem">	Custom item to create
        /// @return	Response object with details of status and content </param>
        public Response CreateCustomGradebookItem(string courseId, string customCategoryId, string customItem)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_CUSTOMITEMS, courseId, customCategoryId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.POST, relativeUrl, customItem);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from CreateCustomGradebookItem: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Delete custom gradebook category for a course with
        /// DELETE /courses/{courseId}/gradebook/customCategories/{customCategoryId}
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="customCategoryId">	ID of the custom category
        /// @return	Response object with details of status and content </param>
        public Response DeleteCustomGradebookCategory(string courseId, string customCategoryId)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_, courseId, customCategoryId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.DELETE, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from DeleteCustomGradebookCategory: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Create custom gradebook category for a course with
        /// PUT /courses/{courseId}/gradebook/customCategories/{customCategoryId}
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="customCategoryId">	ID of the custom category </param>
        /// <param name="customCategory">	Custom category to create
        /// @return	Response object with details of status and content </param>
        public Response UpdateCustomGradebookCategory(string courseId, string customCategoryId, string customCategory)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_, courseId, customCategoryId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.PUT, relativeUrl, customCategory);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from UpdateCustomGradebookCategory: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get custom gradebook category for a course with
        /// GET /courses/{courseId}/gradebook/customCategories/{customCategoryId}
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="customCategoryId">	ID of the custom category
        /// @return	Response object with details of status and content </param>
        public Response GetCustomGradebookCategory(string courseId, string customCategoryId)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_, courseId, customCategoryId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetCustomGradebookCategory: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get custom item in a custom gradebook category for a course with
        /// GET /courses/{courseId}/gradebook/customCategories/{customCategoryId}/customItems/{customItemId}
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="customCategoryId">	ID of the custom category </param>
        /// <param name="customItemId">	ID of the custom item
        /// <returns>Response object with details of status and content </returns>
        public Response GetGradebookCustomItem(string courseId, string customCategoryId, string customItemId)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_CUSTOMITEMS_, courseId, customCategoryId, customItemId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetGradebookCustomItem: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get custom gradebook item in a custom category for a course with
        /// GET /courses/{courseId}/gradebook/customCategories/{customCategoryId}/customItems/{customItemId}
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="customCategoryId">	ID of the custom category </param>
        /// <param name="customItemId">	ID of the custom item
        /// <returns>Response object with details of status and content </returns>
        public Response GetCustomGradebookItem(string courseId, string customCategoryId, string customItemId)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_CUSTOMITEMS_GRADEBOOKITEM, courseId, customCategoryId, customItemId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetCustomGradebookItem: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Delete custom gradebook item in a custom category for a course with
        /// DELETE /courses/{courseId}/gradebook/customCategories/{customCategoryId}/customItems/{customItemId}
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="customCategoryId">	ID of the custom category </param>
        /// <param name="customItemId">	ID of the custom item
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>        
        public Response DeleteCustomGradebookItem(string courseId, string customCategoryId, string customItemId)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_CUSTOMITEMS_, courseId, customCategoryId, customItemId);
            Response response = null;

            try
            {   
                response = DoMethod(HttpMethod.DELETE, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from DeleteCustomGradebookItem: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get gradebook items for a course with
        /// GET /courses/{courseId}/gradebookItems
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course
        /// @return	Response object with details of status and content </param>      
        public Response GetGradebookItems(string courseId)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOKITEMS, courseId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetGradebookItems: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get specific gradebook item for a course with
        /// GET /courses/{courseId}/gradebookItems/{gradebookItemId}
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>        
        public Response GetGradebookItem(string courseId, string gradebookItemId)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOKITEMS_, courseId, gradebookItemId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetGradebookItem: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get grades for specific gradebook item in a course with
        /// GET /courses/{courseId}/gradebookItems/{gradebookItemId}/grades
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>        
        public Response GetGradebookItemGrades(string courseId, string gradebookItemId)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOKITEMS_GRADES, courseId, gradebookItemId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetGradebookItemGrades: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get grades for specific gradebook item in a course using parameters with
        /// GET /courses/{courseId}/gradebookItems/{gradebookItemId}/grades?gradedStudents={gradedStudentIds}&useSourceDomains=true&expand=user
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item </param>
        /// <param name="gradedStudentIds">	ID of students (semicolon separated) </param>
        /// <param name="useSourceDomain">	Indicator of whether to include domains in urls </param>
        /// <param name="expandUser">		Indicator of whether to expand user info 
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>        
        public Response GetGradebookItemGrades(string courseId, string gradebookItemId, string gradedStudentIds, bool useSourceDomain, bool expandUser)
        {
            Response response = null;
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_GRADEBOOKITEMS_GRADES, courseId, gradebookItemId);
                if (gradedStudentIds != null || useSourceDomain || expandUser)
                {
                    relativeUrl += "?";
                    bool firstParameter = true;
                    if (gradedStudentIds != null)
                    {
                        relativeUrl += "gradedStudents=" + gradedStudentIds;
                        firstParameter = false;
                    }
                    if (useSourceDomain)
                    {
                        if (!firstParameter)
                            relativeUrl += "&";
                        relativeUrl += "UseSourceDomain=true";
                        firstParameter = false;
                    }
                    if (expandUser)
                    {
                        if (!firstParameter)
                            relativeUrl += "&";
                        relativeUrl += "expand=user";
                        firstParameter = false;
                    }
                }

                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetGradebookItemGrades: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Update specific gradebook item for a course with
        /// PUT /courses/{courseId}/gradebook/gradebookItems/{gradebookItemId}
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item </param>
        /// <param name="gradebookItem">		Details of gradebook item
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>        
        public Response UpdateGradebookItem(string courseId, string gradebookItemId, string gradebookItem)
        {
            Response response = null;
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__GRADEBOOKITEMS_, courseId, gradebookItemId);
                response = DoMethod(HttpMethod.PUT, relativeUrl, gradebookItem);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from UpdateGradebookItem: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get a user's grade for an item in a course with
        /// GET /users/{userId}/courses/{courseId}/gradebook/gradebookItems/{gradebookItemId}/grade
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>        
        public Response GetGrade(string userId, string courseId, string gradebookItemId)
        {
            Response response = null;
            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_GRADEBOOK__GRADEBOOKITEMS_GRADE, userId, courseId, gradebookItemId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetGrade: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get a user's grade for an item in a course with override for useSourceDomain with
        /// GET /users/{userId}/courses/{courseId}/gradebook/gradebookItems/{gradebookItemId}/grade
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item </param>
        /// <param name="useSourceDomain">	Indicator of whether to include domain in urls
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetGrade(string userId, string courseId, string gradebookItemId, bool useSourceDomain)
        {
            Response response = null;
            try
            {
                string path = useSourceDomain ? PATH_USERS_COURSES_GRADEBOOK__GRADEBOOKITEMS_GRADE__USESOURCEDOMAIN : PATH_USERS_COURSES_GRADEBOOK__GRADEBOOKITEMS_GRADE;
                string relativeUrl = string.Format(path, userId, courseId, gradebookItemId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetGrade: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Create a user's grade for an item in a course with
        /// POST /users/{userId}/courses/{courseId}/gradebook/gradebookItems/{gradebookItemId}/grade
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item </param>
        /// <param name="grade">		Grade on the exam
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response CreateGrade(string userId, string courseId, string gradebookItemId, string grade)
        {
            Response response = null;
            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_GRADEBOOK__GRADEBOOKITEMS_GRADE, userId, courseId, gradebookItemId);
                response = DoMethod(HttpMethod.POST, relativeUrl, grade);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from CreateGrade: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Delete a user's grade for an item in a course with
        /// DELETE /users/{userId}/courses/{courseId}/gradebook/gradebookItems/{gradebookItemId}/grade
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response DeleteGrade(string userId, string courseId, string gradebookItemId)
        {
            Response response = null;
            try
            {
                string relativeUrl = string.Format(PATH_USERS_COURSES_GRADEBOOK__GRADEBOOKITEMS_GRADE, userId, courseId, gradebookItemId);
                response = DoMethod(HttpMethod.DELETE, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from DeleteGrade: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get specific grade for an item in a course with
        /// GET /courses/{courseId}/gradebookItems/{gradebookItemId}/grades/{gradeId}
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetGradebookItemGrade(string courseId, string gradebookItemId, string gradeId)
        {
            Response response = null;
            try
            {
                string relativeUrl = string.Format(PATH_COURSES_GRADEBOOKITEMS_GRADES_, courseId, gradebookItemId, gradeId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetGradebookItemGrade: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get specific grade for an item in a course using parameters with
        /// GET /courses/{courseId}/gradebookItems/{gradebookItemId}/grades/{gradeId}?gradedStudents={gradedStudentIds}&useSourceDomains=true&expand=user
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item </param>
        /// <param name="gradeId">	ID of the grade within the gradebook </param>
        /// <param name="gradedStudentIds">	ID of students (semicolon separated) </param>
        /// <param name="useSourceDomain">	Indicator of whether to include domains in urls </param>
        /// <param name="expandUser">		Indicator of whether to expand user info 
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetGradebookItemGrade(string courseId, string gradebookItemId, string gradeId, string gradedStudentIds, bool useSourceDomain, bool expandUser)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOKITEMS_GRADES_, courseId, gradebookItemId, gradeId);
            Response response = null;
            bool firstParameter = true;

            try
            {
                if (gradedStudentIds != null || useSourceDomain || expandUser)
                {
                    relativeUrl += "?";

                    BuildRelativeUrlAndFirstParameter(gradedStudentIds, useSourceDomain, expandUser, ref relativeUrl, ref firstParameter);
                }

                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetGradebookItemGrade: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Delete user's grade for an item in a course with
        /// DELETE /users/{userId}/courses/{courseId}/gradebookItems/{gradebookItemId}/grade
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response DeleteGradebookItemGrade(string userId, string courseId, string gradebookItemId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_GRADEBOOKITEMS_GRADE, userId, courseId, gradebookItemId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.DELETE, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from DeleteGradebookItemGrade: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Create user's grade for an item in a course with
        /// POST /users/{userId}/courses/{courseId}/gradebookItems/{gradebookItemId}/grade
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item </param>
        /// <param name="grade">	Grade content to be created
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response CreateGradebookItemGrade(string userId, string courseId, string gradebookItemId, string grade)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_GRADEBOOKITEMS_GRADE, userId, courseId, gradebookItemId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.POST, relativeUrl, grade);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from CreateGradebookItemGrade: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Update user's grade for an item in a course with
        /// PUT /users/{userId}/courses/{courseId}/gradebookItems/{gradebookItemId}/grade
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item </param>
        /// <param name="grade">	Grade content to be updated
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response UpdateGradebookItemGrade(string userId, string courseId, string gradebookItemId, string grade)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_GRADEBOOKITEMS_GRADE, userId, courseId, gradebookItemId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.PUT, relativeUrl, grade);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from UpdateGradebookItemGrade: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get gradebook items for a user in a course with
        /// GET /users/{userId}/courses/{courseId}/userGradebookItems
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetUserGradebookItems(string userId, string courseId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_USERGRADEBOOKITEMS, userId, courseId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetUserGradebookItems: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get gradebook items for a user in a course with
        /// GET /users/{userId}/courses/{courseId}/userGradebookItems
        /// with optional useSourceDomain and expand parameters
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="useSourceDomain">	Flag for using source domain parameter </param>
        /// <param name="expandGrade">	Flag for using expand grade parameter
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetUserGradebookItems(string userId, string courseId, bool useSourceDomain, bool expandGrade)
        {
            Response response = null;
            string path = string.Empty;
            string relativeUrl = string.Empty;

            try
            {
                path = PATH_USERS_COURSES_USERGRADEBOOKITEMS;

                if (useSourceDomain || expandGrade)
                {
                    if (useSourceDomain && expandGrade)
                        path = PATH_USERS_COURSES_USERGRADEBOOKITEMS__USESOURCEDOMAIN_EXPANDGRADE;
                    else if (useSourceDomain)
                        path = PATH_USERS_COURSES_USERGRADEBOOKITEMS__USESOURCEDOMAIN;
                    else
                        path = PATH_USERS_COURSES_USERGRADEBOOKITEMS__EXPANDGRADE;
                }

                relativeUrl = string.Format(path, userId, courseId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetUserGradebookItems: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get user gradebook items in a course gradebook with
        /// GET /users/{userId}/courses/{courseId}/gradebook/userGradebookItems
        /// using OAuth1 or Auth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetCourseGradebookUserItems(string userId, string courseId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_GRADEBOOK__USERGRADEBOOKITEMS, userId, courseId);
            Response response = null;

            try
            {                
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetCourseGradebookUserItems: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get user gradebook item in a course gradebook by user gradebook item id with
        /// GET /users/{userId}/courses/{courseId}/gradebook/userGradebookItems/{userGradebookItemId}
        /// using OAuth1 or Auth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="userGradebookItemId"> concatenation of {userId}-{gradebookItemGuid}
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetCourseGradebookUserItem(string userId, string courseId, string userGradebookItemId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_GRADEBOOK__USERGRADEBOOKITEMS_, userId, courseId, userGradebookItemId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetCourseGradebookUserItem: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get user gradebook item in a course gradebook by user gradebook item id with
        /// GET /users/{userId}/courses/{courseId}/gradebook/userGradebookItems/{userGradebookItem}
        /// or GET /users/{userId}/courses/{courseId}/gradebook/userGradebookItems/{userGradebookItem}?expandGrade=true
        /// using OAuth1 or Auth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="userGradebookItemId"> concatenation of {userId}-{gradebookItemGuid} </param>
        /// <param name="expandGrade">	Flag of whether to expand grade data
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetCourseGradebookUserItem(string userId, string courseId, string userGradebookItemId, bool expandGrade)
        {
            string path = string.Empty;
            string relativeUrl = string.Empty;
            Response response = null;

            try
            {
                path = expandGrade ? PATH_USERS_COURSES_GRADEBOOK__USERGRADEBOOKITEMS_EXPANDGRADE : PATH_USERS_COURSES_GRADEBOOK__USERGRADEBOOKITEMS_;
                relativeUrl = string.Format(path, userId, courseId, userGradebookItemId);
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetCourseGradebookUserItem: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Update a user's grade for an item in a course with
        /// PUT /users/{userId}/courses/{courseId}/gradebook/gradebookItems/{gradebookItemId}/grade
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="gradebookItemId">	ID of the gradebook item </param>
        /// <param name="grade">		Grade on the exam
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response UpdateGrade(string userId, string courseId, string gradebookItemId, string grade)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_GRADEBOOK__GRADEBOOKITEMS_GRADE, userId, courseId, gradebookItemId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.PUT, relativeUrl, grade);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from UpdateGrade: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get summary of points available to a student in a course with
        /// GET /users/{userId}/courses/{courseId}/gradebook/userGradebookItemsTotals
        /// using OAuth1 or Auth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetTotalPointsAvailable(string userId, string courseId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_GRADEBOOK__USERGRADEBOOKITEMSTOTAL, userId, courseId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetTotalPointsAvailable: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get custom categories in a course's gradebook with
        /// GET /courses/{courseId}/gradebook/customCategories
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetCustomGradebookCategories(string courseId)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES, courseId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetCustomGradebookCategories: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get custom items in a custom category of a course's gradebook with
        /// GET /courses/{courseId}/gradebook/customCategories/{customCategoryId}/customItems
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="customCategoryId">	ID of a custom category </param>
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetCustomGradebookItems(string courseId, string customCategoryId)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_CUSTOMITEMS, courseId, customCategoryId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetCustomGradebookItems: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get a user's grades for a course with
        /// GET /users/{userId}/courses/{courseId}/gradebook/userGradebookItems
        /// and getGrade(string userId, string courseId, string gradebookItemId, false)
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetGrades(string userId, string courseId)
        {
            return GetGrades(userId, courseId, false);
        }

        /// <summary>
        /// Get a user's grades for a course with
        /// GET /users/{userId}/courses/{courseId}/gradebook/userGradebookItems
        /// and GetGrade(string userId, string courseId, string gradebookItemId, bool useSourceDomain)
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant, or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="useSourceDomain">	Flag for using source domain parameter </param>
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetGrades(string userId, string courseId, bool? useSourceDomain)
        {
            Response response = null;

            try
            {
                response = GetCourseGradebookUserItems(userId, courseId);
                if (response.Error) return response;

                JObject json = JObject.Parse(response.Content);
                JArray userGradebookItems = (JArray)json["userGradebookItems"];
                JArray grades = new JArray();

                foreach (JObject itemIter in userGradebookItems)
                {
                    JObject gradebookItem = (JObject)itemIter;
                    gradebookItem = (JObject)gradebookItem["gradebookItem"];
                    string gradebookItemId = gradebookItem["id"].ToString();

                    Response gradeResponse = GetGrade(userId, courseId, gradebookItemId);
                    if (gradeResponse.Error)
                    {
                        // grades for all items might not exist. That's ok
                        if (gradeResponse.StatusCode == System.Net.HttpStatusCode.NotFound) continue;
                        
                        return gradeResponse;
                    }

                    JObject grade = JObject.Parse(gradeResponse.Content);
                    grades.Add(grade);
                }

                JObject gradesWrapper = new JObject();
                gradesWrapper.Add("grades", grades);
                response.Content = gradesWrapper.ToString();
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetGrades: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get a user's current grades for a course with
        /// GET /users/{userId}/courses/{courseId}/coursegradetodate
        /// using OAuth1 or Auth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetCurrentGrade(string userId, string courseId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_COURSEGRADETODATE, userId, courseId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetCurrentGrade: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get current grades for specific students in a course with
        /// GET /courses/{courseId}/gradebook/rostercoursegradestodate?Student.ID={studentIds}
        /// using OAuth1 or Auth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of course </param>
        /// <param name="studentIds"> Comma-separated list of students to filter 
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetCurrentGrades(string courseId, string studentIds)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__ROSTERCOURSEGRADESTODATE__STUDENTIDS_, courseId, studentIds);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetCurrentGrades: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get current grades for all students in a course with
        /// GET /courses/{courseId}/gradebook/rostercoursegradestodate?offset={offset}&limit={limit}
        /// using OAuth1 or Auth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of course </param>
        /// <param name="offset">	Offset position </param>
        /// <param name="limit">		Limitation on count of records
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetCurrentGrades(string courseId, int offset, int limit)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__ROSTERCOURSEGRADESTODATE__OFFSET_LIMIT_, courseId, Convert.ToString(offset), Convert.ToString(limit));
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetCurrentGrades: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Get current grades for specific students in a course with
        /// GET /courses/{courseId}/gradebook/rostercoursegradestodate?Student.ID={studentIds}&offset={offset}&limit={limit}
        /// using OAuth1 or Auth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of course </param>
        /// <param name="studentIds"> Comma-separated list of students to filter </param>
        /// <param name="offset">	Offset position </param>
        /// <param name="limit">		Limitation on count of records
        /// @return	Response object with details of status and content </param>
        /// <exception cref="IOException"> </exception>
        public Response GetCurrentGrades(string courseId, string studentIds, int offset, int limit)
        {
            string relativeUrl = string.Format(PATH_COURSES_GRADEBOOK__ROSTERCOURSEGRADESTODATE__STUDENTIDS_OFFSET_LIMIT_, courseId, studentIds, Convert.ToString(offset), Convert.ToString(limit));
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception from GetCurrentGrades: ", ex);
                throw;
            }

            return response;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Build RelativeUrl and First parameter
        /// </summary>
        /// <param name="gradedStudentIds">string</param>
        /// <param name="useSourceDomain">bool</param>
        /// <param name="expandUser">bool</param>
        /// <param name="relativeUrl">string</param>
        /// <param name="firstParameter">string</param>
        private void BuildRelativeUrlAndFirstParameter(string gradedStudentIds, bool useSourceDomain, bool expandUser, ref string relativeUrl, ref bool firstParameter)
        {
            if (gradedStudentIds != null)
            {
                relativeUrl += "gradedStudents=" + gradedStudentIds;
                firstParameter = false;
            }

            if (useSourceDomain)
            {
                if (!firstParameter)
                    relativeUrl += "&";
                relativeUrl += "UseSourceDomain=true";
                firstParameter = false;
            }

            if (expandUser)
            {
                if (!firstParameter)
                    relativeUrl += "&";
                relativeUrl += "expand=user";
                firstParameter = false;
            }
        }

        #endregion
    }
}

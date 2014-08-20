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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Com.Pearson.Pdn.Learningstudio.OAuth;
using Com.Pearson.Pdn.Learningstudio.Utility;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Net;
using Com.Pearson.Pdn.Learningstudio.Core;
using Newtonsoft.Json;
using log4net;

namespace Com.Pearson.Pdn.Learningstudio.Exam
{
    public class ExamService : AbstractService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExamService));

        // RELATION CONSTANTS
        private const string RELS_USER_COURSE_EXAM = "https://api.learningstudio.com/rels/user/course/exam";

        // PATH CONSTANTS
        private const string PATH_USERS_COURSES_ITEMS = "/users/{0}/courses/{1}/items";
        private const string PATH_USERS_COURSES_EXAMS = "/users/{0}/courses/{1}/exams";
        private const string PATH_USERS_COURSES_EXAMS_ = "/users/{0}/courses/{1}/exams/{2}";
        private const string PATH_USERS_COURSES_EXAMS_ATTEMPTS = "/users/{0}/courses/{1}/exams/{2}/attempts";
        private const string PATH_USERS_COURSES_EXAMS_ATTEMPTS_ = "/users/{0}/courses/{1}/exams/{2}/attempts/{3}";
        private const string PATH_USERS_COURSES_EXAMS_ATTEMPTS_SUMMARY = "/users/{0}/courses/{1}/exams/{2}/attempts/{3}/summary";
        private const string PATH_USERS_COURSES_EXAMDETAILS = "/users/{0}/courses/{1}/examDetails";
        private const string PATH_USERS_COURSES_EXAMDETAILS_ = "/users/{0}/courses/{1}/examDetails/{2}";
        private const string PATH_COURSES_EXAMSCHEDULES = "/courses/{0}/examSchedules";

        private const string PATH_USERS_COURSES_EXAMS_SECTIONS = "/users/{0}/courses/{1}/exams/{2}/sections";
        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_QUESTIONS = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/questions";

        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_TRUEFALSE_ = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/trueFalseQuestions/{4}";
        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_TRUEFALSE_CHOICES = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/trueFalseQuestions/{4}/choices";

        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_MULTIPLECHOICE_ = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/multipleChoiceQuestions/{4}";
        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_MULTIPLECHOICE_CHOICES = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/multipleChoiceQuestions/{4}/choices";

        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_MANYMULTIPLECHOICE_ = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/manyMultipleChoiceQuestions/{4}";
        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_MANYMULTIPLECHOICE_CHOICES = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/manyMultipleChoiceQuestions/{4}/choices";

        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_MATCHING_ = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/matchingQuestions/{4}";
        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_MATCHING_PREMISES = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/matchingQuestions/{4}/premises";
        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_MATCHING_CHOICES = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/matchingQuestions/{4}/choices";

        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_SHORT_ = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/shortQuestions/{4}";
        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_ESSAY_ = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/essayQuestions/{4}";
        private const string PATH_USERS_COURSES_EXAMS_SECTIONS_FILLINTHEBLANK_ = "/users/{0}/courses/{1}/exams/{2}/sections/{3}/fillintheblankQuestions/{4}";

        private const string PATH_USERS_COURSES_EXAMS_ATTEMPTS_ANSWERS = "/users/{0}/courses/{1}/exams/{2}/attempts/{3}/answers";
        private const string PATH_USERS_COURSES_EXAMS_ATTEMPTS_ANSWERS_ = "/users/{0}/courses/{1}/exams/{2}/attempts/{3}/answers/{4}";

        // EXAM Constants
        private const string PEARSON_EXAM_TOKEN = "Pearson-Exam-Token";
        private const string PEARSON_EXAM_PASSWORD = "Pearson-Exam-Password";
       

        private sealed class QuestionType
        {
            public static readonly QuestionType TRUE_FALSE = new QuestionType("TRUE_FALSE", InnerEnum.TRUE_FALSE, "trueFalse");
            public static readonly QuestionType MULTIPLE_CHOICE = new QuestionType("MULTIPLE_CHOICE", InnerEnum.MULTIPLE_CHOICE, "multipleChoice");
            public static readonly QuestionType MANY_MULTIPLE_CHOICE = new QuestionType("MANY_MULTIPLE_CHOICE", InnerEnum.MANY_MULTIPLE_CHOICE, "manyMultipleChoice");
            public static readonly QuestionType MATCHING = new QuestionType("MATCHING", InnerEnum.MATCHING, "matching");
            public static readonly QuestionType SHORT_ANSWER = new QuestionType("SHORT_ANSWER", InnerEnum.SHORT_ANSWER, "short");
            public static readonly QuestionType ESSAY = new QuestionType("ESSAY", InnerEnum.ESSAY, "essay");
            public static readonly QuestionType FILL_IN_THE_BLANK = new QuestionType("FILL_IN_THE_BLANK", InnerEnum.FILL_IN_THE_BLANK, "fillInTheBlank");

            private static readonly IList<QuestionType> valueList = new List<QuestionType>();

            static QuestionType()
            {
                valueList.Add(TRUE_FALSE);
                valueList.Add(MULTIPLE_CHOICE);
                valueList.Add(MANY_MULTIPLE_CHOICE);
                valueList.Add(MATCHING);
                valueList.Add(SHORT_ANSWER);
                valueList.Add(ESSAY);
                valueList.Add(FILL_IN_THE_BLANK);
            }

            public enum InnerEnum
            {
                TRUE_FALSE,
                MULTIPLE_CHOICE,
                MANY_MULTIPLE_CHOICE,
                MATCHING,
                SHORT_ANSWER,
                ESSAY,
                FILL_IN_THE_BLANK
            }

            private readonly string nameValue;
            private readonly int ordinalValue;
            private readonly InnerEnum innerEnumValue;
            private static int nextOrdinal = 0;

            internal string value;

            internal QuestionType(string name, InnerEnum innerEnum, string value)
            {                
                this.value = value;

                nameValue = name;
                ordinalValue = nextOrdinal++;
                innerEnumValue = innerEnum;
            }

            public string Value()
            {
                return this.value;
            }

            public static QuestionType MatchesValue(string value)
            {
                value = value.ToLower();

                foreach (QuestionType questionType in QuestionType.Values())
                    if (questionType.value.ToLower() == value)
                        return questionType;

                return null;
            }

            public static IList<QuestionType> Values()
            {
                return valueList;
            }

            public InnerEnum InnerEnumValue()
            {
                return innerEnumValue;
            }

            public int Ordinal()
            {
                return ordinalValue;
            }

            public override string ToString()
            {
                return nameValue;
            }

            public static QuestionType ValueOf(string name)
            {
                foreach (QuestionType enumInstance in QuestionType.Values())
                {
                    if (enumInstance.nameValue == name)
                        return enumInstance;
                }

                throw new System.ArgumentException(name);
            }
        }

        #region Public methods

        /// <summary>
        /// Constructs a new ExamService
        /// </summary>
        /// <param name="oAuthServiceFactory"> Service provider for OAuth operations </param>
        public ExamService(OAuthServiceFactory oAuthServiceFactory)
            : base(oAuthServiceFactory)
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
                return "LS-Library-Exam-CSharp-V1";
            }
        }

        /// <summary>
        /// Retrieve all of a user's exams for a course with
        /// GET /users/{userId}/courses/{courses}/items
        /// using OAuth1 or OAuth2 as a student or teacher
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of course </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response GetAllExamItems(string userId, string courseId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_ITEMS, userId, courseId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
                if (response.Error)
                    return response;

                JObject json = (JObject)JsonConvert.DeserializeObject(response.Content);
                JArray items = (JArray)json["items"];
                JArray exams = new JArray();

                foreach (var itemIter in items)
                {
                    JObject item = (JObject)itemIter;
                    JArray links = (JArray)item["links"];

                    foreach (var linkIter in links)
                    {
                        JObject link = (JObject)linkIter;
                        JValue value = (JValue)link["rel"];

                        if (value.Value.Equals(RELS_USER_COURSE_EXAM))
                            exams.Add(item);
                    }
                }

                JObject examItems = new JObject();
                examItems.Add("items", exams);
                response.Content = examItems.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetAllExamItems: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Retrieve all of a user's existing exams for a course with 
        /// GET /users/{userId}/courses/{courseId}/exams
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>
        public Response GetExistingExams(string userId, string courseId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS, userId, courseId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetExistingExams: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Retrieve details for all exams for a course with 
        /// GET /users/{userId}/courses/{courseId}/exams
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>
        public Response GetExamDetails(string userId, string courseId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMDETAILS, userId, courseId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetExamDetails: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Retrieve details for all exams for a course with 
        /// GET /users/{userId}/courses/{courseId}/exams
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>
        public Response GetExamDetails(string userId, string courseId, string examId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMDETAILS_, userId, courseId, examId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetExamDetails: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Retrieve exam schedules for a course with 
        /// GET /courses/{courseId}/examschedules
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="courseId">	ID of the course </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>
        public Response GetExamSchedules(string courseId)
        {
            string relativeUrl = string.Format(PATH_COURSES_EXAMSCHEDULES, courseId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetExamSchedules: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Retrieve a user's exam for a course with
        /// GET /users/{userId}/courses/{courseId}/exams/{examId}
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>
        public Response GetExistingExam(string userId, string courseId, string examId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_, userId, courseId, examId);
            Response response = null;

            try
            {              
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetExistingExam: ", ex);
                throw;
            }   

            return response;
        }

        /// <summary>
        /// Creates an exam for a user in a course with 
        /// POST /users/userId/courses/{courseId}/exams/{examId}
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>
        public Response CreateUserExam(string userId, string courseId, string examId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_, userId, courseId, examId);
            Response response = null;
            
            try
            {
                response = GetExistingExam(userId, courseId, examId);
                if (response.StatusCode != HttpStatusCode.NotFound) return response;

                response = DoMethod(HttpMethod.POST, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from CreateUserExam: ", ex);
                throw;
            }   

            return response;
        }

        /// <summary>
        /// Delete a users's exam in a course with 
        /// DELETE /users/{userId}/courses/{courseId}/exams/{examId}
        /// using OAuth1 or OAuth2 as a teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response DeleteUserExam(string userId, string courseId, string examId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_, userId, courseId, examId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.DELETE, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from DeleteUserExam: ", ex);
                throw;
            }   

            return response;
        }

        /// <summary>
        /// Create an exam attempt for a user in a course with 
        /// POST /users/{userId}/courses/{courseId}/exams/{examId}/attempts
        /// using OAuth1 or OAuth2 as student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response CreateExamAttempt(string userId, string courseId, string examId)
        {
            return CreateExamAttempt(userId, courseId, examId, null);
        }

        /// <summary>
        /// Create an exam attempt for a user in a course with 
        /// POST /users/{userId}/courses/{courseId}/exams/{examId}/attempts
        /// using OAuth1 or OAuth2
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <param name="examPassword">	Optional password from instructor </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response CreateExamAttempt(string userId, string courseId, string examId, string examPassword)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_ATTEMPTS, userId, courseId, examId);
            IDictionary<string, string> examHeaders = null;
            Response response = null;

            try
            {
                if (examPassword != null)
                {
                    examHeaders = new Dictionary<string, string>();
                    examHeaders.Add(PEARSON_EXAM_PASSWORD, examPassword);
                }
                
                response = DoMethod(examHeaders, HttpMethod.POST, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from CreateExamAttempt: ", ex);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Retrieve a users's attempt of an exam in a course with 
        /// GET /users/{userId}/courses/{courseId}/exams/{examId}/attempts
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response GetExamAttempts(string userId, string courseId, string examId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_ATTEMPTS, userId, courseId, examId);
            Response response = null;
            
            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetExamAttempts: ", ex);
                throw;
            }
            
            return response;
        }

        /// <summary>
        /// Retrieve a user's attempt of an exam in a course with 
        /// GET /users/{userId}/courses/{courseId}/exams/{examId}/attempts/{attemptId}
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <param name="attemptId">	ID of the exam attempt </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response GetExamAttempt(string userId, string courseId, string examId, string attemptId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_ATTEMPTS_, userId, courseId, examId, attemptId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetExamAttempt: ", ex);
                throw;
            }            

            return response;
        }

        /// <summary>
        ///  Retrieves and filters a user's current attempt of an exam in a course with 
        ///  GET /users/{userId}/courses/{courseId}/exams/{examId}/attempts
        ///  using OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response GetCurrentExamAttempt(string userId, string courseId, string examId)
        {
            string attemptsJson = string.Empty;
            Response response = null;
            JObject attempt = null;

            try
            {
                response = GetExamAttempts(userId, courseId, examId);
                if (response.Error) return response;

                attemptsJson = response.Content;

                JObject currentAttempt = GetCurrentAttempt(response);

                if (currentAttempt == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Content = null;
                    return response;
                }

                attempt = new JObject();
                attempt.Add("attempt", currentAttempt);
                response.Content = attempt.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetCurrentExamAttempt: ", ex);
                throw;
            }         

            return response;
        }



        /// <summary>
        /// Retrieve a summary of a user's attempt of an exam in a course with 
        /// GET /users/{userId}/courses/{courseId}/exams/{examId}/attempts/{attempdId}/summary
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="attemptId">	ID of the attempt </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response GetExamAttemptSummary(string userId, string courseId, string examId, string attemptId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_ATTEMPTS_SUMMARY, userId, courseId, examId, attemptId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetExamAttemptSummary: ", ex);
                throw;
            } 

            return response;
        }

        /// <summary>
        /// Retrieve a user's current attempt or create new attempt of an exam in a course with
        /// getCurrentExamAttempt and createExamAttempt
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response StartExamAttempt(string userId, string courseId, string examId)
        {
            return StartExamAttempt(userId, courseId, examId, null);
        }

        /// <summary>
        /// Retrieve a user's current attempt or create new attempt of an exam in a course with
        /// getCurrentExamAttempt and createExamAttempt
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <param name="examPassword">	Optional password from instructor </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response StartExamAttempt(string userId, string courseId, string examId, string examPassword)
        {
            Response response = null;

            try
            {
                response = GetCurrentExamAttempt(userId, courseId, examId);
                if (response.StatusCode != HttpStatusCode.NotFound) return response;

                response = CreateExamAttempt(userId, courseId, examId, examPassword);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from StartExamAttempt: ", ex);
                throw;
            } 

            return response;
        }

        /// <summary>
        /// Retrieve sections of an user's exam in a course with 
        /// GET /users/{userId}/courses/{courseId}/exams/{examId}/sections
        /// using OAuth1 or OAuth2 as a student, teacher, teaching assistant or administrator
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>
        public Response GetExamSections(string userId, string courseId, string examId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS, userId, courseId, examId);
            Response response = null;

            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetExamSections: ", ex);
                throw;
            }   

            return response;
        }

        /// <summary>
        /// Retrieve details of questions for a section of a user's exam in a course with 
        /// GET /users/{userId}/courses/{courseId}/exams/{examId}/sections/{sectionId}/questions
        /// and getExamSectionQuestion
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <param name="sectionId">	ID of the section on the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response GetExamSectionQuestions(string userId, string courseId, string examId, string sectionId)
        {

            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_QUESTIONS, userId, courseId, examId, sectionId);
            string sectionQuestionJson = string.Empty;
            Response response = null;
            string questionType = string.Empty;
            string questionId = string.Empty;
            
            try
            {
                response = DoMethod(HttpMethod.GET, relativeUrl, NO_CONTENT);
                if (response.Error) return response;

                sectionQuestionJson = response.Content;
                
                JObject jObject = JObject.Parse(response.Content);
                JArray questions = (JArray)jObject["questions"];
                JArray sectionQuestions = new JArray();

                foreach (JObject question in questions)
                {
                    questionType = question["type"].ToString();
                    questionId = question["id"].ToString();
                
                    Response questionResponse = GetExamSectionQuestion(userId, courseId, examId, sectionId, questionType, questionId);
                    if (questionResponse.Error) 
                        return questionResponse;
                
                    JObject sectionQuestion = JObject.Parse(questionResponse.Content);
                    sectionQuestion.Add("id", questionId);
                    sectionQuestion.Add("type", questionType);
                    sectionQuestion.Add("pointsPossible", question["pointsPossible"].ToString());
                    sectionQuestions.Add(sectionQuestion);
                }

                GetExamSectionQuestionsResponse(response, sectionQuestions);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetExamSectionQuestions: ", ex);
                throw;
            }   

            return response;
        }

        /// <summary>
        /// Retrieve details of a question for a section of a user's exam in a course with
        /// GET /users/{userId}/courses/{courseId}/exams/{examId}/sections/{sectionId}/{questionType}Questions/{questionId}
        /// and GET /users/{userId}/courses/{courseId}/exams/{examId}/sections/{sectionId}/{questionType}Questions/{questionId}/choices
        /// and GET /users/{userId}/courses/{courseId}/exams/{examId}/sections/{sectionId}/{questionType}Questions/{questionId}/premises
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="examId">	ID of the exam </param>
        /// <param name="sectionId">	ID of the section </param>
        /// <param name="questionType">	Type of question </param>
        /// <param name="questionId">	ID of the question </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response GetExamSectionQuestion(string userId, string courseId, string examId, string sectionId, string questionType, string questionId)
        {
            Response response = null;
            string questionRelativeUrl = string.Empty;
            string choicesRelativeUrl = string.Empty;
            string premisesRelativeUrl = string.Empty;
            IDictionary<string, string> extraHeaders = null;
            JObject details = null;
            JObject question = null;

            try
            {
                QuestionType type = QuestionType.MatchesValue(questionType);
                if (type == null)
                    throw new Exception("Invalid Question Type");

                response = GetCurrentExamAttempt(userId, courseId, examId);
                if (response.Error) return response;

                extraHeaders = GetExamHeaders(response);

                GetRelativeUrls(userId, courseId, examId, sectionId, questionId, ref questionRelativeUrl, ref choicesRelativeUrl, ref premisesRelativeUrl, type);

                details = new JObject();

                // Add Question to Details
                response = DoMethod(extraHeaders, HttpMethod.GET, questionRelativeUrl, NO_CONTENT);
                if (response.Error) return response;

                question = AddQuestionToDetailsObject(response, details, question, type);

                // Add Choices to Details
                if (choicesRelativeUrl != null)
                {
                    response = DoMethod(extraHeaders, HttpMethod.GET, choicesRelativeUrl, NO_CONTENT);
                    if (response.Error) return response;

                    AddChoicesToDetailsObject(response, details);
                }

                // Add Premises to Details
                if (premisesRelativeUrl != null)
                {
                    response = DoMethod(extraHeaders, HttpMethod.GET, premisesRelativeUrl, NO_CONTENT);
                    if (response.Error) return response;

                    AddPremisesToDetailsObject(response, details);
                }

                response.Content = details.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetExamSectionQuestion: ", ex);
                throw;
            }   

            return response;
        }

        /// <summary>
        /// Updates a user's answer for a question on a specific attempt of an exam in a course with
        /// GET /users/{userId}/courses/{courseId}/exams/{examId}/attempts/{attemptId}/answers/{answerId}
        /// POST /users/{userId}/courses/{courseId}/exams/{examId}/attempts/{attemptId}/answers
        /// PUT /users/{userId}/courses/{courseId}/exams/{examId}/attempts/{attemptId}/answers/{answerId}
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <param name="attemptId">	ID of the attempt on the exam </param>
        /// <param name="questionId">	ID of the question on the exam </param>
        /// <param name="answer">	Answer to the question on the exam </param>
        /// <returns> Response object with details of status and content </returns>     
        public Response AnswerQuestion(string userId, string courseId, string examId, string attemptId, string questionId, string answer)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_ATTEMPTS_ANSWERS_, userId, courseId, examId, attemptId, questionId);
            Response response = null;
            IDictionary<string, string> extraHeaders = null;

            try
            {
                response = GetExamAttempt(userId, courseId, examId, attemptId);
                if (response.Error) return response;

                extraHeaders = GetExamHeaders(response);

                response = DoMethod(extraHeaders, HttpMethod.GET, relativeUrl, NO_CONTENT);

                if (response.Error)
                    return DoMethod(extraHeaders, HttpMethod.PUT, relativeUrl, answer);

                if (response.StatusCode != HttpStatusCode.NotFound)
                    return response;

                relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_ATTEMPTS_ANSWERS, userId, courseId, examId, attemptId);
                response = DoMethod(extraHeaders, HttpMethod.POST, relativeUrl, answer);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from AnswerQuestion: ", ex);
                throw;
            }   

            return response;
        }

        /// <summary>
        /// Retrieve a user's answer for a question on a specific attempt of an exam in a course with
        /// GET /users/{userId}/courses/{courseId}/exams/{examId}/attempts/{attemptId}/answers/{answerId}
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <param name="attemptId">	ID of the attempt on the exam </param>
        /// <param name="questionId">	ID of the question on the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response GetQuestionAnswer(string userId, string courseId, string examId, string attemptId, string questionId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_ATTEMPTS_ANSWERS_, userId, courseId, examId, attemptId, questionId);
            IDictionary<string, string> extraHeaders = null;
            Response response = null;

            try
            {
                response = GetExamAttempt(userId, courseId, examId, attemptId);
                if (response.Error) return response;

                extraHeaders = GetExamHeaders(response);

                response = DoMethod(extraHeaders, HttpMethod.GET, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from GetQuestionAnswer: ", ex);
                throw;
            }  

            return response;
        }

        /// <summary>
        /// Delete a user's answer for a question on a specific attempt of an exam in a course with
        /// DELETE /users/{userId}/courses/{courseId}/exams/{examId}/attempts/{attemptId}/answers/{answerId}
        /// using OAuth2 as a student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <param name="attemptId">	ID of the attempt on the exam </param>
        /// <param name="questionId">	ID of the question on the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public Response DeleteQuestionAnswer(string userId, string courseId, string examId, string attemptId, string questionId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_ATTEMPTS_ANSWERS_, userId, courseId, examId, attemptId, questionId);
            Response response = null;
            IDictionary<string, string> extraHeaders = null;

            try
            {
                response = GetExamAttempt(userId, courseId, examId, attemptId);
                if (response.Error) return response;

                extraHeaders = GetExamHeaders(response);

                response = DoMethod(extraHeaders, HttpMethod.DELETE, relativeUrl, NO_CONTENT);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from DeleteQuestionAnswer: ", ex);
                throw;
            }  

            return response;
        }

        /// <summary>
        /// Updates a user's attempt of an exam in a course to complete with 
        /// PUT /users/{userId}/courses/{examId}/exams/{examId}/attempts/{attemptId}
        /// using OAuth2 as student
        /// </summary>
        /// <param name="userId">	ID of the user </param>
        /// <param name="courseId">	ID of the course </param>
        /// <param name="examId">	ID of the exam </param>
        /// <param name="attemptId">	ID of the attempt on the exam </param>
        /// <returns> Response object with details of status and content </returns>
        /// <exception cref="IOException"> </exception>        
        public virtual Response CompleteExamAttempt(string userId, string courseId, string examId, string attemptId)
        {
            string relativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_ATTEMPTS_, userId, courseId, examId, attemptId);
            IDictionary<string, string> extraHeaders = null;
            Response response = null;

            try
            {
                response = GetExamAttempt(userId, courseId, examId, attemptId);
                if (response.Error) return response;

                extraHeaders = GetExamHeaders(response);

                JObject jObject = JObject.FromObject(new { attempts = new { isCompleted = true } });

                response = DoMethod(extraHeaders, HttpMethod.PUT, relativeUrl, jObject.ToString());
            }
            catch (Exception ex)
            {
                Logger.Error("Exception from CompleteExamAttempt: ", ex);
                throw;
            }

            return response;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Get Exam headers
        /// </summary>
        /// <param name="response">Response object</param>
        /// <returns>Dictionary of string, string</returns>
        private IDictionary<string, string> GetExamHeaders(Response response)
        {
            JObject attempt = JObject.Parse(response.Content);
            attempt = (JObject)attempt["attempt"];

            IDictionary<string, string> extraHeaders = new Dictionary<string,string>();
            extraHeaders.Add(PEARSON_EXAM_TOKEN, attempt["pearsonExamToken"].ToString());

            return extraHeaders;
        }

        /// <summary>
        /// Get Current Attempt
        /// </summary>
        /// <param name="response">Response object</param>
        /// <returns>JObject</returns>
        private JObject GetCurrentAttempt(Response response)
        {
            JObject jObject = JObject.Parse(response.Content);
            JArray attempts = (JArray)jObject["attempts"];
            JObject currentAttempt = null;

            foreach (JObject attempt in attempts)
            {
                if (!Convert.ToBoolean(attempt["isCompleted"]))
                {
                    currentAttempt = attempt;
                    break;
                }
            }
            return currentAttempt;
        }

        /// <summary>
        /// Build the response object for ExamSectionQuestions method
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="sectionQuestions">JArray</param>
        private static void GetExamSectionQuestionsResponse(Response response, JArray sectionQuestions)
        {
            JObject sectionQuestionsWrapper = new JObject();
            sectionQuestionsWrapper.Add("questions", sectionQuestions);
            response.Content = sectionQuestionsWrapper.ToString();
        }

        /// <summary>
        /// Build Question, Choices or Premises relative URLs
        /// </summary>
        /// <param name="userId">string</param>
        /// <param name="courseId">string</param>
        /// <param name="examId">string</param>
        /// <param name="sectionId">string</param>
        /// <param name="questionId">string</param>
        /// <param name="questionRelativeUrl">string</param>
        /// <param name="choicesRelativeUrl">string</param>
        /// <param name="premisesRelativeUrl">string</param>
        /// <param name="type">QuestionType</param>
        private void GetRelativeUrls(string userId, string courseId, string examId, string sectionId, string questionId, ref string questionRelativeUrl, ref string choicesRelativeUrl, ref string premisesRelativeUrl, QuestionType type)
        {
            switch (type.InnerEnumValue())
            {
                case QuestionType.InnerEnum.TRUE_FALSE:
                    questionRelativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_TRUEFALSE_, userId, courseId, examId, sectionId, questionId);
                    choicesRelativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_TRUEFALSE_CHOICES, userId, courseId, examId, sectionId, questionId);
                    break;
                case QuestionType.InnerEnum.MULTIPLE_CHOICE:
                    questionRelativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_MULTIPLECHOICE_, userId, courseId, examId, sectionId, questionId);
                    choicesRelativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_MULTIPLECHOICE_CHOICES, userId, courseId, examId, sectionId, questionId);
                    break;
                case QuestionType.InnerEnum.MANY_MULTIPLE_CHOICE:
                    questionRelativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_MANYMULTIPLECHOICE_, userId, courseId, examId, sectionId, questionId);
                    choicesRelativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_MANYMULTIPLECHOICE_CHOICES, userId, courseId, examId, sectionId, questionId);
                    break;
                case QuestionType.InnerEnum.MATCHING:
                    questionRelativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_MATCHING_, userId, courseId, examId, sectionId, questionId);
                    premisesRelativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_MATCHING_PREMISES, userId, courseId, examId, sectionId, questionId);
                    choicesRelativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_MATCHING_CHOICES, userId, courseId, examId, sectionId, questionId);
                    break;
                case QuestionType.InnerEnum.SHORT_ANSWER:
                    questionRelativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_SHORT_, userId, courseId, examId, sectionId, questionId);
                    break;
                case QuestionType.InnerEnum.ESSAY:
                    questionRelativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_ESSAY_, userId, courseId, examId, sectionId, questionId);
                    break;
                case QuestionType.InnerEnum.FILL_IN_THE_BLANK:
                    questionRelativeUrl = string.Format(PATH_USERS_COURSES_EXAMS_SECTIONS_FILLINTHEBLANK_, userId, courseId, examId, sectionId, questionId);
                    break;
            }
        }

        /// <summary>
        /// Add Premises To Details Object
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="details">JObject</param>
        private void AddPremisesToDetailsObject(Response response, JObject details)
        {
            JObject choices = JObject.Parse(response.Content);
            details.Add("premises", choices.GetValue("premises"));
        }

        /// <summary>
        /// Add Choices To Details Object
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="details">JObject</param>
        private void AddChoicesToDetailsObject(Response response, JObject details)
        {
            JObject choices = JObject.Parse(response.Content);
            details.Add("choices", choices.GetValue("choices"));
        }

        /// <summary>
        /// Add Question To Details Object
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="details">JObject</param>
        /// <param name="question">JObject</param>
        /// <param name="type">QuestionType</param>
        /// <returns></returns>
        private JObject AddQuestionToDetailsObject(Response response, JObject details, JObject question, QuestionType type)
        {
            question = JObject.Parse(response.Content);
            details.Add("question", question.GetValue(type.Value() + "Question"));
            return question;
        }

        #endregion
    }
}

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

namespace Com.Pearson.Pdn.Learningstudio.Utility
{
    public class ConfigManager
    {
        private const string CONST_APPLICATION_ID = "ApplicationId";
        private const string CONST_APPLICATION_NAME = "ApplicationName";
        private const string CONST_CLIENT_STRING = "ClientString";
        private const string CONST_CONSUMER_KEY = "ConsumerKey";
        private const string CONST_CONSUMER_SECRET = "ConsumerSecret";
        private const string CONST_USERNAME = "Username";
        private const string CONST_PASSWORD = "Password";
        private const string CONST_GRADE_CUSTOMER_CATEGORIES_URL = "GradeCustomerCategories.URL";
        private const string CONST_GRADE_CUSTOMER_CATEGORIES_MULTIPLE = "GradeCustomerCategories.Multiple";
        private const string CONST_GRADE_CUSTOMER_CATEGORIES_SINGLE = "GradeCustomerCategories.Single";
        private const string CONST_GRADE_CUSTOMER_CATEGORIES_ID = "GradeCustomerCategories.Id";
        private const string PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES = "PathCoursesGradebookCustomCategories";
        private const string PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_ = "PathCoursesGradebookCustomCategories_";
        private const string PATH_COURSES_MSOFFICEDOCUMENTS_ORIGINALDOCUMENT = "PathCoursesMsOfficeDocumentsOriginalDocument";
        private const string TEACHER_USERNAME = "TeacherUsername";
        private const string TEACHER_PASSWORD = "TeacherPassword";
        private const string COURSE_ID = "CourseId";
        private const string ADMIN_USER_ID = "AdminUserId";
        private const string TA_USER_ID = "TaUserId";
        private const string TEACHER_USER_ID = "TeacherUserId";
        private const string STUDENT_USER_ID = "StudentUserId";
        private const string MS_OFFICE_DOCUMENT_ID = "MsofficedocumentId";
        private const string STUDENT_USERNAME = "StudentUsername";
        private const string TA_USERNAME = "TaUsername";
        private const string ADMIN_USERNAME = "AdminUsername";
        private const string EXAM_ID = "ExamId";
        private const string PROTECTED_EXAM_ID = "ProtectedExamId";
        private const string PROTECTED_EXAM_PASSWORD = "ProtectedExamPassword";
        private const string SECTION_ID = "SectionId";
        private const string MULTIPLECHOICE_QUESTION_TYPE = "MultiplechoiceQuestionType";
        private const string TRUEFALSE_QUESTION_TYPE = "TrueFalseQuestionType";

        private const string MATCHING_QUESTION_TYPE = "MatchingQuestionType";
        private const string MANYMULTIPLECHOICE_QUESTION_TYPE = "ManyMultipleChoiceQuestionType";
        private const string SHORTANSWER_QUESTION_TYPE = "ShortAnswerQuestionType";
        private const string ESSAY_QUESTION_TYPE = "EssayQuestionType";
        private const string FILLINTHEBLANK_QUESTION_TYPE = "FillInTheBlankQuestionType";
        private const string UNIT_ID = "UnitId";
        private const string GRADEBOOK_ITEM_ID = "GradebookItemId";
        private const string ITEM_ID = "ItemId";
        private const string MULTIMEDIA_ID = "MultimediaId";
        private const string MULTIMEDIA_CONTENT_PATH = "MultimediaContentPath";
        private const string MSOFFICEDOCUMENT_CONTENT_PATH = "MsofficedocumentContentPath";

        private const string WEBCONTENTUPLOAD_ID = "WebcontentuploadId";
        private const string WEBCONTENTUPLOAD_CONTENT_PATH = "WebcontentuploadContentPath";
        private const string THREAD_ID = "ThreadId";
        private const string TOPIC_ID = "TopicId";
        private const string RESPONSE_ID = "ResponseId";


        public static string ApplicationId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(CONST_APPLICATION_ID);
            }
        }

        public static string ApplicationName
        {
            get
            {
                return ConfigUtil.GetConfigurationString(CONST_APPLICATION_NAME);
            }
        }

        public static string ClientString
        {
            get
            {
                return ConfigUtil.GetConfigurationString(CONST_CLIENT_STRING);
            }
        }

        public static string ConsumerKey
        {
            get
            {
                return ConfigUtil.GetConfigurationString(CONST_CONSUMER_KEY);
            }
        }

        public static string ConsumerSecret
        {
            get
            {
                return ConfigUtil.GetConfigurationString(CONST_CONSUMER_SECRET);
            }
        }

        public static string Username
        {
            get
            {
                return ConfigUtil.GetConfigurationString(CONST_USERNAME);
            }
        }

        public static string Password
        {
            get
            {
                return ConfigUtil.GetConfigurationString(CONST_PASSWORD);
            }
        }

        public static Uri GradeCustomerCategoriesURL
        {
            get
            {
                return new Uri(ConfigUtil.GetConfigurationString(CONST_GRADE_CUSTOMER_CATEGORIES_URL));
            }
        }

        public static string GradeCustomerCategoriesMultiple
        {
            get
            {
                return ConfigUtil.GetConfigurationString(CONST_GRADE_CUSTOMER_CATEGORIES_MULTIPLE);
            }
        }

        public static string GradeCustomerCategoriesSingle
        {
            get
            {
                return ConfigUtil.GetConfigurationString(CONST_GRADE_CUSTOMER_CATEGORIES_SINGLE);
            }
        }

        public static string GradeCustomerCategoriesId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(CONST_GRADE_CUSTOMER_CATEGORIES_ID);
            }
        }

        public static string PathCoursesGradebookCustomCategories
        {
            get
            {
                return ConfigUtil.GetConfigurationString(PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES);
            }
        }

        public static string PathCoursesGradebookCustomCategories_
        {
            get
            {
                return ConfigUtil.GetConfigurationString(PATH_COURSES_GRADEBOOK__CUSTOMCATEGORIES_);
            }
        }

        public static string PathCoursesMsOfficeDocumentsOriginalDocument
        {
            get
            {
                return ConfigUtil.GetConfigurationString(PATH_COURSES_MSOFFICEDOCUMENTS_ORIGINALDOCUMENT);
            }
        }

        public static string TeacherUsername
        {
            get
            {
                return ConfigUtil.GetConfigurationString(TEACHER_USERNAME);
            }
        }

        public static string TeacherPassword
        {
            get
            {
                return ConfigUtil.GetConfigurationString(TEACHER_PASSWORD);
            }
        }

        public static string MsofficedocumentId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(MS_OFFICE_DOCUMENT_ID);
            }
        }

        public static string CourseId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(COURSE_ID);
            }
        }

        public static string AdminUserId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(ADMIN_USER_ID);
            }
        }

        public static string TaUserId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(TA_USER_ID);
            }
        }

        public static string TeacherUserId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(TEACHER_USER_ID);
            }
        }

        public static string StudentUserId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(STUDENT_USER_ID);
            }
        }

        public static string StudentUsername
        {
            get
            {
                return ConfigUtil.GetConfigurationString(STUDENT_USERNAME);
            }
        }

        public static string TaUsername
        {
            get
            {
                return ConfigUtil.GetConfigurationString(TA_USERNAME);
            }
        }

        public static string AdminUsername
        {
            get
            {
                return ConfigUtil.GetConfigurationString(ADMIN_USERNAME);
            }
        }

        public static string ExamId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(EXAM_ID);
            }
        }

        public static string ProtectedExamId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(PROTECTED_EXAM_ID);
            }
        }

        public static string ProtectedExamPassword
        {
            get
            {
                return ConfigUtil.GetConfigurationString(PROTECTED_EXAM_PASSWORD);
            }
        }

        public static string SectionId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(SECTION_ID);
            }
        }

        public static string MultiplechoiceQuestionType
        {
            get
            {
                return ConfigUtil.GetConfigurationString(MULTIPLECHOICE_QUESTION_TYPE);
            }
        }

        public static string TrueFalseQuestionType
        {
            get
            {
                return ConfigUtil.GetConfigurationString(TRUEFALSE_QUESTION_TYPE);
            }
        }

        public static string MatchingQuestionType
        {
            get
            {
                return ConfigUtil.GetConfigurationString(MATCHING_QUESTION_TYPE);
            }
        }

        public static string ManyMultipleChoiceQuestionType
        {
            get
            {
                return ConfigUtil.GetConfigurationString(MANYMULTIPLECHOICE_QUESTION_TYPE);
            }
        }

        public static string ShortAnswerQuestionType
        {
            get
            {
                return ConfigUtil.GetConfigurationString(SHORTANSWER_QUESTION_TYPE);
            }
        }

        public static string EssayQuestionType
        {
            get
            {
                return ConfigUtil.GetConfigurationString(ESSAY_QUESTION_TYPE);
            }
        }

        public static string FillInTheBlankQuestionType
        {
            get
            {
                return ConfigUtil.GetConfigurationString(FILLINTHEBLANK_QUESTION_TYPE);
            }
        }

        public static string UnitId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(UNIT_ID);
            }
        }

        public static string GradebookItemId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(GRADEBOOK_ITEM_ID);
            }
        }

        public static string ItemId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(ITEM_ID);
            }
        }

        public static string MultimediaId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(MULTIMEDIA_ID);
            }
        }

        public static string MultimediaContentPath
        {
            get
            {
                return ConfigUtil.GetConfigurationString(MULTIMEDIA_CONTENT_PATH);
            }
        }

        public static string MsofficedocumentContentPath
        {
            get
            {
                return ConfigUtil.GetConfigurationString(MSOFFICEDOCUMENT_CONTENT_PATH);
            }
        }

        public static string WebcontentuploadId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(WEBCONTENTUPLOAD_ID);
            }
        }

        public static string WebcontentuploadContentPath
        {
            get
            {
                return ConfigUtil.GetConfigurationString(WEBCONTENTUPLOAD_CONTENT_PATH);
            }
        }

        public static string ThreadId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(THREAD_ID);
            }
        }

        public static string TopicId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(TOPIC_ID);
            }
        }

        public static string ResponseId
        {
            get
            {
                return ConfigUtil.GetConfigurationString(RESPONSE_ID);
            }
        }
        
    }
}
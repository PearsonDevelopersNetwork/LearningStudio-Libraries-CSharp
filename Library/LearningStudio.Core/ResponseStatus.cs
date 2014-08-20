using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LearningStudio.Core
{
    public class ResponseStatus
    {
        public enum InnerEnum
        {
            OK = 200,
            CREATED = 201,
            NO_CONTENT = 204,
            BAD_REQUEST = 400,
            FORBIDDEN = 403,
            NOT_FOUND = 404,
            INTERNAL_ERROR = 500
        }
    }
}

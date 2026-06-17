using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Localization
{
    public static class ValidationErrors
    {
        public const string ValidationError = "ValidationError";
        public const string Required = "Required";
        public const string MinLength = "MinLength";
        public const string MaxLength = "MaxLength";
        public const string MinValue = "MinValue";
        public const string MaxValue = "MaxValue";
        public const string GreaterThan = "GreaterThan";
        public const string InvalidEmail = "InvalidEmail";
        public const string AlreadyUsed = "AlreadyUsed";
        public const string AlreadyExists = "AlreadyExists";
    }
}

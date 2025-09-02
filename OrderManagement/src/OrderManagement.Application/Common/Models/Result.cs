using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Common.Models
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string Error { get; private set; } = string.Empty;

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, string.Empty);
        public static Result Failure(string error) => new(false, error);

        public static explicit operator Result(string error) => Failure(error);
    }
}

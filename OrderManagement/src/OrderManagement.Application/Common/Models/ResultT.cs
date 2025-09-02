using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Common.Models
{
    public  class Result<T> : Result
    {
        public T? Value { get; private set; }

        private Result(bool isSuccess, T? value, string error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(true, value, string.Empty);
        public static new Result<T> Failure(string error) => new(false, default, error);

        public static implicit operator Result<T>(T value) => Success(value);
      //  public static implicit operator Result<T>(string error) => Failure(error);
    }
}


namespace ItemProposalAPI.Validation
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public ErrorType ErrorType { get; set; }
        public List<object> Errors { get; set; } = new List<object>();

        public static Result<T> Success(T data) => new Result<T> { IsSuccess = true, Data = data };
        public static async Task<Result<T>> Success(Task<T> dataTask)
        {
            var data = await dataTask;
            return new Result<T> { IsSuccess = true, Data = data };
        }
        public static Result<T> Failure(ErrorType errorType, object error) => new Result<T> { IsSuccess = false, ErrorType = errorType ,Errors = new List<object> { error } };
        public static Result<T> Failure(ErrorType errorType, List<object> errors) => new Result<T> { IsSuccess = false, ErrorType = errorType, Errors = errors};
    }

    public enum ErrorType
    {
        None,         // Indicates no error
        NotFound,     // Represents a 404 error
        BadRequest, // Represents a 400 error
        Unauthorized, // Represents a 401 error
        Forbidden, //Represents a 403 error
        Conflict,     // Represents a 409 error (if needed)
        GeneralError  // Represents a 500 error or other unspecified issues
    }
}

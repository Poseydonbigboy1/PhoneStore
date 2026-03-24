namespace PhoneStore.Helpers
{
    public class ResultObject<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = String.Empty;
        public T? Data { get; set; }

        public static ResultObject<T> Success(T data)
        {
            return new ResultObject<T> { IsSuccess = true,  Data = data, Message = string.Empty };
        }

        public static ResultObject<T> Error(string message)
        {
            return new ResultObject<T> { IsSuccess = false, Data = default, Message = message };
        }

        public static ResultObject<T> Error(Exception ex)
        {
            return new ResultObject<T> { IsSuccess = false, Data = default, Message = ex.Message };
        }
    }
}

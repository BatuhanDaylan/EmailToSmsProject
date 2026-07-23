namespace EmailToSms.Api.Common
{
    public class ResultOfT<T> : Result
    {
        public T? Data { get; set; }

        private ResultOfT(bool isSuccess , string? message , int statusCode , T? data , List<string>? errors = null)
            : base(isSuccess, message, statusCode , errors)
        {
            Data = data;
        }

        //Başarılı ve veri taşıyan durumlar için yardımcı metot
        public static ResultOfT<T> Success(T data, string message = "İşlem başarıyla gerçekleşti.", int statusCode = 200)
        {
            return new ResultOfT<T>(true, message, statusCode, data);
        }

        //Başarısız durumlar için yardımcı metot
        public static new ResultOfT<T> Failure(string message, List<string> errors, int statusCode = 400)
        {
            return new ResultOfT<T>(false, message, statusCode, default, errors);
        }

        //Başarısız durumlar (Tek hata) için yardımcı metot
        public static new ResultOfT<T> Failure(string message, string error, int statusCode = 400)
        {
            return new ResultOfT<T>(false, message, statusCode, default, new List<string> { error });
        }
    }
}
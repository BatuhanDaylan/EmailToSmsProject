using System.Text.Json.Serialization;

namespace EmailToSms.Api.Common
{
    //veri taşımayan hata sınıfı
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        [JsonIgnore]
        public int StatusCode { get; set; }

        public Result(bool isSuccess , string? message , int statusCode , List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            StatusCode = statusCode;
            Errors = errors ?? new List<string>();
        }

        //işlem başarılıysa
        public static Result Success(string message = "İşlem başarıyla gerçekleşti." , int statusCode = 200)
        {
            return new Result(true , message , statusCode);
        }

        //işlem başarısızsa hata listesi döndüren yardımcı method
        public static Result Failure(string message , List<string> errors , int statusCode = 400)
        {
            return new Result(false , message , statusCode , errors);
        }

        //işlem başarısızsa tek bir hata döndüren yardımcı method
        public static Result Failure(string message , string error , int statusCode = 400)
        {
            return new Result(false, message, statusCode , new List<string> { error });
        }
    }
}

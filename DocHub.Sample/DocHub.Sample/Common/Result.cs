namespace DocHub.Sample.Common;

public enum ResultCode
{
    Default = 0,
    NeedOtpConfirmation = 100,
    Unauthorized = 401
}

public class Result
{
    public bool Success { get; set; }
    public ResultCode Code { get; set; }
    public List<string> Messages { get; set; } = [];

    public Result()
    {
    }

    public Result(bool success)
    {
        Success = success;
    }

    public Result(params string[] messages)
    {
        Messages = messages.ToList();
    }
}

public enum ReceiveOtpMethod
{
    Email = 1,
    Sms = 2,
    None = -1
}

public static class ReceiveOtpMethodExtensions
{
    private static readonly Dictionary<ReceiveOtpMethod, string> Descriptions = new()
    {
        { ReceiveOtpMethod.Email, "Email" },
        { ReceiveOtpMethod.Sms, "Sms" },
        { ReceiveOtpMethod.None, "Không có" }
    };

    public static int GetCode(this ReceiveOtpMethod method) => (int)method;

    public static string GetDescription(this ReceiveOtpMethod method)
    {
        return Descriptions.GetValueOrDefault(method, "Không xác định");
    }

    public static ReceiveOtpMethod FromCode(int code)
    {
        if (Enum.IsDefined(typeof(ReceiveOtpMethod), code))
        {
            return (ReceiveOtpMethod)code;
        }

        throw new ArgumentException($"Không tìm thấy phương thức OTP với code: {code}");
    }

    public static string GetDescriptionFromCode(int code)
    {
        if (Enum.IsDefined(typeof(ReceiveOtpMethod), code))
        {
            return ((ReceiveOtpMethod)code).GetDescription();
        }

        return $"Không tìm thấy phương thức OTP với code: {code}";
    }
}

public class Result<TResultData> : Result
{
    public TResultData? Data { get; set; }

    public Result()
    {
    }

    public Result(bool success)
    {
        Success = success;
    }

    public Result(TResultData data)
    {
        Success = true;
        Data = data;
    }

    public Result(params string[] messages)
    {
        Messages = messages.ToList();
    }
}
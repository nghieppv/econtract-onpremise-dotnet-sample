namespace DocHub.Sample.Common;

public class UpdateProcessDocumentRequest
{
    public string? Id { get; set; }
    public bool ProcessInOrder { get; set; }
    public List<ProcessesRequest>? Processes { get; set; }
}

public class ProcessesRequest
{
    public int OrderNo { get; set; }
    public string? ProcessedByUserCode { get; set; }
    public string? AccessPermissionCode { get; set; }
    public string? Position { get; set; }
    public int PageSign { get; set; }

    public ProcessesRequest()
    {
    }

    public ProcessesRequest(int orderNo, string processedByUserCode, string accessPermissionCode,
        string position, int pageSign)
    {
        OrderNo = orderNo;
        ProcessedByUserCode = processedByUserCode;
        AccessPermissionCode = accessPermissionCode;
        Position = position;
        PageSign = pageSign;
    }
}
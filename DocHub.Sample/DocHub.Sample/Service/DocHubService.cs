using DocHub.Sample.Common;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace DocHub.Sample.Service;

public interface IDocHubService
{
    Task<Result<string>> AuthenticateAsync(string? username, string? password, string? comId, string? baseUrl);
    Task<Result<BatchImportDto>> CreateBatchImportAsync(CreateBatchImportDataDto dto);
    Task<Result<BatchImportDto>> SendBatchDocumentAsync(int id);
    Task<Result<DocumentTemplateDto>> GetDocumentTemplateAsync(int id);
    Task<Result<DocumentDto>> CreateDocumentAsync(CreateDocumentRequest request);
    Task<Result<DocumentDto>> UpdateProcessDocumentAsync(UpdateProcessDocumentRequest dto);
    Task<Result<DocumentDto>> SendProcessDocumentAsync(string batchId);

    Task<Result<DocumentsRequest>> GetDocumentAsync(string search, int pageSize = 10, int page = 1,
        bool waitToESign = false);

    Task<Result<ProcessRespone>> ProcessAsync(ProcessDto request);
    Task<Result<DocumentDto>> SendNotifyAsync(string documentId);
}

public class DocHubService : IDocHubService
{
    private static readonly HttpClient HttpClient = new();
    private static string? _accessToken;
    private static string? _baseUrl;
    private static string? _comId;

    public async Task<Result<string>> AuthenticateAsync(string? username, string? password, string? comId,
        string? baseUrl)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(baseUrl))
        {
            return new("Thiếu thông tin đăng nhập");
        }

        _baseUrl = baseUrl;
        _comId = comId;
        var httpContent = new StringContent(JsonConvert.SerializeObject(new
        {
            username,
            password,
            companyId = comId
        }), Encoding.UTF8, "application/json");

        var response = await HttpClient.PostAsync(_baseUrl + "/api/auth/password-login", httpContent);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Result<string>>(content);

        if (result?.Success == true)
        {
            _accessToken = result.Data;
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        //Console.WriteLine(result?.Messages[0]);
        return result ?? new("Đăng nhập thất bại");
    }

    public async Task<Result<BatchImportDto>> CreateBatchImportAsync(CreateBatchImportDataDto dto) =>
        await PostAsync<BatchImportDto>("/api/batch-imports/create-advanced", dto);

    public async Task<Result<BatchImportDto>> SendBatchDocumentAsync(int id) =>
        await PostAsync<BatchImportDto>($"/api/batch-imports/send/{id}", null);

    public async Task<Result<DocumentTemplateDto>> GetDocumentTemplateAsync(int id) =>
        await GetAsync<DocumentTemplateDto>($"/api/document-templates/{id}");

    public async Task<Result<DocumentDto>> CreateDocumentAsync(CreateDocumentRequest request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/api/documents/create");
        var content = new MultipartFormDataContent
        {
            { new StringContent(request.No!), "No" },
            { new StringContent(request.Subject!), "Subject" },
            { new StringContent(request.Description ?? ""), "Description" },
            { new StringContent(request.TypeId.ToString()), "TypeId" },
            { new StringContent(request.DepartmentId.ToString()), "DepartmentId" }
        };
        if (!string.IsNullOrEmpty(request.FileInfo.FilePath))
        {
            content.Add(new StreamContent(File.OpenRead(request.FileInfo.FilePath)), "File",
                request.FileInfo.FileName!);
        }
        else
        {
            content.Add(new ByteArrayContent(request.FileInfo.File!), "File", request.FileInfo.FileName!);
        }

        httpRequest.Content = content;
        return await SendAsync<DocumentDto>(httpRequest);
    }

    public async Task<Result<DocumentDto>> UpdateProcessDocumentAsync(UpdateProcessDocumentRequest dto) =>
        await PostAsync<DocumentDto>("/api/documents/update-process", dto);

    public async Task<Result<DocumentDto>> SendProcessDocumentAsync(string batchId) =>
        await PostAsync<DocumentDto>($"/api/documents/send-process/{batchId}", null);

    public async Task<Result<DocumentsRequest>> GetDocumentAsync(string search, int pageSize = 10, int page = 1,
        bool waitToESign = false)
    {
        var uri =
            $"/api/documents?search={HttpUtility.UrlEncode(search)}&pageSize={pageSize}&page={page}&WaitToESign={waitToESign}";
        return await GetAsync<DocumentsRequest>(uri);
    }

    public async Task<Result<ProcessRespone>> ProcessAsync(ProcessDto request)
    {
        var jsonPayload = JsonConvert.SerializeObject(request,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync(_baseUrl + "/api/documents/process", httpContent);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Result<ProcessRespone>>(content);
        return result ?? new("Thất bại");
    }

    public async Task<Result<DocumentDto>> SendNotifyAsync(string documentId) =>
        await PostAsync<DocumentDto>($"/api/documents/send-notify/{documentId}", null);

    private async Task<Result<T>> GetAsync<T>(string url)
    {
        var response = await HttpClient.GetAsync(_baseUrl + url);
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Result<T>>(content) ?? new("Thất bại");
    }

    private async Task<Result<T>> PostAsync<T>(string url, object? payload)
    {
        var jsonPayload = JsonConvert.SerializeObject(payload);
        var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync(_baseUrl + url, httpContent);
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Result<T>>(content) ?? new("Thất bại");
    }

    private async Task<Result<T>> SendAsync<T>(HttpRequestMessage request)
    {
        var response = await HttpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Result<T>>(content) ?? new("Thất bại");
    }
}
using Microsoft.JSInterop;
using Common.Helpers.IO;

public static class AspDotNetExtensions
{
    public static async Task<string> DownloadExcelStream<T>(this IJSRuntime jsRuntime, IEnumerable<T>? data, string fileName = "", int[]? hide = null) where T : class
    {
        if (data != null && data.Any())
        {
            using MemoryStream fileStream = data.WriteToStream(ref fileName, hideColumnIndex: hide);
            await jsRuntime.DownloadFileFromStream(fileStream, fileName);
        }
        return fileName;
    }

    public static async Task DownloadExcelUrl<T>(this IJSRuntime jsRuntime, IEnumerable<T>? data, string filePath, string fileName = "", string fileHost = "https://localhost:5001/files", int[]? hide = null) where T : class
    {
        if (data != null && data.Any())
        {
            string fullPath = Path.Combine(filePath, fileName);
            string fullName = data.WriteToExcel(fullPath, hideColumnIndex: hide);
            string fileUrl = $"{fileHost}/{Path.GetFileName(fullName)}";
            await jsRuntime.DownloadFileFromUrl(fileUrl, fullName);
        }
    }

    public static async Task DownloadFileFromStream(this IJSRuntime jsRuntime, Stream stream, string fileName = "")
    {
        if (stream != null)
        {
            stream.Position = 0;
            using var streamRef = new DotNetStreamReference(stream);
            await jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
    }

    public static async Task DownloadFileFromUrl(this IJSRuntime jsRuntime, string fileUrl, string fileName = "")
    {
        if (!string.IsNullOrEmpty(fileUrl))
            await jsRuntime.InvokeVoidAsync("triggerFileDownload", fileName, fileUrl);
    }
}
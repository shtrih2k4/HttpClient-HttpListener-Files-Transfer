using System.Net;

internal partial class Program
{
    
    static HttpClient httpClient = new();
    static async Task Main()
    {
        string uri = "http://127.0.0.1:8888/connection/";
        string[] filepaths = new[] { @"C:\Users\zevas\Desktop\1.jpeg",@"C:\Users\zevas\Desktop\2.jpg",@"C:\Users\zevas\Desktop\3.jpg" };
        StringContent amount_of_files = new(filepaths.Length.ToString());
        using var response1=await httpClient.PostAsync(uri, amount_of_files);
        string response1text = await response1.Content.ReadAsStringAsync();
        Console.WriteLine(response1text);
        foreach (string filename in filepaths)
        {
            using var response2=await httpClient.PostAsync(uri,new StringContent(filename.Split(@"\")[^1]));
            string response2Text= await response2.Content.ReadAsStringAsync();

            StreamContent content = new(File.OpenRead(filename));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            using HttpResponseMessage response3 = await httpClient.PostAsync(uri, content);

            string responseText = await response3.Content.ReadAsStringAsync();
            Console.WriteLine(responseText);
        }   
    }
}

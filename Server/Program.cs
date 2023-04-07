using System.Net;
using System.Text;

HttpListener server = new();
try
{
    server.Prefixes.Add("http://127.0.0.1:8888/connection/");
   
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
    return;
}
string uploadpath = @$"{Directory.GetCurrentDirectory()}\uploads";
Directory.CreateDirectory(uploadpath);
server.Start();
var context = await server.GetContextAsync();
byte[] amount_of_files_in_bytes = new byte[4];
await context.Request.InputStream.ReadAsync(amount_of_files_in_bytes);

string amount_of_files_as_string = Encoding.UTF8.GetString(amount_of_files_in_bytes);
if (int.TryParse(amount_of_files_as_string, out int amount_of_files))
{


    byte[] amount_of_files_message = Encoding.UTF8.GetBytes($"Server has detected {amount_of_files} files");
    context.Response.ContentLength64 = amount_of_files_message.Length;
    context.Response.ContentType = "text/plain";
    //context.Response.StatusCode = 200;
    await context.Response.OutputStream.WriteAsync(amount_of_files_message);
    for(int i=0;i<amount_of_files;i++)
{
    var filename_context = await server.GetContextAsync();
    Console.WriteLine($"Connection:{context.Request.RemoteEndPoint}");
    byte[] filename_as_bytes = new byte[256];
    
    int wrote_bytes=await filename_context.Request.InputStream.ReadAsync(filename_as_bytes);

    string filename = Encoding.UTF8.GetString(filename_as_bytes,0,wrote_bytes);

    byte[] filename_message = Encoding.UTF8.GetBytes($"File {filename} is starting to downloading");
    filename_context.Response.ContentLength64=filename_message.Length;
    filename_context.Response.ContentType = "text/plain";
    //filename_context.Response.StatusCode = 200;
    
    
    await filename_context.Response.OutputStream.WriteAsync(filename_message,0,filename_message.Length);

    
    var file_context=await server.GetContextAsync();
    string fullpath = $"{uploadpath}\\{filename}";
    using (var filestream = new FileStream(fullpath, FileMode.Create))
    {
        await file_context.Request.InputStream.CopyToAsync(filestream);
    }
    HttpListenerResponse response = file_context.Response;
    byte[] message = Encoding.UTF8.GetBytes($"File {filename} has been downloaded");
    response.ContentLength64 = message.Length;
    response.ContentType = "text/plain";
    
    //response.StatusCode = 200;
    
    await response.OutputStream.WriteAsync(message);
}
}
else
{
    byte[] amount_error = Encoding.UTF8.GetBytes("Server can not detect any files");
    context.Response.ContentLength64 = amount_error.Length;
    context.Response.ContentType = "text/plain";
    //context.Response.StatusCode = 200;
    await context.Response.OutputStream.WriteAsync(amount_error);
    return;
}





server.Stop();

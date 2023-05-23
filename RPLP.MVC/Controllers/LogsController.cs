using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RPLP.JOURNALISATION;
using RPLP.MVC.Models;

namespace RPLP.MVC.Controllers;

public class LogsController : Controller
{
    private string _path = @"/var/log/rplp";
    private string _fileName = "Log_Revue_Par_Les_Paires.csv";

    private readonly HttpClient _httpClient;


    public LogsController()
    {
        this._httpClient = new HttpClient();
        this._httpClient.BaseAddress = new Uri("http://rplp.api/api/");
        this._httpClient.DefaultRequestHeaders.Accept
            .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        this._httpClient.DefaultRequestHeaders.Accept
            .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream"));
    }


    [HttpGet]
    public ActionResult ClearLogs()
    {
        RPLP.JOURNALISATION.Logging.Instance.ClearLogs();
        return Ok();
    }
    
    [HttpGet]
    public ActionResult ExportLogs()
    {
        string filePath = Path.Combine(_path, _fileName);
        var fs = new FileStream(filePath, FileMode.Open); // convert it to a stream

        // Return the file. A byte array can also be used instead of a stream
        return File(fs, "application/octet-stream", $"{Guid.NewGuid()}_Logs.csv");
    }

    public IActionResult Error(int? statusCode = null)
    {
        List<int> statues = new List<int>() { 400, 401, 402, 403, 404, 405, 406, 407, 408, 500, 501, 502, 503, 504 };

        if (statusCode.HasValue)
        {
            if (statues.Contains((int)statusCode))
            {
                var viewName = statusCode.ToString();
                return View(viewName);
            }
            else
            {
                return View((object)400);
            }
        }
        else
        {
            return View((object)400);
        }
    }

    public IActionResult Index()
    {
        string? email = User.FindFirst(u => u.Type == ClaimTypes.Email)?.Value;

        string? userType = this._httpClient
                    .GetFromJsonAsync<string>($"Verificator/UserType/{email}")
                    .Result;

        if (userType == "")
        {
            return Error(403);
        }
        else
        {
            string filePath = Path.Combine(_path, _fileName);

            if (string.IsNullOrWhiteSpace(filePath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "LogsController - Index - la variable filePath est null ou vide", 0));
            }

            if (!System.IO.File.Exists(filePath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                              "LogsController - Index - le fichier /var/log/rplp/Log_Revue_Par_Les_Paires.csv est introuvable ", 0));
            }

            string csvData = System.IO.File.ReadAllText(filePath);
            DataTable dt = new DataTable();
            bool firstRow = true;
            foreach (string row in csvData.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        if (firstRow)
                        {
                            foreach (string cell in row.Split('~'))
                            {
                                dt.Columns.Add(cell.Trim());
                            }

                            firstRow = false;
                        }
                        else
                        {
                            dt.Rows.Add();
                            int i = 0;
                            foreach (string cell in row.Split('~'))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Trim();
                                i++;
                            }
                        }
                    }
                }
            }

            return View(dt);
        }
    }
}
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RPLP.JOURNALISATION;

namespace RPLP.MVC.Controllers;

public class LogsController : Controller
{
    private string _path = @"/var/log/rplp";
    private string _fileName = "Log_Revue_Par_Les_Paires.csv";
    
    [HttpGet]
    public ActionResult ClearLogs()
    {
        RPLP.JOURNALISATION.Logging.Instance.ClearLogs();
        RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Methode ClearLogs appeler"));
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

    public IActionResult Index()
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
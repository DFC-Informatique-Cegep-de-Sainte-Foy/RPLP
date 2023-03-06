using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace RPLP.MVC.Controllers;

public class LogsController : Controller
{
    public IActionResult Index()
    {
        string path = @"/var/log/rplp";

        string fileName = "Log_Revue_Par_Les_Paires.csv";
        string filePath = Path.Combine(path, fileName);

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
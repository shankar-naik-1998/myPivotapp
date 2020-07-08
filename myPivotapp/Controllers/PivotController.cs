using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using myPivotapp.BussinesLogic;
using myPivotapp.Models;
using Newtonsoft.Json.Linq;
using System.IO;

using Newtonsoft.Json;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Collections;
using System.Data;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace myPivotapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PivotController : ControllerBase
    {
       
        public static IPivotServices _pivotServices;
        public static JObject pivotInputJsonFIle;
        public PivotController(IPivotServices pivotServices)
        {
            _pivotServices = pivotServices;
            JObject pivotInputJsonFIle;
        }

        [Route("getPivot")]
        [HttpGet]
        public dynamic getPivot([FromQuery] string row, [FromQuery] string column, [FromQuery] string data)
        {

            try
            {/*
                if (Inputfile == null || Inputfile.Length == 0)
                    return Content("file not selected");*/

                var filepathforref = Path.Combine(
                           Directory.GetCurrentDirectory(), "refertext.txt");
                var filepath = System.IO.File.ReadAllText(filepathforref);

                

                string path = filepath;
                

                //Use when you can save the file-- string extension = Path.GetExtension(Inputfile.FileName);
                //string path = @"C:\pivotInputFiles\SalesJan2009.xlsx";
                string extension = Path.GetExtension(path);

              

                //string inputJsonString = string.Empty;
                DataTable dt = new DataTable();

                if (extension.ToString().ToUpper() == ".CSV")
                {
                    dt = _pivotServices.CSVToDataTable(path);

                }
                else if (extension.ToString().ToUpper() == ".XLSX")
                {
                    dt = _pivotServices.ExcelToDataTable(path);

                }
                
              

                //inputJsonString = JsonConvert.SerializeObject(pivotInputJsonFIle);
                //string text = System.IO.File.ReadAllText(@"C:\Users\user\pivotInputData.json");



                /*IEnumerable<BsonDocument> bsonIEnumerable = new List<BsonDocument>();
                string[] separatingChars = { "[","]" }; // split on these chars
                string[] docs = inputJsonString.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
                foreach(var doc in docs)
                {*/
                //var file = BsonSerializer.Deserialize<BsonDocument>(inputJsonString);



                // }
                dynamic result=_pivotServices.Create(dt, row, column, data);
               System.IO.File.Delete(filepath);
                System.IO.File.Delete(filepathforref);

                return result;



                /*var  input = System.IO.File.ReadAllText(@"C:\Users\user\pivotInputData.json");
                JObject result = JObject.Parse(input);*/



                /*PivotInputModel data = new PivotInputModel
                {
                    Id = Guid.NewGuid().ToString(),
                    pivotInput = BsonDocument.Parse(result.ToString())

                };*/
                

            }
            catch (Exception e)
            {
                return null;
            }
        }
        [Route("getColumn")]
        [HttpPost]
        public IEnumerable<string> GetFormColumns([FromForm] IFormFile Inputfile)
        {
            try
            {
                var filepath = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           Inputfile.FileName);

                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    Inputfile.CopyToAsync(stream);
                }

                string path = filepath;
                string extension = Path.GetExtension(path);
                //Store the file to local with name as Temp for Future reference
                /* var filepathforfuture = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           string.Format("tempFile{0}", extension));

                using (var futurestream = new FileStream(filepathforfuture, FileMode.Create))
                {
                    Inputfile.CopyToAsync(futurestream);
                }*/

                //Store the file filepathforfuture tolocal txt file
               var txtfilepathforfuture = Path.Combine(
                           Directory.GetCurrentDirectory(), "refertext.txt");

                System.IO.File.WriteAllText(txtfilepathforfuture, filepath);



                //--till here

                //string inputJsonString = string.Empty;
                DataTable dt = new DataTable();

                if (extension.ToString().ToUpper() == ".CSV")
                {
                    dt = _pivotServices.CSVToDataTable(path);

                }
                else if (extension.ToString().ToUpper() == ".XLSX")
                {
                    dt = _pivotServices.ExcelToDataTable(path);

                }
                string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
                return columnNames;


            }
            catch (Exception e)
            {
                return null;
            }


        }

        [HttpGet]
        public string Get()
        {
            return "Welcome to Pivot App :) ";
        }
    }
}


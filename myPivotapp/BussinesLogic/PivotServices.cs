using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting.Internal;
using MongoDB.Bson;
using MongoDB.Driver;
using myPivotapp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spire.Xls;
using IronXL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.IO;
using System.Linq.Expressions;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace myPivotapp.BussinesLogic
{
    public class PivotServices : IPivotServices
    {
        private readonly IMongoCollection<BsonDocument> _pivotInput;
        private IHostingEnvironment _env;


        public PivotServices(IPivotDatabaseSettings settings, IHostingEnvironment env)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);


            _pivotInput = database.GetCollection<BsonDocument>(settings.BooksCollectionName);
            _env = env;


        }

        public  dynamic Create(DataTable pivotInput, string row, string column, string data)
        {
            string row1 = row;
            string col1 = column;
            string data1 = data;

            try
            {
                /*PivotInputModel model= new PivotInputModel()
            {
                Id="101",
                Name="Ramesh",
                City="Bangallore"
            };*/

                //var obj = "{'id':'1','name':'Rakesh','city':'Bangalore'}";
                // var dbObject = JObject.Parse(obj);



                // _pivotInput.InsertOne(pivotInput);
                List<BsonDocument> batch = new List<BsonDocument>();
                foreach (DataRow dr in pivotInput.Rows)
                {
                    var dictionary = dr.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dr[col.ColumnName]);
                    batch.Add(new BsonDocument(dictionary));
                }

                 _pivotInput.InsertManyAsync(batch.AsEnumerable());
                var data2 = pivotInput.AsEnumerable().Select(x => new { 
                    row_Key = x.Field<String>(row),
                    Pivotted_column = x.Field<String>(column),
                    data = Convert.ToDouble( x.Field<String>(data))
                });

                DataTable pivotDataTable = data2.ToPivotTable(
                     item => item.row_Key,
                    item => item.Pivotted_column,
                    items => items.Any() ? items.Sum(x => x.data) : 0);


                /*dynamic pt =  pivotDataTable.Rows.Cast<DataRow>()
                     .Select(row => pivotDataTable.Columns.Cast<DataColumn>()
                     .Select(col => Convert.ToString(row[col]))
                     .ToArray())
                     .ToList();*/
                //dynamic pt = pivotDataTable.AsEnumerable().ToArray();

                string JSONString = string.Empty;
                JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(pivotDataTable);
                



                /*string[,] stringArray = new string[pivotDataTable.Rows.Count,pivotDataTable.Columns.Count];

                for (int row = 0; row < pivotDataTable.Rows.Count; row++)
                {
                    for (int col = 0; col < pivotDataTable.Columns.Count; col++)
                    {
                        stringArray[row, col] = pivotDataTable.Rows[row][col].ToString();
                    }
                }*/
                //Delete Afterward Only to store PivotedTable on to MongoDB
                List<BsonDocument> newbatch = new List<BsonDocument>();
                foreach (DataRow dr in pivotDataTable.Rows)
                {
                    var dictionary = dr.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dr[col.ColumnName]);
                    newbatch.Add(new BsonDocument(dictionary));
                }

                _pivotInput.InsertManyAsync(newbatch.AsEnumerable());

                /*var dotNetObjList = newbatch.ConvertAll(BsonTypeMapper.MapToDotNetValue);

                Newtonsoft.Json.JsonConvert.SerializeObject(dotNetObjList);*/
                /*string JSONString = string.Empty;
                return JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(pivotDataTable);*/



                //--------till here
                return JSONString;


            }
            catch (Exception e)
            {
                return null;
            }

        }

        public DataTable ExcelToDataTable(string filepath)
        {
            try
            {
                /* string ConnectionString = string.Format(" Provider = Microsoft.ACE.OLEDB.12.0;Data Source ={0};Extended Properties = Excel 5.0", file.FileName);


                 StringBuilder stbQuery = new StringBuilder();
                 stbQuery.Append("SELECT top 10 * FROM [A1:M98]");
                 OleDbDataAdapter adp = new OleDbDataAdapter(stbQuery.ToString(), ConnectionString);

                 DataTable pivotInput = new DataTable();
                 adp.Fill(pivotInput);
                 //-----Convert to Json
                 var input = Newtonsoft.Json.JsonConvert.Seriali

                zeObject(pivotInput);
                 return JObject.Parse(input);*/

                //---------------
                //get content path 

                //GC.WaitForPendingFinalizers();
                /*var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                string folderName =@"c://";
                string directorypathString = System.IO.Path.Combine(folderName, "SubFolder");
                System.IO.Directory.CreateDirectory(directorypathString);
                string fileName = "pivotInputFile";
                string filepathString = System.IO.Path.Combine(directorypathString, fileName);
                System.IO.File.Create(filepathString);

                //var uploads = Path.Combine(_env.ContentRootPath, "uploads");
                //var filePath = Path.Combine(uploads, file.FileName);
                GC.Collect();
                
                using (var fileStream = new FileStream(filepathString, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                Stream filestream = file.OpenReadStream();
                var inFilePath = file.FileName.ToString();
                var outFilePath = @"InputJson.json";
                string filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                //filename = this.EnsureCorrectFilename(filename);
                //FileStream output = System.IO.File.Create(this.GetPathAndFilename(filename);

                //using (var inFile = System.IO.File.Open(inFilePath, FileMode.Open, FileAccess.Read))
                using (var outFile = System.IO.File.CreateText(outFilePath))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(filestream, new ExcelReaderConfiguration()
                    { FallbackEncoding = Encoding.GetEncoding(1252) }))
                    {
                        var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });
                        var table = ds.Tables[0];
                        var json = JsonConvert.SerializeObject(table, Formatting.Indented);
                        return JObject.Parse(json);
                    }
                }

*/
                /*WorkBook workbook = WorkBook.Load(filepath);
               var jsonfilepath= Path.Combine(_env.ContentRootPath, "exportJson.json");
                File.Create(jsonfilepath);
                workbook.SaveAsJson(jsonfilepath);
                JObject data = JObject.Parse(File.ReadAllText(jsonfilepath));
                return data;*/
                Workbook workbook = new Workbook();
                workbook.LoadFromFile(filepath);

                //Get the first worksheet
                Worksheet sheet = workbook.Worksheets[0];
                //Export data to data table
                DataTable dt = sheet.ExportDataTable();

                //Remove Empty Rows from DataTable

                dt = dt.Rows
             .Cast<DataRow>()
            .Where(row => !row.ItemArray.All(field => field is DBNull ||
                                     string.IsNullOrWhiteSpace(field as string)))
             .CopyToDataTable();

                foreach (var column in dt.Columns.Cast<DataColumn>().ToArray())
                {
                    if (dt.AsEnumerable().All(dr => dr.IsNull(column)))
                        dt.Columns.Remove(column);
                }

                return dt;


            }
            catch (Exception e)
            {
                return null;
            }



        }
        public DataTable CSVToDataTable(string filepath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(filepath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }


    }
    }


  

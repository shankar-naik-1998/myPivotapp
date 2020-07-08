using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using myPivotapp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace myPivotapp.BussinesLogic
{
    public interface IPivotServices
    {
        dynamic Create(DataTable pivotInput, string row, string column, string data);
        DataTable ExcelToDataTable(string filepath);
        DataTable CSVToDataTable(string filepath);



    }
}
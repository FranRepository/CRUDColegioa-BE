using System.Reflection;
using CTS_ReturnsApp.Models;
using Microsoft.AspNetCore.Mvc;

    namespace CTS_ReturnsApp.Interfaces
    {
        public interface IDb2Repository
        {


        public JsonResult InfoTruck(string vin);

        public List<IssueSTlModelSimple> GetIssuesShopIssueShoptechList(DateTime first, DateTime second, string vin);
        }
    }



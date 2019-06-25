using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MyTemplate.Core.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate.Web.Core.ViewComponents
{
    public enum AjaxMethod
    {
        GET = 0,
        POST = 1,
    }
    public enum AjaxUpdateMode
    {
        Replace = 0,
        ReplaceWith = 1,
        Before = 2,
        After = 3
    }

    public class PaginationOptions
    {
        public string Id { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string AjaxForm { get; set; }
        public AjaxMethod Method { get; set; }
        public AjaxUpdateMode UpdateMode { get; set; }
        public string UpdateTarget { get; set; }
        public string Begin { get; set; }
        public string Complete { get; set; }
        public IPaginationResult PaginationResult { get; set; }
    }


    public class PaginationOptionModel : PaginationOptions
    {
        public string QueryJson { get; set; }
        public long TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class Pagination : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(
            PaginationOptions options)
        {
            var model = new PaginationOptionModel()
            {
                Id = options.Id,
                TotalCount = options.PaginationResult.Total,
                PageNumber = options.PaginationResult.Page,
                PageSize = options.PaginationResult.Size,
                QueryJson = JsonConvert.SerializeObject(options.PaginationResult.Query),

                Controller = options.Controller,
                Action = options.Action,
                AjaxForm = options.AjaxForm,
                Method = options.Method,
                UpdateMode = options.UpdateMode,
                UpdateTarget = options.UpdateTarget,
                Begin = options.Begin,
                Complete = options.Complete
            };

            return View(await Task.FromResult(model));
        }
    }
}

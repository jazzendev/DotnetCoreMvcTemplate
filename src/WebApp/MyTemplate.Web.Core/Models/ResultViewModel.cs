using System;
namespace MyTemplate.Web.Core.Models
{
    public class ResultViewModel
    {
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; }
        public string Script { get; set; }
    }
}

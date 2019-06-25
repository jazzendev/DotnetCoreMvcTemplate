using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyTemplate.Web.Core.Models
{
    public class PortalLoginViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "请输入用户名。")]
        public string Username { get; set; }
        [Required(ErrorMessage = "请输入密码。")]
        [MinLength(6, ErrorMessage = "至少输入6位密码。")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsPasswordToChange { get; set; }
        public bool IsLocked { get; set; }

        public string CreatorId { get; set; }
        public string EditorId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastEditTime { get; set; }
        public bool IsValid { get; set; }

        public string Roles { get; set; }

        public IEnumerable<string> RoleList { get; set; }

        public bool IsPasswordChanged { get; set; }
    }
}

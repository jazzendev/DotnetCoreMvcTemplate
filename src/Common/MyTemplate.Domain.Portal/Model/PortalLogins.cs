using Dapper.Contrib.Extensions;
using MyTemplate.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MyTemplate.Domain.Portal.Model
{
    public enum PortalRoles
    {
        [Description("超级管理员")]
        SuperAdmin = 1,
        [Description("管理员")]
        Admin = 10
    }

    [Table("PortalLogins")]
    public class PortalLogin : IMonitorModel<string>
    {
        [ExplicitKey]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Domain { get; set; }
        public string PasswordHash { get; set; }
        public bool IsPasswordToChange { get; set; }
        public bool IsLocked { get; set; }

        public string CreatorId { get; set; }
        public string EditorId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastEditTime { get; set; }
        public bool IsValid { get; set; }
    }


    [Table("PortalRoles")]
    public class PortalRole : IDatabaseModel<string>
    {
        [ExplicitKey]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsBackend { get; set; }
        public bool IsDefault { get; set; }
        public bool IsValid { get; set; }
    }

    [Table("PortalLoginRoles")]
    public class PortalLoginRole
    {
        [ExplicitKey]
        public string LoginId { get; set; }
        [ExplicitKey]
        public string RoleId { get; set; }
    }
}

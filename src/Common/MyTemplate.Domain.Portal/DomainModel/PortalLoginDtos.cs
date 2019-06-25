using MyTemplate.Core.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTemplate.Domain.Portal.DomainModel
{
    public class PortalLoginQuery : PaginationQuery
    {
        public string Username { get; set; }
        public bool? IsValid { get; set; }
    }

    public class SimplePortalLoginDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
    }

    public class PortalLoginDto
    {
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

        public string Roles { get; set; }
        public IEnumerable<string> RoleList
        {
            get
            {
                if (string.IsNullOrEmpty(this.Roles))
                {
                    return null;
                }
                var roles = this.Roles.Split(',');
                return roles;
            }
        }
    }

    public class PortalRoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsBackend { get; set; }

        public bool IsDefault { get; set; }
        public bool IsValid { get; set; }
    }
}

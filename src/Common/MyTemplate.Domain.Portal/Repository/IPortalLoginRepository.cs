using MyTemplate.Core.Repository;
using MyTemplate.Domain.Portal.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTemplate.Domain.Portal.Repository
{
    public interface IPortalLoginRepository
    {
        PaginationResult<PortalLoginDto> GetPortalLogins(PortalLoginQuery query);

        PortalLoginDto VerifyPortalLogin(string username, string password);

        PortalLoginDto CreatePortalLogin(string username, string password, IEnumerable<string> roleIds, bool isPasswordToChange);

        PortalLoginDto GetPortalLogin(string id);

        PortalLoginDto UpdatePortalLogin(string id, string username, string password, IEnumerable<string> roleIds, bool isPasswordToChange, bool isPasswordChanged, bool isLocked);

        IEnumerable<SimplePortalLoginDto> GetOwnerCandidates();

        string GetCurrentLogin();

        IEnumerable<PortalRoleDto> GetRoles(bool? isValid = true);

        bool CheckUsernameAvaiable(string id, string username);
        bool ChangePassword(string id, string password, string newPassword);
    }
}

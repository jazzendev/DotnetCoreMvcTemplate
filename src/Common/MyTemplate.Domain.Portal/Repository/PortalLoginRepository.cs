using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using MyTemplate.Core.Repository;
using MyTemplate.Core.Security;
using MyTemplate.Core.Utility;
using MyTemplate.Domain.Portal.DomainModel;
using MyTemplate.Domain.Portal.Model;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.Extensions.Logging;

namespace MyTemplate.Domain.Portal.Repository
{
    public class PortalLoginRepository : MonitorRepository, IPortalLoginRepository
    {
        private readonly ILogger _logger;
        private readonly SimplePasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public PortalLoginRepository(IConnectionFactory connectionFactory,
                                     SimplePasswordHasher passwordHasher,
                                     ILogger<PortalLoginRepository> logger,
                                     IMapper mapper,
                                     IPrincipal principal) : base(connectionFactory, logger, principal)
        {
            _passwordHasher = passwordHasher;
            _logger = logger;
            _mapper = mapper;
        }

        public PaginationResult<PortalLoginDto> GetPortalLogins(PortalLoginQuery query)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var columnsCommand = @"
SELECT l.Id, l.Username, l.PasswordHash, l.IsPasswordToChange, l.CreatorId, l.CreationTime, l.EditorId, l.LastEditTime, l.IsValid,
  STUFF((
    SELECT ','+r.Name
    FROM PortalRoles r INNER JOIN PortalLoginRoles lr on lr.RoleId=r.Id
    WHERE lr.LoginId=l.Id
    FOR XML PATH('')
  ), 1, 1, '') as Roles";
                var queryCommand = @"
FROM [PortalLogins] l
WHERE (@IsValid is NULL or @IsValid=l.IsValid) AND
    (@Username is NULL or l.Username like @Username)";

                var pagingCommand = query.Page == -1 ? "" : @"
ORDER BY l.Id DESC
OFFSET @Offset ROWS
FETCH NEXT @Size ROWS ONLY; ";

                query.Username = string.IsNullOrWhiteSpace(query.Username) ?
                    null : $"%{query.Username.Trim()}%";
                var data = connection.Query<PortalLoginDto>($@"{columnsCommand} {queryCommand} {pagingCommand}", query).ToList();
                var count = connection.ExecuteScalar<long>($@"SELECT COUNT(l.Id) {queryCommand}", query);

                var result = CombinePaginationResult(query, data, count);

                return result;
            }
        }

        public PortalLoginDto VerifyPortalLogin(string username, string password)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var command = @"
SELECT u.*
FROM [PortalLogins] u
WHERE u.IsValid=1 AND u.Username = @Username";

                var login = connection.Query<PortalLogin>(command, new { Username = username }).FirstOrDefault();

                if (login == null)
                {
                    return null;
                }

                var verifyResult = _passwordHasher.VerifyHashedPassword(login.PasswordHash, password);

                if (!verifyResult)
                {
                    return null;
                }

                var dto = new PortalLoginDto()
                {
                    Id = login.Id,
                    Username = login.Username,
                    IsPasswordToChange = login.IsPasswordToChange,
                    IsLocked = login.IsLocked,
                    CreatorId = login.CreatorId,
                    EditorId = login.EditorId,
                    CreationTime = login.CreationTime,
                    LastEditTime = login.LastEditTime,
                    IsValid = login.IsValid
                };

                return dto;
            }
        }

        public PortalLoginDto CreatePortalLogin(string username, string password, IEnumerable<string> roleIds, bool isPasswordToChange)
        {
            var login = new PortalLogin()
            {
                Id = IdHelper.NewId(),
                Username = username,
                PasswordHash = _passwordHasher.HashPassword(password),
                IsPasswordToChange = isPasswordToChange,
                IsLocked = false,
                CreationTime = DateTime.UtcNow,
                CreatorId = CurrentUserId,
                EditorId = null,
                LastEditTime = null,
                IsValid = true,
            };

            var columnsCommand = @"
SELECT l.Id, l.Username, l.PasswordHash, l.IsPasswordToChange, l.CreatorId, l.CreationTime, l.EditorId, l.LastEditTime, l.IsValid,
  STUFF((
    SELECT ','+r.Name
    FROM PortalRoles r INNER JOIN PortalLoginRoles lr on lr.RoleId=r.Id
    WHERE lr.LoginId=l.Id
    FOR XML PATH('')
  ), 1, 1, '') as Roles";
            var queryCommand = @"
FROM [PortalLogins] l
WHERE l.Id=@Id";

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var transcation = connection.BeginTransaction())
                {
                    try
                    {
                        connection.Insert(login, transcation);
                        foreach (var r in roleIds)
                        {
                            var role = new PortalLoginRole()
                            {
                                LoginId = login.Id,
                                RoleId = r
                            };
                            connection.Insert(role, transcation);
                        }

                        var dto = connection.QueryFirstOrDefault<PortalLoginDto>($@"{columnsCommand} {queryCommand}", new { Id = login.Id }, transcation);

                        transcation.Commit();

                        return dto;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                        transcation.Rollback();
                        return null;
                    }
                }
            }
        }

        public PortalLoginDto GetPortalLogin(string id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var columnsCommand = @"
SELECT l.Id, l.Username, l.PasswordHash, l.IsPasswordToChange, l.CreatorId, l.CreationTime, l.EditorId, l.LastEditTime, l.IsValid,
  STUFF((
    SELECT ','+r.Name
    FROM PortalRoles r INNER JOIN PortalLoginRoles lr on lr.RoleId=r.Id
    WHERE lr.LoginId=l.Id
    FOR XML PATH('')
  ), 1, 1, '') as Roles";
                var queryCommand = @"
FROM [PortalLogins] l
WHERE l.Id=@Id";

                var login = connection.Query<PortalLoginDto>($@"{columnsCommand} {queryCommand}", new { Id = id }).FirstOrDefault();
                return login;
            }
        }

        public PortalLoginDto UpdatePortalLogin(string id, string username, string password, IEnumerable<string> roleIds, bool isPasswordToChange, bool isPasswordChanged, bool isLocked)
        {
            var columnsCommand = @"
SELECT l.Id, l.Username, l.PasswordHash, l.IsPasswordToChange, l.CreatorId, l.CreationTime, l.EditorId, l.LastEditTime, l.IsValid,
  STUFF((
    SELECT ','+r.Name
    FROM PortalRoles r INNER JOIN PortalLoginRoles lr on lr.RoleId=r.Id
    WHERE lr.LoginId=l.Id
    FOR XML PATH('')
  ), 1, 1, '') as Roles";
            var queryCommand = @"
FROM [PortalLogins] l
WHERE l.Id=@Id";

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var transcation = connection.BeginTransaction())
                {
                    try
                    {
                        PortalLoginDto dto = connection.Query<PortalLoginDto>($@"{columnsCommand} {queryCommand}", new { Id = id }, transcation).FirstOrDefault();
                        var login = new PortalLogin()
                        {
                            Id = id,
                            Username = username,
                            PasswordHash = isPasswordChanged ? _passwordHasher.HashPassword(password) : dto.PasswordHash,
                            IsPasswordToChange = dto.IsPasswordToChange ? dto.IsPasswordToChange : isPasswordToChange,
                            IsLocked = isLocked,
                            CreationTime = DateTime.UtcNow,
                            CreatorId = CurrentUserId,
                            EditorId = CurrentUserId,
                            LastEditTime = DateTime.UtcNow,
                            IsValid = true,
                        };

                        connection.Update(login, transcation);

                        var existingRoles = connection.Query<PortalLoginRole>(@"SELECT * FROM PortalLoginRoles WHERE LoginId=@Id",
                                                                     new { Id = id }, transcation);

                        var roles = existingRoles.Select(r => r.RoleId);
                        var toAdd = roleIds.Where(r => !roles.Contains(r));
                        var toRemove = existingRoles.Where(r => !roleIds.Contains(r.RoleId));

                        foreach (var r in toAdd)
                        {
                            var role = new PortalLoginRole()
                            {
                                LoginId = login.Id,
                                RoleId = r
                            };
                            connection.Insert(role, transcation);
                        }

                        foreach (var r in toRemove)
                        {
                            connection.Delete(r, transcation);
                        }



                        dto = connection.Query<PortalLoginDto>($@"{columnsCommand} {queryCommand}", new { Id = id }, transcation).FirstOrDefault();

                        transcation.Commit();

                        return dto;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                        transcation.Rollback();
                        return null;
                    }
                }
            }
        }

        public IEnumerable<SimplePortalLoginDto> GetOwnerCandidates()
        {
            var command = @"
SELECT l.Id, l.Username
FROM [PortalLogins] l
    INNER JOIN PortalLoginRoles lr on l.Id=lr.LoginId
    INNER JOIN PortalRoles r ON lr.RoleId=r.Id
WHERE l.IsValid=1 AND r.Name in ('PM','Developer')
GROUP BY l.Id, l.Username";
            using (var connection = GetConnection())
            {
                connection.Open();
                var result = connection.Query<SimplePortalLoginDto>(command);
                return result;
            }
        }

        public string GetCurrentLogin()
        {
            var loginId = CurrentUserId;
            return loginId;
        }

        public IEnumerable<PortalRoleDto> GetRoles(bool? isValid)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var command = @"
SELECT r.*
FROM [PortalRoles] r
WHERE @IsValid IS NULL OR r.IsValid=@IsValid";

                var roles = connection.Query<PortalRoleDto>(command, new { IsValid = isValid });

                return roles;
            }
        }

        public bool CheckUsernameAvaiable(string id, string username)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var command = @"
SELECT top 1 *
FROM [PortalLogins] l
WHERE l.Username=@Username AND (@Id is null OR l.Id!=@Id)";

                var login = connection.Query<PortalLoginDto>(command, new { Username = username, Id = id }).FirstOrDefault();

                return login == null;
            }
        }

        public bool ChangePassword(string id, string password, string newPassword)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                var command = @"
SELECT l.*
FROM [PortalLogins] l
WHERE l.IsValid=1 AND l.Id = @Id";

                var login = connection.Query<PortalLogin>(command, new { Id = id }).FirstOrDefault();

                if (login == null)
                {
                    return false;
                }

                var verifyResult = _passwordHasher.VerifyHashedPassword(login.PasswordHash, password);

                if (!verifyResult)
                {
                    return false;
                }

                var newPasswordHash = _passwordHasher.HashPassword(newPassword);

                login.PasswordHash = newPasswordHash;
                login.IsPasswordToChange = false;

                return connection.Update(login);
            }
        }

    }
}

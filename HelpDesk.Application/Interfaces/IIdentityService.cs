using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDesk.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<(bool Success, string[] Errors)> CreateUserAsync(User user, string password, CancellationToken ct = default);
        Task<User?> ValidateCredentialsAsync(string email, string password);
        Task<(bool Success, string[] Errors)> ChangeRoleAsync(User user, UserRole newRole);
        Task<bool> IsInRoleAsync(User user, string role);
    }
}

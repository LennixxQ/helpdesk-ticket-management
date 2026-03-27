using HelpDesk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDesk.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}

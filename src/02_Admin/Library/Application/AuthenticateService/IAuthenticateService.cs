using Domain.Authenticate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated(LoginRequestDTO request, out string token);
    }
}

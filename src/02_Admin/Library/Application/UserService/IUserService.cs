using Domain.Authenticate;
using EFCore.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public interface IUserService
    {
        bool IsValid(LoginRequestDTO req);

        IEnumerable<CmNumberInfo> GetNumberList();
    }
}

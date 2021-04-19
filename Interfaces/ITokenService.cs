using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UdemyDotNetProjectNew.Entities;

namespace UdemyDotNetProjectNew.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}

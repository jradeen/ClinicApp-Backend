using System;
using ClinicApp.API.Models;

namespace ClinicApp.API.Interfaces;

public interface ITokenServices
{
    Task<string> GenerateJwtToken(AppUser user);

}

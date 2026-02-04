using Microsoft.AspNetCore.Builder;

namespace App.Shared.Installers;

public interface IServiceInstaller
{
    void Install(WebApplicationBuilder builder);
}
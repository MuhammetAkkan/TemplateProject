using System.Reflection;
using App.Shared.Installers;

namespace App.Presentation.Extensions.Installers;

public static class InstallerExtensions
{
    // Artık parametre olarak Assembly[] istiyoruz
    public static void InstallServicesInAssembly(this WebApplicationBuilder builder, params Assembly[] assemblies)
    {
        // Gelen her bir assembly içindeki Installer'ları bul
        var installers = assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(x => typeof(IServiceInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IServiceInstaller>()
            .ToList();

        foreach (var installer in installers)
        {
            installer.Install(builder);
        }
    }
}
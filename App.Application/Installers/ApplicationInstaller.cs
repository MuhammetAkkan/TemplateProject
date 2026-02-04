using App.Shared;
using App.Shared.Installers;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using TS.MediatR;

namespace App.Application.Installers;

public class ApplicationInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        // fluentValidation
        builder.Services.AddValidatorsFromAssembly(AssemblyReference.Assembly);
        
    }
}
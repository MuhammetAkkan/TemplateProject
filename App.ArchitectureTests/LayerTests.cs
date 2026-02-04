using NetArchTest.Rules;

namespace App.ArchitectureTests;

public class LayerTests
{
    // Hedef: Domain katmanı, Infrastructure, Application veya Presentation'a bağımlı OLMAMALI.
    // Domain en içteki çekirdektir, dış dünyadan habersiz olmalıdır.
    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Other_Layers()
    {
        // 1. Domain Assembly'sini al
        var domainAssembly = typeof(App.Domain.AssemblyReference).Assembly;

        // 2. Kuralı Tanımla
        var result = Types.InAssembly(domainAssembly)
            .ShouldNot() // EŞLEŞMEMELİ
            .HaveDependencyOn("App.Infrastructure")
            .And()
            .HaveDependencyOn("App.Presentation")
            .And()
            .HaveDependencyOn("App.Application")
            .GetResult();

        // 3. Kontrol Et
        Assert.True(result.IsSuccessful, "Domain katmanı dış katmanlara bağımlı olmamalıdır!");
    }

    // Hedef: Application katmanı, Infrastructure veya Presentation'a bağımlı OLMAMALI.
    // Application sadece Domain'i bilmelidir.
    [Fact]
    public void Application_Should_Not_Have_Dependency_On_Infrastructure_Or_Presentation()
    {
        var applicationAssembly = typeof(App.Application.AssemblyReference).Assembly;

        var result = Types.InAssembly(applicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("App.Infrastructure")
            .And()
            .HaveDependencyOn("App.Presentation")
            .GetResult();

        Assert.True(result.IsSuccessful, "Application katmanı Infra veya Presentation'a bağımlı olmamalıdır!");
    }
    
    // Hedef: Presentation katmanı, Infrastructure'a doğrudan erişmemeli (Genelde Application üzerinden geçer).
    // NOT: Minimal API kullanıyorsan veya DI Container'ı Presentation'da kuruyorsan bu test bazen esnetilebilir.
    // Ama Controller'lar içinde DbContext kullanımı yasaklanmalıdır.
    [Fact]
    public void Presentation_Should_Not_Have_Dependency_On_Infrastructure()
    {
        var presentationAssembly = typeof(App.Presentation.AssemblyReference).Assembly;

        var result = Types.InAssembly(presentationAssembly)
            .That()
            .ResideInNamespace("App.Presentation.Controllers") // Sadece Controller'ları kontrol et
            .ShouldNot()
            .HaveDependencyOn("App.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, "Controller'lar Infrastructure'a doğrudan erişmemelidir!");
    }
}
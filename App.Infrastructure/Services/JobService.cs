using System.Linq.Expressions;
using App.Application.Abstractions.Services;
using Hangfire;

namespace App.Infrastructure.Services;

public sealed class JobService : IJobService
{
    public void Enqueue(Expression<Action> methodCall)
    {
        // Hangfire'ın sihirli metodu burada çalışıyor
        BackgroundJob.Enqueue(methodCall);
    }
}
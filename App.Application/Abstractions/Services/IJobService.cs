using System.Linq.Expressions;

namespace App.Application.Abstractions.Services;

public interface IJobService
{
    // Hangfire'ın çalışma mantığı Expression aldığı için bu imzayı kullanıyoruz.
    // Bu sayede Application katmanı Hangfire'a bağımlı olmadan Lambda expression gönderebilir.
    void Enqueue(Expression<Action> methodCall);
}
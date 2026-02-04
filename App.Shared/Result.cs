using System.Text.Json.Serialization;

namespace App.Shared;

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3
}

public sealed record Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }
    public string[]? ValidationErrors { get; } // Yeni Eklenen Özellik
    
    private Error(string code, string message, ErrorType type, string[]? validationErrors = null)
    {
        Code = code;
        Message = message;
        Type = type;
        ValidationErrors = validationErrors;
    }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new("Error.NullValue", "Değer null olamaz", ErrorType.Failure);

    public static Error Failure(string message) => 
        new("Error.Failure", message, ErrorType.Failure);
    
    public static Error Failure(string code, string message) => 
        new(code, message, ErrorType.Failure);
    
    public static Error NotFound(string message) => 
        new("Error.NotFound", message, ErrorType.NotFound);
    
    // Validasyon metodunu güncelledik: Artık hata listesi alabiliyor
    public static Error Validation(string message, string[]? errors = null) => 
        new("Error.Validation", message, ErrorType.Validation, errors);
    
    public static Error Conflict(string message) => 
        new("Error.Conflict", message, ErrorType.Conflict);

    public static Error InternalServer(string message) => 
        new("Error.InternalServer", message, ErrorType.Failure);
}

// 3. BASE RESULT (Void İşlemler)
public class Result
{
    protected Result(bool isSuccess, Error error, string? successMessage = null)
    {
        // Guard Clause: Mantıksız durumları engelle
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException();

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        
        Error = isSuccess ? null : error;
        
        SuccessMessage = successMessage;
    }

    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; }

    [JsonIgnore]
    public bool IsFailure => !IsSuccess;

    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // Artık null olduğu için çalışacak
    public Error? Error { get; }

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SuccessMessage { get; }

    // --- Factory Methods ---
    public static Result Success() => new(true, Error.None);
    public static Result Success(string message) => new(true, Error.None, message);
    public static Result Failure(Error error) => new(false, error);
    public static Result Failure(string message) => new(false, Error.Failure(message));
    
    public static implicit operator Result(Error error) => Failure(error);
}

// 4. GENERIC RESULT (Data Dönen İşlemler)
public class Result<T> : Result
{
    // Default değer (null) ise JSON'a yazma
    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Value { get; }

    protected Result(bool isSuccess, T? value, Error error, string? successMessage = null) 
        : base(isSuccess, error, successMessage)
    {
        Value = value;
    }

    // --- Factory Methods ---

    public static Result<T> Success(T value) => 
        new(true, value, Error.None);

    public static Result<T> Success(T value, string message) => 
        new(true, value, Error.None, message);

    public new static Result<T> Failure(Error error) => 
        new(false, default, error);
    
    public new static Result<T> Failure(string message) => 
        new(false, default, Error.Failure(message));

    // --- Implicit Operators ---
    
    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) => Failure(error);
}
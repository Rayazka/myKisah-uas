namespace myKisah.Middleware;

// Global Error Handling (semua request)
// PENANGGUNG JAWAB: Josefhint
// TEKNIK: Code Reuse + Defensive Programming
    // - Semua exception ditangkap di SATU tempat
    // - Format response JSON konsisten di seluruh API
    // - Controller fokus ke happy path, tidak perlu try-catch

// ** Penjelasan:
// Middleware yang menangkap SEMUA exception yang terjadi di aplikasi
// dan mengubahnya menjadi HTTP response JSON yang konsisten.

// ** Mapping Exception → HTTP Status:
// | Exception                 | Status | Makna                  |
// |--------------------------|--------|------------------------|
// | ArgumentNullException     | 400    | Input null             |
// | ArgumentException         | 400    | Input tidak valid      |
// | KeyNotFoundException      | 404    | Data tidak ditemukan   |
// | InvalidOperationException | 422    | Operasi tidak valid    |
// | Exception (fallback)      | 500    | Internal server error  |
//
// ** Format response JSON:
// {
//     "error": "Pesan error",
//     "statusCode": 400
// }
//
// TODO:
// [ ] 1. Constructor: terima RequestDelegate next (standar middleware pattern)
//
// [ ] 2. Implement InvokeAsync(HttpContext context):
//        a. try { await _next(context); }
//        b. catch (ArgumentNullException ex) → SetResponse(context, 400, ex.Message)
//        c. catch (ArgumentException ex) → SetResponse(context, 400, ex.Message)
//        d. catch (KeyNotFoundException ex) → SetResponse(context, 404, ex.Message)
//        e. catch (InvalidOperationException ex) → SetResponse(context, 422, ex.Message)
//        f. catch (Exception ex) → SetResponse(context, 500, "Internal server error")
//
// [ ] 3. Implement method SetResponse(HttpContext context, int statusCode, string message):
//        - context.Response.StatusCode = statusCode
//        - context.Response.ContentType = "application/json"
//        - Serialize response object → context.Response.WriteAsync(json)
//        - Response object: new { error = message, statusCode }
//
// [ ] 4. REGISTER DI Program.cs (JANGAN LUPA!):
//        app.UseMiddleware<ErrorHandlingMiddleware>();
//        Tempatkan di PALING ATAS, sebelum middleware lain.
//
// Tips:
// - Pakai System.Text.Json untuk serialisasi
// - Response object anonymous type: new { error = message, statusCode }
// - Pastikan context.Response.ContentType = "application/json"

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        // TODO: Simpan RequestDelegate
        throw new NotImplementedException("TODO: Implement constructor");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // TODO: Implement try-catch dengan mapping exception → HTTP status
        throw new NotImplementedException("TODO: Implement InvokeAsync");
    }

    private static async Task SetResponse(HttpContext context, int statusCode, string message)
    {
        // TODO: Set status code, content type, dan tulis response JSON
        throw new NotImplementedException("TODO: Implement SetResponse");
    }
}

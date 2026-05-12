using System.Text.Json;
using System.Text.Json.Serialization;
namespace myKisah.Utils;

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

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Jalankan middleware berikutnya / controller
            await _next(context);
        }
        catch (ArgumentNullException ex)
        {
            await WriteErrorResponse(context, 400, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteErrorResponse(context, 400, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteErrorResponse(context, 404, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteErrorResponse(context, 422, ex.Message);
        }
        catch (Exception)
        {
            // Exception yang tidak dikenal → 500 tanpa detail
            // (jangan bocorin internal error ke client)
            await WriteErrorResponse(context, 500, "Internal server error.");
        }
    }

    // Ini untuk menghindari duplikasi kode saat menulis response error
    private async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var response = new { error = message, statusCode };
        var json = System.Text.Json.JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }


    private static async Task SetResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new { error = message, statusCode };
        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}

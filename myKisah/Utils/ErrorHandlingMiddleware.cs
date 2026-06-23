using System.Text.Json;
using System.Text.Json.Serialization;

namespace myKisah.Utils;

// Global Error Handling Middleware
// PENANGGUNG JAWAB: Jojo
// TEKNIK: Code Reuse + Defensive Programming
// Tujuan:
// - Tangkap semua exception di satu tempat
// - Response JSON konsisten
// - Controller fokus ke happy path, tidak perlu try-catch di tiap controller

public class ErrorHandlingMiddleware
{
    // RequestDelegate -> pointer ke middleware berikutnya / controller
    private readonly RequestDelegate _next;

    // Constructor: inject middleware selanjutnya
    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // Fungsi utama middleware
    // Menangkap semua exception yang terjadi saat request diproses
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Jalankan middleware berikutnya / controller
            await _next(context);
        }
        catch (ArgumentNullException ex)
        {
            // Input null → status 400
            await WriteErrorResponse(context, 400, ex.Message);
        }
        catch (ArgumentException ex)
        {
            // Input invalid → status 400
            await WriteErrorResponse(context, 400, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            // Entity tidak ditemukan → status 404
            await WriteErrorResponse(context, 404, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            // Operasi tidak valid → status 422
            await WriteErrorResponse(context, 422, ex.Message);
        }
        catch (Exception)
        {
            // Semua exception lainnya → status 500
            // Tidak menampilkan detail internal error untuk keamanan
            await WriteErrorResponse(context, 500, "Internal server error.");
        }
    }

    // Fungsi bantu: menulis response JSON
    // Tujuan:
    // - Hindari duplikasi kode di setiap catch
    // - Response konsisten untuk semua exception
    private async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;           // set HTTP status code
        context.Response.ContentType = "application/json"; // set content type JSON

        // Buat object response
        var response = new { error = message, statusCode };

        // Serialize ke JSON string
        var json = JsonSerializer.Serialize(response);

        // Kirim response ke client
        await context.Response.WriteAsync(json);
    }

    // Versi statis / alternatif (tidak dipakai di InvokeAsync saat ini)
    // Bisa dipanggil tanpa instance class
    private static async Task SetResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new { error = message, statusCode };
        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}
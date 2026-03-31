using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Company.Function;

public class ResizeHttpTrigger
{
    private readonly ILogger<ResizeHttpTrigger> _logger;

    public ResizeHttpTrigger(ILogger<ResizeHttpTrigger> logger)
    {
        _logger = logger;
    }

    [Function("ResizeHttpTrigger")]
public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
{
    _logger.LogInformation("ResizeHttpTrigger function received a request.");

    if (!int.TryParse(req.Query["w"], out int w) ||
        !int.TryParse(req.Query["h"], out int h) ||
        w <= 0 || h <= 0)
    {
        return new BadRequestObjectResult("Invalid parameters w or h.");
    }

    try
    {
        using var msInput = new MemoryStream();
        await req.Body.CopyToAsync(msInput);

        if (msInput.Length == 0)
            return new BadRequestObjectResult("Request body is empty.");

        msInput.Position = 0;

        using var image = Image.Load(msInput);

        image.Mutate(x => x.Resize(w, h));

        using var msOutput = new MemoryStream();
        image.SaveAsJpeg(msOutput);

        byte[] targetImageBytes = msOutput.ToArray();

        return new FileContentResult(targetImageBytes, "image/jpeg");
    }
    catch (UnknownImageFormatException)
    {
        return new BadRequestObjectResult("Invalid image format.");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error.");
        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
}
}
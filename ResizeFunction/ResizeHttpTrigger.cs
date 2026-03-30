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
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        _logger.LogInformation("ResizeHttpTrigger function received a request.");

        if (!int.TryParse(req.Query["w"], out int w) || !int.TryParse(req.Query["h"], out int h))
        {
            return new BadRequestObjectResult("Invalid or missing parameters w and h.");
        }

        byte[] targetImageBytes;

        using (var msInput = new MemoryStream())
        {
            await req.Body.CopyToAsync(msInput);
            msInput.Position = 0;

            using (var image = Image.Load(msInput))
            {
                image.Mutate(x => x.Resize(w, h));

                using (var msOutput = new MemoryStream())
                {
                    image.SaveAsJpeg(msOutput);
                    targetImageBytes = msOutput.ToArray();
                }
            }
        }

        return new FileContentResult(targetImageBytes, "image/jpeg");
    }
}
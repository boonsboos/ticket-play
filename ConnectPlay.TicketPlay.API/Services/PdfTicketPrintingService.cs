using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Abstract;
using ConnectPlay.TicketPlay.Models;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace ConnectPlay.TicketPlay.API.Services;

public class PdfTicketPrintingService : ITicketPrintingService
{
    private readonly IHallRepository hallRepository;

    public PdfTicketPrintingService(IHallRepository hallRepository)
    {
        this.hallRepository = hallRepository;
    }

    public async Task<Stream> PrintTicketsAsync(Order order)
    {
        var screening = order.Tickets.First().Screening;
        var movie = screening.Movie;
        var hall = await hallRepository.GetHallAsync(screening)
            ?? throw new InvalidDataException("Hall for the screening was somehow missing");

        // we can hardcode the NL locale here because the deployment is only located in the Netherlands
        var totalAsString = order.Total.ToString("C", CultureInfo.CreateSpecificCulture("nl-NL"));

        var ticket = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                page.DefaultTextStyle(style => style.FontSize(18));                    

                page.Content()
                    .AlignCenter()
                    .Column(column =>
                    {
                        column.Item().Text(movie.Title)
                            .SemiBold()
                            .FontSize(24);

                        column.Item().Text($"Zaal {hall.HallNumber} om {screening.StartTime:HH:mm}");
                        column.Item().Text($"Stoelen: {string.Join(", ", order.Tickets.Select(t => t.Seat.ToString()))}");
                        column.Item().Text("Betaald: " + totalAsString);

                        column.Item()
                            .MaxHeight(7, Unit.Centimetre)
                            .MaxWidth(7, Unit.Centimetre)
                            .Svg(GetOrderQrCode(order));

                        column.Item().Text("Geniet van de show!")
                            .SemiBold();
                    });                    
            });
        });

        return new MemoryStream(ticket.GeneratePdf());
    }

    private static string GetOrderQrCode(Order order)
    {
        using var qrCodeData = QRCodeGenerator.GenerateQrCode(order.OrderCode, QRCodeGenerator.ECCLevel.Q);
        using var svgRenderer = new SvgQRCode(qrCodeData);
        return svgRenderer.GetGraphic();
    }
}

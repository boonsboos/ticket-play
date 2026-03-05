using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Services;
using NSubstitute;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class SeatAssignmentServiceTests
{
    [TestMethod]
    public async Task AssignAsync_AssignsEmptySeats_InMiddle()
    {
        // Arrange
        var seatRepository = Substitute.For<ISeatRepository>();
        var ticketRepository = Substitute.For<ITicketRepository>();

        var service = new SeatAssignmentService(seatRepository, ticketRepository);

        // Act
        var assignedSeats = await service.AssignAsync(testScreening, [testTicket1]);

        // Assert
        Assert.AreEqual(1, assignedSeats.Count);
        
        var seat = assignedSeats.First();

        var 
    }
}

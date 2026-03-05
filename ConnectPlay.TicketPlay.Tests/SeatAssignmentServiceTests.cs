using ConnectPlay.TicketPlay.Abstract.Repositories;
using ConnectPlay.TicketPlay.API.Services;
using ConnectPlay.TicketPlay.Models;
using NSubstitute;

namespace ConnectPlay.TicketPlay.Tests;

[TestClass]
public class SeatAssignmentServiceTests
{
    private static readonly Hall testHall = new()
    {
        Capacity = 60,
        HallNumber = 10,
        Has3DProjector = true,
        WheelchairAccessible = false,
        Id = 1
    };

    private static readonly Screening testScreening = new()
    {
        Hall = testHall,
        Movie = new Movie
        {
            Id = 1,
            Title = "Test Movie",
            Duration = 120,
            Description = "A test movie description",
            MinimumAge = 0,
            Genre = "Action",
            Language = "nl",
            PosterUrl = new Uri("http://example.com"),
            ReleaseDate = DateOnly.FromDateTime(DateTime.Now),
            Tags = "A"
        },
        HasBreak = true,
    };

    private static readonly TicketType testTicketType = TicketType.Adult;

    [TestMethod]
    public async Task AssignAsync_AssignsSingleSeat()
    {
        // Arrange
        var seatRepository = Substitute.For<ISeatRepository>();
        seatRepository.GetSeatsAsync(Arg.Any<Hall>()).Returns([
            new Seat {
                Hall = testHall,
                Row = 1,
                SeatNumber = 1,
                IsForWheelchair = false
            }
        ]);

        var ticketRepository = Substitute.For<ITicketRepository>();

        var service = new SeatAssignmentService(seatRepository, ticketRepository);

        // Act
        var assignedSeats = await service.AssignAsync(testScreening, [testTicketType]);

        // Assert
        Assert.AreEqual(1, assignedSeats.Count());

        var seat = assignedSeats.First();

        Assert.AreEqual(1, seat.Row);
        Assert.AreEqual(1, seat.SeatNumber);
    }

    [TestMethod]
    public async Task AssignAsync_Assigns_MultipleAdjacentSeats()
    {
        // Arrange
        var seatRepository = Substitute.For<ISeatRepository>();
        seatRepository.GetSeatsAsync(Arg.Any<Hall>()).Returns([
            new Seat {
                Hall = testHall,
                Row = 1,
                SeatNumber = 1,
                IsForWheelchair = false
            },
            new Seat {
                Hall = testHall,
                Row = 1,
                SeatNumber = 2,
                IsForWheelchair = false
            }
        ]);

        var ticketRepository = Substitute.For<ITicketRepository>();

        var service = new SeatAssignmentService(seatRepository, ticketRepository);

        // Act
        var assignedSeats = await service.AssignAsync(testScreening, [testTicketType, testTicketType]);

        // Assert
        Assert.AreEqual(2, assignedSeats.Count());

        var seat1 = assignedSeats.ElementAt(0);
        Assert.AreEqual(1, seat1.Row);
        Assert.AreEqual(1, seat1.SeatNumber);

        var seat2 = assignedSeats.ElementAt(1);
        Assert.AreEqual(1, seat2.Row);
        Assert.AreEqual(2, seat2.SeatNumber);
    }

    [TestMethod]
    public async Task AssignAsync_Assigns_MultipleSeats_InTheSameRow()
    {
        // Arrange
        var seatRepository = Substitute.For<ISeatRepository>();
        seatRepository.GetSeatsAsync(Arg.Any<Hall>()).Returns([
            new Seat {
                Hall = testHall,
                Row = 1,
                SeatNumber = 1,
                IsForWheelchair = false
            },
            new Seat {
                Hall = testHall,
                Row = 1,
                SeatNumber = 3,
                IsForWheelchair = false
            }
        ]);

        var ticketRepository = Substitute.For<ITicketRepository>();

        var service = new SeatAssignmentService(seatRepository, ticketRepository);

        // Act
        var assignedSeats = await service.AssignAsync(testScreening, [testTicketType, testTicketType]);

        // Assert
        Assert.AreEqual(2, assignedSeats.Count());

        var seat1 = assignedSeats.ElementAt(0);
        Assert.AreEqual(1, seat1.Row);
        Assert.AreEqual(1, seat1.SeatNumber);

        var seat2 = assignedSeats.ElementAt(1);
        Assert.AreEqual(1, seat2.Row);
        Assert.AreEqual(3, seat2.SeatNumber);
    }

    [TestMethod]
    public async Task AssignAsync_Assigns_MultipleSeats_InDifferentRows()
    {
        // Arrange
        var seatRepository = Substitute.For<ISeatRepository>();
        seatRepository.GetSeatsAsync(Arg.Any<Hall>()).Returns([
            new Seat {
                Hall = testHall,
                Row = 1,
                SeatNumber = 1,
                IsForWheelchair = false
            },
            new Seat {
                Hall = testHall,
                Row = 3,
                SeatNumber = 3,
                IsForWheelchair = false
            }
        ]);

        var ticketRepository = Substitute.For<ITicketRepository>();

        var service = new SeatAssignmentService(seatRepository, ticketRepository);

        // Act
        var assignedSeats = await service.AssignAsync(testScreening, [testTicketType, testTicketType]);

        // Assert
        Assert.AreEqual(2, assignedSeats.Count());

        var seat1 = assignedSeats.ElementAt(0);
        Assert.AreEqual(1, seat1.Row);
        Assert.AreEqual(1, seat1.SeatNumber);

        var seat2 = assignedSeats.ElementAt(1);
        Assert.AreEqual(3, seat2.Row);
        Assert.AreEqual(3, seat2.SeatNumber);
    }

    [TestMethod]
    public async Task AssignAsync_CannotAssign_IfNoAvailableSpace()
    {
        // Arrange
        var seatRepository = Substitute.For<ISeatRepository>();
        seatRepository.GetSeatsAsync(Arg.Any<Hall>()).Returns([
            new Seat {
                Hall = testHall,
                Row = 1,
                SeatNumber = 1,
                IsForWheelchair = false
            }
        ]);

        var ticketRepository = Substitute.For<ITicketRepository>();

        var service = new SeatAssignmentService(seatRepository, ticketRepository);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await service.AssignAsync(testScreening, [testTicketType, testTicketType]);
        });
    }
}
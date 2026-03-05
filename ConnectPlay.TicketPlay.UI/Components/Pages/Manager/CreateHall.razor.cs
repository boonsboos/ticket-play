using ConnectPlay.TicketPlay.Contracts.Hall;
using ConnectPlay.TicketPlay.UI.Api;
using Microsoft.AspNetCore.Components;

namespace ConnectPlay.TicketPlay.UI.Components.Pages.Manager;

public partial class CreateHall : ComponentBase
{
    [Inject] public required IHallApi HallApi { get; set; }

    private readonly CreateHallFormModel form = new();
    private CreateHallRequest request = new();

    private string? message;
    private string alertClass = "alert-success";

    private bool isSubmitting = false;

    private async Task HandleSubmit()
    {
        isSubmitting = true;
        try
        {
            // basic validation
            if (form.Rows.Count == 0)
                throw new InvalidOperationException("Add at least one row.");

            if (form.Rows.Any(r => r <= 0))
                throw new InvalidOperationException("Each row must have at least 1 seat.");

            HallWheelchairSeat? wheelchair = null;

            if (form.WheelchairRow > 0 || form.WheelchairSeat > 0)
            {
                var row = form.WheelchairRow;
                if (row < 1 || row > form.Rows.Count)
                    throw new InvalidOperationException("Wheelchair row is out of range.");

                var maxSeat = form.Rows[row - 1];
                var seat = form.WheelchairSeat;
                if (seat < 1 || seat > maxSeat)
                    throw new InvalidOperationException("Wheelchair seat is out of range.");

                wheelchair = new HallWheelchairSeat(row, seat);
            }


            request = request with
            {
                HallNumber = form.HallNumber,
                Has3DProjector = form.Has3DProjector,
                WheelchairSeat = (form.WheelchairRow > 0 && form.WheelchairSeat > 0)
                    ? new HallWheelchairSeat(form.WheelchairRow, form.WheelchairSeat)
                    : null,
                Rows = [.. form.Rows]
            };

            await HallApi.CreateNewHallAsync(request);

            message = "Hall created successfully.";
            alertClass = "alert-success";
        }
        catch (Exception ex)
        {
            message = ex.Message;
            alertClass = "alert-danger";
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private void AddRow()
    {
        var defaultSeats = form.Rows.LastOrDefault() > 0 ? form.Rows.Last() : 5;
        form.Rows.Add(defaultSeats);
        EnsureWheelchairSelectionValid();
    }

    private void RemoveRow(int index)
    {
        if (index < 0 || index >= form.Rows.Count) return;

        form.Rows.RemoveAt(index);
        EnsureWheelchairSelectionValid();
    }

    private void EnsureWheelchairSelectionValid()
    {
        if (form.WheelchairRow <= 0 || form.WheelchairSeat <= 0) return;

        // If no rows, clear
        if (form.Rows.Count == 0)
        {
            form.WheelchairRow = 0;
            form.WheelchairSeat = 0;
            return;
        }

        // Clamp row
        form.WheelchairRow = Math.Clamp(form.WheelchairRow, 1, form.Rows.Count);

        // Clamp seat to seats in selected row
        form.WheelchairSeat = Math.Clamp(form.WheelchairSeat, 1, form.Rows[form.WheelchairRow - 1]);
    }

    private int SeatsInSelectedWheelchairRow()
    {
        if (form.WheelchairRow <= 0) return 0;
        var row = form.WheelchairRow;
        if (row < 1 || row > form.Rows.Count) return 0;
        return form.Rows[row - 1];
    }


    public sealed class CreateHallFormModel
    {
        public int HallNumber { get; set; } = 1;
        public bool Has3DProjector { get; set; }

        // dynamic rows: each int = seats in that row
        public List<int> Rows { get; set; } = [5]; // start with 1 row of 5 seats by default
        public int WheelchairRow { get; set; } = 0;
        public int WheelchairSeat { get; set; } = 0;
    }
}
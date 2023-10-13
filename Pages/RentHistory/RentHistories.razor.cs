using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace LibrarySystem.Pages.RentHistory
{
    public partial class RentHistories
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public LibraryService LibraryService { get; set; }

        protected IEnumerable<LibrarySystem.Models.Library.RentHistory> rentHistories;

        protected RadzenDataGrid<LibrarySystem.Models.Library.RentHistory> grid0;
        protected override async Task OnInitializedAsync()
        {
            rentHistories = await LibraryService.GetRentHistories();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddRentHistory>("Add RentHistory", null);
            await grid0.Reload();
        }

        protected async Task EditRow(LibrarySystem.Models.Library.RentHistory args)
        {
            await DialogService.OpenAsync<EditRentHistory>("Edit RentHistory", new Dictionary<string, object> { {"rent_id", args.rent_id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, LibrarySystem.Models.Library.RentHistory rentHistory)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await LibraryService.DeleteRentHistory(rentHistory.rent_id);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete RentHistory"
                });
            }
        }
    }
}
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
    public partial class EditRentHistory
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

        [Parameter]
        public int rent_id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            rentHistory = await LibraryService.GetRentHistoryByRentId(rent_id);
        }
        protected bool errorVisible;
        protected LibrarySystem.Models.Library.RentHistory rentHistory;

        protected async Task FormSubmit()
        {
            try
            {
                await LibraryService.UpdateRentHistory(rent_id, rentHistory);
                DialogService.Close(rentHistory);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}
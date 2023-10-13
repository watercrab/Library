using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace LibrarySystem.Pages.Signin
{
    public partial class Signins
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

        protected IEnumerable<LibrarySystem.Models.Library.Signin> signins;

        protected RadzenDataGrid<LibrarySystem.Models.Library.Signin> grid0;
        protected override async Task OnInitializedAsync()
        {
            signins = await LibraryService.GetSignins();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddSignin>("Add Signin", null);
            await grid0.Reload();
        }

        protected async Task EditRow(LibrarySystem.Models.Library.Signin args)
        {
            await DialogService.OpenAsync<EditSignin>("Edit Signin", new Dictionary<string, object> { {"signin_id", args.signin_id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, LibrarySystem.Models.Library.Signin signin)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await LibraryService.DeleteSignin(signin.signin_id);

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
                    Detail = $"Unable to delete Signin"
                });
            }
        }
    }
}
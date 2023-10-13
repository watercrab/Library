using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace LibrarySystem.Pages.User
{
    public partial class Users
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

        protected IEnumerable<LibrarySystem.Models.Library.User> users;

        protected RadzenDataGrid<LibrarySystem.Models.Library.User> grid0;
        protected override async Task OnInitializedAsync()
        {
            users = await LibraryService.GetUsers();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddUser>("Add User", null);
            await grid0.Reload();
        }

        protected async Task EditRow(LibrarySystem.Models.Library.User args)
        {
            await DialogService.OpenAsync<EditUser>("Edit User", new Dictionary<string, object> { {"user_id", args.user_id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, LibrarySystem.Models.Library.User user)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await LibraryService.DeleteUser(user.user_id);

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
                    Detail = $"Unable to delete User"
                });
            }
        }
    }
}
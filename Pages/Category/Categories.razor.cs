using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace LibrarySystem.Pages.Category
{
    public partial class Categories
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

        protected IEnumerable<LibrarySystem.Models.Library.Category> categories;

        protected RadzenDataGrid<LibrarySystem.Models.Library.Category> grid0;
        protected override async Task OnInitializedAsync()
        {
            categories = await LibraryService.GetCategories();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddCategory>("Add Category", null);
            await grid0.Reload();
        }

        protected async Task EditRow(LibrarySystem.Models.Library.Category args)
        {
            await DialogService.OpenAsync<EditCategory>("Edit Category", new Dictionary<string, object> { {"category_id", args.category_id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, LibrarySystem.Models.Library.Category category)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await LibraryService.DeleteCategory(category.category_id);

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
                    Detail = $"Unable to delete Category"
                });
            }
        }
    }
}
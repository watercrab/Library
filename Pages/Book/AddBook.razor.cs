using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace LibrarySystem.Pages.Book
{
    public partial class AddBook
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

        protected override async Task OnInitializedAsync()
        {
            book = new LibrarySystem.Models.Library.Book();
        }
        protected bool errorVisible;
        protected LibrarySystem.Models.Library.Book book;

        protected async Task FormSubmit()
        {
            try
            {
                await LibraryService.CreateBook(book);
                DialogService.Close(book);
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
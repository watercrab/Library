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
    public partial class EditSignin
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
        public int signin_id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            signin = await LibraryService.GetSigninBySigninId(signin_id);
        }
        protected bool errorVisible;
        protected LibrarySystem.Models.Library.Signin signin;

        protected async Task FormSubmit()
        {
            try
            {
                await LibraryService.UpdateSignin(signin_id, signin);
                DialogService.Close(signin);
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
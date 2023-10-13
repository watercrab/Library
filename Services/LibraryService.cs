using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using LibrarySystem.Data;

namespace LibrarySystem
{
    public partial class LibraryService
    {
        LibraryContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly LibraryContext context;
        private readonly NavigationManager navigationManager;

        public LibraryService(LibraryContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportBooksToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/library/books/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/library/books/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportBooksToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/library/books/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/library/books/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnBooksRead(ref IQueryable<LibrarySystem.Models.Library.Book> items);

        public async Task<IQueryable<LibrarySystem.Models.Library.Book>> GetBooks(Query query = null)
        {
            var items = Context.Books.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnBooksRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnBookGet(LibrarySystem.Models.Library.Book item);
        partial void OnGetBookByBookId(ref IQueryable<LibrarySystem.Models.Library.Book> items);


        public async Task<LibrarySystem.Models.Library.Book> GetBookByBookId(int bookid)
        {
            var items = Context.Books
                              .AsNoTracking()
                              .Where(i => i.book_id == bookid);

 
            OnGetBookByBookId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnBookGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnBookCreated(LibrarySystem.Models.Library.Book item);
        partial void OnAfterBookCreated(LibrarySystem.Models.Library.Book item);

        public async Task<LibrarySystem.Models.Library.Book> CreateBook(LibrarySystem.Models.Library.Book book)
        {
            OnBookCreated(book);

            var existingItem = Context.Books
                              .Where(i => i.book_id == book.book_id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Books.Add(book);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(book).State = EntityState.Detached;
                throw;
            }

            OnAfterBookCreated(book);

            return book;
        }

        public async Task<LibrarySystem.Models.Library.Book> CancelBookChanges(LibrarySystem.Models.Library.Book item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnBookUpdated(LibrarySystem.Models.Library.Book item);
        partial void OnAfterBookUpdated(LibrarySystem.Models.Library.Book item);

        public async Task<LibrarySystem.Models.Library.Book> UpdateBook(int bookid, LibrarySystem.Models.Library.Book book)
        {
            OnBookUpdated(book);

            var itemToUpdate = Context.Books
                              .Where(i => i.book_id == book.book_id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(book);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterBookUpdated(book);

            return book;
        }

        partial void OnBookDeleted(LibrarySystem.Models.Library.Book item);
        partial void OnAfterBookDeleted(LibrarySystem.Models.Library.Book item);

        public async Task<LibrarySystem.Models.Library.Book> DeleteBook(int bookid)
        {
            var itemToDelete = Context.Books
                              .Where(i => i.book_id == bookid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnBookDeleted(itemToDelete);


            Context.Books.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterBookDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportCategoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/library/categories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/library/categories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCategoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/library/categories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/library/categories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCategoriesRead(ref IQueryable<LibrarySystem.Models.Library.Category> items);

        public async Task<IQueryable<LibrarySystem.Models.Library.Category>> GetCategories(Query query = null)
        {
            var items = Context.Categories.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCategoriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCategoryGet(LibrarySystem.Models.Library.Category item);
        partial void OnGetCategoryByCategoryId(ref IQueryable<LibrarySystem.Models.Library.Category> items);


        public async Task<LibrarySystem.Models.Library.Category> GetCategoryByCategoryId(int categoryid)
        {
            var items = Context.Categories
                              .AsNoTracking()
                              .Where(i => i.category_id == categoryid);

 
            OnGetCategoryByCategoryId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCategoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCategoryCreated(LibrarySystem.Models.Library.Category item);
        partial void OnAfterCategoryCreated(LibrarySystem.Models.Library.Category item);

        public async Task<LibrarySystem.Models.Library.Category> CreateCategory(LibrarySystem.Models.Library.Category category)
        {
            OnCategoryCreated(category);

            var existingItem = Context.Categories
                              .Where(i => i.category_id == category.category_id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Categories.Add(category);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(category).State = EntityState.Detached;
                throw;
            }

            OnAfterCategoryCreated(category);

            return category;
        }

        public async Task<LibrarySystem.Models.Library.Category> CancelCategoryChanges(LibrarySystem.Models.Library.Category item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCategoryUpdated(LibrarySystem.Models.Library.Category item);
        partial void OnAfterCategoryUpdated(LibrarySystem.Models.Library.Category item);

        public async Task<LibrarySystem.Models.Library.Category> UpdateCategory(int categoryid, LibrarySystem.Models.Library.Category category)
        {
            OnCategoryUpdated(category);

            var itemToUpdate = Context.Categories
                              .Where(i => i.category_id == category.category_id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(category);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCategoryUpdated(category);

            return category;
        }

        partial void OnCategoryDeleted(LibrarySystem.Models.Library.Category item);
        partial void OnAfterCategoryDeleted(LibrarySystem.Models.Library.Category item);

        public async Task<LibrarySystem.Models.Library.Category> DeleteCategory(int categoryid)
        {
            var itemToDelete = Context.Categories
                              .Where(i => i.category_id == categoryid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCategoryDeleted(itemToDelete);


            Context.Categories.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCategoryDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportRentHistoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/library/renthistories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/library/renthistories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportRentHistoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/library/renthistories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/library/renthistories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnRentHistoriesRead(ref IQueryable<LibrarySystem.Models.Library.RentHistory> items);

        public async Task<IQueryable<LibrarySystem.Models.Library.RentHistory>> GetRentHistories(Query query = null)
        {
            var items = Context.RentHistories.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnRentHistoriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnRentHistoryGet(LibrarySystem.Models.Library.RentHistory item);
        partial void OnGetRentHistoryByRentId(ref IQueryable<LibrarySystem.Models.Library.RentHistory> items);


        public async Task<LibrarySystem.Models.Library.RentHistory> GetRentHistoryByRentId(int rentid)
        {
            var items = Context.RentHistories
                              .AsNoTracking()
                              .Where(i => i.rent_id == rentid);

 
            OnGetRentHistoryByRentId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnRentHistoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnRentHistoryCreated(LibrarySystem.Models.Library.RentHistory item);
        partial void OnAfterRentHistoryCreated(LibrarySystem.Models.Library.RentHistory item);

        public async Task<LibrarySystem.Models.Library.RentHistory> CreateRentHistory(LibrarySystem.Models.Library.RentHistory renthistory)
        {
            OnRentHistoryCreated(renthistory);

            var existingItem = Context.RentHistories
                              .Where(i => i.rent_id == renthistory.rent_id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.RentHistories.Add(renthistory);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(renthistory).State = EntityState.Detached;
                throw;
            }

            OnAfterRentHistoryCreated(renthistory);

            return renthistory;
        }

        public async Task<LibrarySystem.Models.Library.RentHistory> CancelRentHistoryChanges(LibrarySystem.Models.Library.RentHistory item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnRentHistoryUpdated(LibrarySystem.Models.Library.RentHistory item);
        partial void OnAfterRentHistoryUpdated(LibrarySystem.Models.Library.RentHistory item);

        public async Task<LibrarySystem.Models.Library.RentHistory> UpdateRentHistory(int rentid, LibrarySystem.Models.Library.RentHistory renthistory)
        {
            OnRentHistoryUpdated(renthistory);

            var itemToUpdate = Context.RentHistories
                              .Where(i => i.rent_id == renthistory.rent_id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(renthistory);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterRentHistoryUpdated(renthistory);

            return renthistory;
        }

        partial void OnRentHistoryDeleted(LibrarySystem.Models.Library.RentHistory item);
        partial void OnAfterRentHistoryDeleted(LibrarySystem.Models.Library.RentHistory item);

        public async Task<LibrarySystem.Models.Library.RentHistory> DeleteRentHistory(int rentid)
        {
            var itemToDelete = Context.RentHistories
                              .Where(i => i.rent_id == rentid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnRentHistoryDeleted(itemToDelete);


            Context.RentHistories.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterRentHistoryDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportSigninsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/library/signins/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/library/signins/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSigninsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/library/signins/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/library/signins/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSigninsRead(ref IQueryable<LibrarySystem.Models.Library.Signin> items);

        public async Task<IQueryable<LibrarySystem.Models.Library.Signin>> GetSignins(Query query = null)
        {
            var items = Context.Signins.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnSigninsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSigninGet(LibrarySystem.Models.Library.Signin item);
        partial void OnGetSigninBySigninId(ref IQueryable<LibrarySystem.Models.Library.Signin> items);


        public async Task<LibrarySystem.Models.Library.Signin> GetSigninBySigninId(int signinid)
        {
            var items = Context.Signins
                              .AsNoTracking()
                              .Where(i => i.signin_id == signinid);

 
            OnGetSigninBySigninId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnSigninGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSigninCreated(LibrarySystem.Models.Library.Signin item);
        partial void OnAfterSigninCreated(LibrarySystem.Models.Library.Signin item);

        public async Task<LibrarySystem.Models.Library.Signin> CreateSignin(LibrarySystem.Models.Library.Signin signin)
        {
            OnSigninCreated(signin);

            var existingItem = Context.Signins
                              .Where(i => i.signin_id == signin.signin_id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Signins.Add(signin);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(signin).State = EntityState.Detached;
                throw;
            }

            OnAfterSigninCreated(signin);

            return signin;
        }

        public async Task<LibrarySystem.Models.Library.Signin> CancelSigninChanges(LibrarySystem.Models.Library.Signin item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSigninUpdated(LibrarySystem.Models.Library.Signin item);
        partial void OnAfterSigninUpdated(LibrarySystem.Models.Library.Signin item);

        public async Task<LibrarySystem.Models.Library.Signin> UpdateSignin(int signinid, LibrarySystem.Models.Library.Signin signin)
        {
            OnSigninUpdated(signin);

            var itemToUpdate = Context.Signins
                              .Where(i => i.signin_id == signin.signin_id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(signin);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSigninUpdated(signin);

            return signin;
        }

        partial void OnSigninDeleted(LibrarySystem.Models.Library.Signin item);
        partial void OnAfterSigninDeleted(LibrarySystem.Models.Library.Signin item);

        public async Task<LibrarySystem.Models.Library.Signin> DeleteSignin(int signinid)
        {
            var itemToDelete = Context.Signins
                              .Where(i => i.signin_id == signinid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnSigninDeleted(itemToDelete);


            Context.Signins.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSigninDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportUsersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/library/users/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/library/users/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportUsersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/library/users/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/library/users/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnUsersRead(ref IQueryable<LibrarySystem.Models.Library.User> items);

        public async Task<IQueryable<LibrarySystem.Models.Library.User>> GetUsers(Query query = null)
        {
            var items = Context.Users.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnUsersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnUserGet(LibrarySystem.Models.Library.User item);
        partial void OnGetUserByUserId(ref IQueryable<LibrarySystem.Models.Library.User> items);


        public async Task<LibrarySystem.Models.Library.User> GetUserByUserId(int userid)
        {
            var items = Context.Users
                              .AsNoTracking()
                              .Where(i => i.user_id == userid);

 
            OnGetUserByUserId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnUserGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnUserCreated(LibrarySystem.Models.Library.User item);
        partial void OnAfterUserCreated(LibrarySystem.Models.Library.User item);

        public async Task<LibrarySystem.Models.Library.User> CreateUser(LibrarySystem.Models.Library.User user)
        {
            OnUserCreated(user);

            var existingItem = Context.Users
                              .Where(i => i.user_id == user.user_id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Users.Add(user);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(user).State = EntityState.Detached;
                throw;
            }

            OnAfterUserCreated(user);

            return user;
        }

        public async Task<LibrarySystem.Models.Library.User> CancelUserChanges(LibrarySystem.Models.Library.User item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnUserUpdated(LibrarySystem.Models.Library.User item);
        partial void OnAfterUserUpdated(LibrarySystem.Models.Library.User item);

        public async Task<LibrarySystem.Models.Library.User> UpdateUser(int userid, LibrarySystem.Models.Library.User user)
        {
            OnUserUpdated(user);

            var itemToUpdate = Context.Users
                              .Where(i => i.user_id == user.user_id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(user);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterUserUpdated(user);

            return user;
        }

        partial void OnUserDeleted(LibrarySystem.Models.Library.User item);
        partial void OnAfterUserDeleted(LibrarySystem.Models.Library.User item);

        public async Task<LibrarySystem.Models.Library.User> DeleteUser(int userid)
        {
            var itemToDelete = Context.Users
                              .Where(i => i.user_id == userid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnUserDeleted(itemToDelete);


            Context.Users.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterUserDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}
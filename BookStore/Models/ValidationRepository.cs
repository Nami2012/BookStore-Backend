using System.Linq;


namespace BookStore.Models.Auth
{
    public class ValidationRepository
    {
        BookStoreDBEntities context = new BookStoreDBEntities();
        public User_Credentials ValidateUser(string username, string password)
        {
            return context.User_Credentials.FirstOrDefault(User_Credentials =>
                                        User_Credentials.Username == username && User_Credentials.Password == password);
        }

        public Admin ValidateAdmin(string username, string password)
        {
            return context.Admins.FirstOrDefault(Admin =>
                                        Admin.Username == username && Admin.Password == password);
        }
        public void dispose()
        {
            context.Dispose();
        }
    }
}
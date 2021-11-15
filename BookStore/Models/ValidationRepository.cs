using System.Linq;


namespace BookStore.Models.Auth
{
    public class ValidationRepository
    {
        BookStoreDBEntities context = new BookStoreDBEntities();
        public User_Credentials ValidateUser(string username, string password)
        {
            User_Credentials user  = context.User_Credentials.FirstOrDefault(User_Credentials =>
                                        User_Credentials.Username == username && User_Credentials.Password == password);
            if(user != null)
            {
                User_Account_Info user_info = context.User_Account_Info.SingleOrDefault(u => u.UId == user.UId);
                if (user_info.ActiveStatus)
                {
                    return user;
                }
            }
            return null;
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
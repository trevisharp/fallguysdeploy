using System.Linq;
using server.Models;
using System.Threading.Tasks;

public static class TokenSystem
{
    public static async Task<User> Test(string token)
    {
        User user = null;
        if (token.Length != 24)
        {
            var deviceids = await User.Where(t => t.Device == token);
            user = deviceids.FirstOrDefault();
        }
        else
        {
            var tokens = await Token.Where(t => t.Id == token);
            if (tokens.Count() == 0)
            {
                var deviceids = await User.Where(t => t.Device == token);
                user = deviceids.FirstOrDefault();
            }
            else
            {
                var userid = tokens.FirstOrDefault()?.UserId ?? "";
                var users = await User.Where(u => u.Id == userid);
                user = users?.FirstOrDefault();
            }
        }
        return user;
    }
}
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Models;

// Global namespace
public interface IUserStore
{
    Task<bool> ValidateCredentials(string username, string password);
    Task<AppUser> FindBySubjectId(string subjectId);
    Task<AppUser> FindByUsername(string username);
    Task<AppUser> FindByExternalProvider(string provider, string subjectId);
    Task<AppUser> AutoProvisionUser(string provider, string subjectId, List<Claim> claims);
    Task<bool> SaveAppUser(AppUser user, string newPasswordToHash = null);

    Task<List<AppUser>> GetAllOrOneUsers(int id);
    Task DeleteUser(int id);
}
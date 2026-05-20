using Microsoft.EntityFrameworkCore;
using UserApi.Shared.Data;
using UserApi.Shared.Exceptions;

namespace UserApi.Features.Users;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User> GetByIdAsync(long id);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(long id, User updated);
    Task DeleteAsync(long id);
}

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) => _db = db;

    public async Task<List<User>> GetAllAsync() =>
        await _db.Users.Include(u => u.Address).Include(u => u.Company).ToListAsync();

    public async Task<User> GetByIdAsync(long id) =>
        await _db.Users.Include(u => u.Address).Include(u => u.Company).FirstOrDefaultAsync(u => u.Id == id)
        ?? throw new UserNotFoundException(id);

    public async Task<User> CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(long id, User updated)
    {
        var existing = await GetByIdAsync(id);
        existing.Name = updated.Name;
        existing.Username = updated.Username;
        existing.Email = updated.Email;
        existing.Phone = updated.Phone;
        existing.Website = updated.Website;
        existing.Address = updated.Address;
        existing.Company = updated.Company;
        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(long id)
    {
        var user = await GetByIdAsync(id);
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }
}

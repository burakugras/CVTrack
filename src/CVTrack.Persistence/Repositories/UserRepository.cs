using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CVTrack.Application.Interfaces;
using CVTrack.Domain.Entities;
using CVTrack.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CVTrack.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.CVs)
                .Include(u => u.JobApplications)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var list = await _context.Users.ToListAsync();
            return list;
        }

        public Task<User?> GetByIdAsync(Guid id) =>
            _context.Users.FindAsync(id).AsTask();

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}

using DevProject.Domain.Entities;
using DevProject.Domain.Interfaces;
using DevProject.Infrastructure.Context;

namespace DevProject.Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }
}

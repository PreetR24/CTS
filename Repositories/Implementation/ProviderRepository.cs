using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace CareSchedule.Repositories.Implementation
{
    public class ProviderRepository(CareScheduleContext _db) : IProviderRepository
    {
        public List<Provider> GetAll()
        {
            return _db.Providers.AsNoTracking().ToList();
        }

        public Provider? GetById(int id)
        {
            return _db.Providers.AsNoTracking().FirstOrDefault(p => p.ProviderId == id);
        }

        public Provider Create(Provider entity)
        {
            _db.Providers.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public Provider CreateWithId(Provider entity, int providerId)
        {
            if (providerId <= 0) throw new ArgumentException("Invalid providerId.");

            _db.Database.OpenConnection();
            using var tx = _db.Database.BeginTransaction();
            try
            {
                _db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Provider] ON;");
                _db.Database.ExecuteSqlRaw(
                    "INSERT INTO [Provider] ([ProviderID], [Name], [Specialty], [Credentials], [ContactInfo], [Status]) VALUES (@id, @name, @specialty, @credentials, @contactInfo, @status);",
                    new SqlParameter("@id", providerId),
                    new SqlParameter("@name", entity.Name),
                    new SqlParameter("@specialty", (object?)entity.Specialty ?? DBNull.Value),
                    new SqlParameter("@credentials", (object?)entity.Credentials ?? DBNull.Value),
                    new SqlParameter("@contactInfo", (object?)entity.ContactInfo ?? DBNull.Value),
                    new SqlParameter("@status", entity.Status)
                );
                _db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Provider] OFF;");
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                try { _db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Provider] OFF;"); } catch { }
                throw;
            }
            finally
            {
                _db.Database.CloseConnection();
            }

            return _db.Providers.First(p => p.ProviderId == providerId);
        }

        public void Update(Provider entity)
        {
            _db.Providers.Update(entity);
            _db.SaveChanges();
        }
    }
}

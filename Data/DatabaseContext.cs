using Microsoft.EntityFrameworkCore;

using CRML.Server.Models;
using CRML.Server.Controllers;


namespace CRML.Server.Data {
    public class DatabaseContext : DbContext {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) {}

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<Motif> Motifs { get; set; } = null!;
    }

    public class CustomerRepository : IControllerRepository<Customer> {
        private readonly DatabaseContext _context;
        private const int max_appointments = 25;

        public CustomerRepository(DatabaseContext context) {
            _context = context;
        }

        public async Task<List<Customer>> GetAll(IGetAllQuery<Customer> query) {
            var customers = _context.Customers.AsQueryable();
            customers = query.ApplyQuery(customers);

            return await customers.ToListAsync();
        }

        public async Task<Customer?> Get(int id) {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer> Create(Customer customer) {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer?> Update(int id, Customer replacement) {
            var customer = _context.Customers.Update(replacement);
            if (customer == null) return null;

            await _context.SaveChangesAsync();

            return customer.Entity;
        }

        public async Task<Customer?> Delete(int id) {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return null;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return customer;
        }
    }

    public class AppointmentRepository : IControllerRepository<Appointment> {
        private readonly DatabaseContext _context;

        public AppointmentRepository(DatabaseContext context) {
            _context = context;
        }

        public async Task<List<Appointment>> GetAll(IGetAllQuery<Appointment> query) {
            var appointments = _context.Appointments.AsQueryable();
            appointments = query.ApplyQuery(appointments);

            return await appointments.ToListAsync();
        }

        public async Task<Appointment?> Get(int id) {
            return await _context.Appointments.FindAsync(id);
        }

        public async Task<Appointment> Create(Appointment appointment) {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }

        public async Task<Appointment?> Update(int id, Appointment replacement) {
            // TODO: I don't know if Update is correct here or if finding and updating each field is correct.
            var appointment = _context.Appointments.Update(replacement);
            if (appointment == null) return null;

            await _context.SaveChangesAsync();

            return appointment.Entity;
        }

        public async Task<Appointment?> Delete(int id) {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return null;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }
    }

    public class MotifRepository : IControllerRepository<Motif> {
        private readonly DatabaseContext _context;

        public MotifRepository(DatabaseContext context) {
            _context = context;
        }

        public async Task<List<Motif>> GetAll(IGetAllQuery<Motif> query) {
            var motifs = _context.Motifs.AsQueryable();
            motifs = query.ApplyQuery(motifs);

            return await motifs.ToListAsync();
        }

        public async Task<Motif?> Get(int id) {
            return await _context.Motifs.FindAsync(id);
        }

        public async Task<Motif> Create(Motif motif) {
            await _context.Motifs.AddAsync(motif);
            await _context.SaveChangesAsync();

            return motif;
        }

        public async Task<Motif?> Update(int id, Motif replacement) {
            // TODO: I don't know if Update is correct here or if finding and updating each field is correct.
            var motif = _context.Motifs.Update(replacement);
            if (motif == null) return null;

            await _context.SaveChangesAsync();

            return motif.Entity;
        }

        public async Task<Motif?> Delete(int id) {
            var motif = await _context.Motifs.FindAsync(id);
            if (motif == null) return null;

            _context.Motifs.Remove(motif);
            await _context.SaveChangesAsync();

            return motif;
        }
    }
}


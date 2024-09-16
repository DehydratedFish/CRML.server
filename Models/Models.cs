using CRML.Server.Controllers;


namespace CRML.Server.Models {
/*
*******************************************************************************
* Customer
*******************************************************************************
*/
    public class Customer {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public bool HasWA { get; set; }
    }

    public class CustomerPost {
        public string Name { get; set; }   = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public bool HasWA { get; set; }

        public Customer ToCustomer() {
            return new Customer {
                Id = 0,
                Name = Name,
                Email = Email,
                Phone = Phone,
                HasWA = HasWA,
            };
        }
    }

    public class CustomerQuery : IGetAllQuery<Customer> {
        public string? Name { get; set; } = null;

        public IQueryable<Customer> ApplyQuery(IQueryable<Customer> query) {
            if (!string.IsNullOrWhiteSpace(Name)) {
                query = query.Where(c => c.Name.Contains(Name));
            }

            return query;
        }
    }

    public class CustomerPreview {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        CustomerPreview(Customer customer) {
            Id   = customer.Id;
            Name = customer.Name;
        }
    }


/*
*******************************************************************************
* Appointment
*******************************************************************************
*/
    public class Appointment {
        public int Id { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public string Kind { get; set; } = string.Empty;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
    }

    public class AppointmentGet {
        public int Id { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public string Kind { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public AppointmentGet(Appointment appointment) {
            Id = appointment.Id;
            Start = appointment.Start;
            End = appointment.End;
            Kind = appointment.Kind;
            CustomerId = appointment.CustomerId;
        }
    }

    public class AppointmentPost {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public string Kind { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public Appointment toAppointment() {
            return new Appointment {
                Id = 0,
                Start = Start,
                End = End,
                Kind = Kind,
                CustomerId = CustomerId,
            };
        }
    }

    public class AppointmentPut {
        public int Id { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public string Kind { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public Appointment toAppointment() {
            return new Appointment {
                Id = Id,
                Start = Start,
                End = End,
                Kind = Kind,
                CustomerId = CustomerId,
            };
        }
    }

    public class AppointmentQuery : IGetAllQuery<Appointment> {
        public int? CustomerId { get; set; } = null;
        public int? Limit { get; set; } = null;
        public DateTimeOffset? After { get; set; } = null;

        public IQueryable<Appointment> ApplyQuery(IQueryable<Appointment> query) {
            if (CustomerId != null) {
                query = query.Where(a => a.CustomerId == CustomerId);
            }

            if (Limit != null) {
                query = query.Take(Limit.GetValueOrDefault());
            }

            if (After != null) {
                query = query.Where(a => a.End > After);
            }

            return query;
        }
    }

/*
*******************************************************************************
* Motif
*******************************************************************************
*/
    public class Motif {
        public int    Id { get; set; }
        public string Titel { get; set; }       = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Position { get; set; }    = string.Empty;
        public int    Deposit { get; set; }
        public int    Payment { get; set; }
        public int    Amount { get; set; }

        public int?   CustomerId { get; set; }
        public List<string> Attachments { get; set; } = new List<string>();
    }

    public class MotifPut {
        public int Id { get; set; }
        public string Titel { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Position { get; set; }    = string.Empty;
        public int    Deposit { get; set; }
        public int    Payment { get; set; }
        public int    Amount { get; set; }

        public int? CustomerId { get; set; }

        public Motif toMotif() {
            return new Motif {
                Id = Id,
                Titel = Titel,
                Description = Description,
                CustomerId = CustomerId,
            };
        }
    }

    public class MotifPost {
        public string Titel { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Position { get; set; }    = string.Empty;
        public int    Deposit { get; set; }
        public int    Payment { get; set; }
        public int    Amount { get; set; }

        public int? CustomerId { get; set; }
        public string[] Attachments { get; set; } = [];

        public Motif toMotif() {
            return new Motif {
                Id = 0,
                Titel = Titel,
                Description = Description,
                CustomerId = CustomerId,
            };
        }
    }

    public class MotifQuery : IGetAllQuery<Motif> {
        public int? CustomerId { get; set; } = null;

        public IQueryable<Motif> ApplyQuery(IQueryable<Motif> query) {
            if (CustomerId != null) {
                query = query.Where(c => c.CustomerId == CustomerId);
            }

            return query;
        }
    }
}


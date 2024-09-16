using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Mvc;

using CRML.Server.Models;


namespace CRML.Server.Controllers {
    public interface IGetAllQuery<T> {
        public IQueryable<T> ApplyQuery(IQueryable<T> queryable);
    }

    // NOTE: This interface is currently only used for controllers so I think it is unnecesarry to put it in a seperate file.
    public interface IControllerRepository<T> {
        public Task<List<T>> GetAll(IGetAllQuery<T> query);
        public Task<T?>      Get(int id);

        public Task<T>       Create(T model);
        public Task<T?>      Update(int id, T replacement);
        public Task<T?>      Delete(int id);
    }

    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase {
        public readonly IControllerRepository<Customer> _repo;

        public CustomerController(IControllerRepository<Customer> repo) {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetAll([FromQuery] CustomerQuery query) {
            var customers = await _repo.GetAll(query);

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> Get(int id) {
            var customer = await _repo.Get(id);
            if (customer == null) return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> Create(CustomerPost customer_post) {
            var new_customer = await _repo.Create(customer_post.ToCustomer());

            return Ok(new_customer);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Customer>> Update(int id, Customer replacement) {
            var customer = await _repo.Update(id, replacement);
            if (customer == null) return NotFound();

            return Ok(customer);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Customer>> Delete(int id) {
            var customer = await _repo.Delete(id);
            if (customer == null) return NotFound();

            return Ok(customer);
        }
    }

    [ApiController]
    [Route("/api/[controller]")]
    public class AppointmentController : ControllerBase {
        public readonly IControllerRepository<Appointment> _repo;

        public AppointmentController(IControllerRepository<Appointment> repo) {
            _repo = repo;
        }

        // TODO: This also gets used on "/api/appointment/anything". Find a way to prevent this.
        [HttpGet]
        public async Task<ActionResult<List<Appointment>>> GetAll([FromQuery] AppointmentQuery query) {
            var appointments = await _repo.GetAll(query);

            // TODO: This is really slow... reading all queried records into an array
            //       just to map it to an additional array. Should be enough for my purposes
            //       for now as I only return a few records anyway. (Limit number of returned things.)
            var results = appointments.Select(a => new AppointmentGet(a));

            return Ok(results);
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> Create(AppointmentPost appointment_post) {
            var new_appointment = await _repo.Create(appointment_post.toAppointment());

            return Ok(new AppointmentGet(new_appointment));
        }

        [HttpPut]
        public async Task<ActionResult<Appointment>> Update(AppointmentPut replacement) {
            var appointment = await _repo.Update(replacement.Id, replacement.toAppointment());
            if (appointment == null) return NotFound();

            return Ok(new AppointmentGet(appointment));
        }
    }

    [ApiController]
    [Route("/api/[controller]")]
    public class MotifController : ControllerBase {
        public readonly IControllerRepository<Motif> _repo;

        public MotifController(IControllerRepository<Motif> repo) {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Motif>>> GetAll([FromQuery] MotifQuery query) {
            var motifs = await _repo.GetAll(query);

            return Ok(motifs);
        }

        [HttpPost]
        public async Task<ActionResult<Motif>> Create(MotifPost motif_post) {
            var new_motif = await _repo.Create(motif_post.toMotif());

            return Ok(new_motif);
        }

        [HttpPut]
        public async Task<ActionResult<Motif>> Update(MotifPut replacement) {
            var motif = await _repo.Update(replacement.Id, replacement.toMotif());
            if (motif == null) return NotFound();

            return Ok(motif);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Motif>> Delete(int id, [FromServices] IWebHostEnvironment env) {
            var deleted = await _repo.Delete(id);
            if (deleted == null) return NotFound();

            var filePath = env.WebRootPath + $"MotifFiles/{id}";
            if (System.IO.Directory.Exists(filePath)) {
                System.IO.Directory.Delete(filePath, true);
            }

            return Ok(deleted);
        }

        [HttpGet("attachment/{id}/{filename}")]
        public IActionResult Download(int id, string filename, [FromServices] IWebHostEnvironment env) {
            var filePath = env.WebRootPath + $"MotifFiles/{id}/{filename}";
            var stream = System.IO.File.OpenRead(filePath);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filename, out string? contentType)) {
                contentType = "application/octet-stream";
            }

            return File(stream, contentType, filename);
        }

        // TODO: Somehow I can't get the files from the parameters. Is it ok to use like this?
        [HttpPost("attachment/{id}")]
        public async Task<IActionResult> Upload(int id, [FromServices] IWebHostEnvironment env) {
            if (Request.Form.Files.Count == 0) return Ok();

            var filePath = env.WebRootPath + $"MotifFiles/{id}/";
            Directory.CreateDirectory(filePath);

            var motif = await _repo.Get(id);
            if (motif == null) return NotFound();

            foreach (var file in Request.Form.Files) {
                var fileName = Path.GetFileName(file.FileName);
                // TODO: Check already present files.

                motif.Attachments.Add(fileName);

                using (FileStream stream = new FileStream(filePath + fileName, FileMode.Create)) {
                    await file.CopyToAsync(stream);
                }
            }

            // TODO: Do this in one step.
            await _repo.Update(motif.Id, motif);

            return Ok();
        }

        [HttpDelete("attachment/{id}/{filename}")]
        public async Task<IActionResult> Delete(int id, string filename, [FromServices] IWebHostEnvironment env) {
            var motif = await _repo.Get(id);
            if (motif == null) return NotFound();

            if (!motif.Attachments.Contains(filename)) return NotFound();

            var filePath = env.WebRootPath + $"MotifFiles/{id}/{filename}";
            System.IO.File.Delete(filePath);

            motif.Attachments.Remove(filename);
            await _repo.Update(id, motif);

            return Ok();
        }
    }
}


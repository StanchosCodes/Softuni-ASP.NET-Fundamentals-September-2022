using Contacts.Data;
using Contacts.Data.Models;
using Contacts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Contacts.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ContactsDbContext context;

        public ContactsController(ContactsDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Contact> contacts = await this.context.Contacts
                .ToListAsync();

            IEnumerable<ContactViewModel> contactModels = contacts.Select(c => new ContactViewModel()
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address,
                Website = c.Website
            });

            return View(contactModels);
        }

        [HttpGet]
        public async Task<IActionResult> Team()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await this.context.Users.Where(u => u.Id == userId)
                .Include(u => u.ApplicationUsersContacts)
                .ThenInclude(uc => uc.Contact)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user id!");
            }

            IEnumerable<ContactViewModel> usersContacts = user.ApplicationUsersContacts
                .Select(uc => new ContactViewModel()
                {
                    Id = uc.Contact.Id,
                    FirstName = uc.Contact.FirstName,
                    LastName = uc.Contact.LastName,
                    Email = uc.Contact.Email,
                    PhoneNumber = uc.Contact.PhoneNumber,
                    Address = uc.Contact.Address,
                    Website = uc.Contact.Website
                });

            return View("Team", usersContacts);
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            AddContactViewModel contactModel = new AddContactViewModel();

            return View(contactModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddContactViewModel contactModel)
        {
            if (!ModelState.IsValid)
            {
                return View(contactModel);
            }

            try
            {
                Contact newContact = new Contact()
                {
                    FirstName = contactModel.FirstName,
                    LastName = contactModel.LastName,
                    Email = contactModel.Email,
                    PhoneNumber = contactModel.PhoneNumber,
                    Address = contactModel.Address,
                    Website = contactModel.Website
                };

                await this.context.Contacts.AddAsync(newContact);
                await this.context.SaveChangesAsync();

                return RedirectToAction("All", "Contacts");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Failed to add contact!");

                return View(contactModel);
            }
        }

        public async Task<IActionResult> AddToTeam(int contactId)
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var user = await this.context.Users
                    .Where(u => u.Id == userId)
                    .Include(u => u.ApplicationUsersContacts)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new ArgumentException("Invalid user id!");
                }

                var contact = await this.context.Contacts.FirstOrDefaultAsync(c => c.Id == contactId);

                if (contact == null)
                {
                    throw new ArgumentException("Invalid contact id!");
                }

                if (!user.ApplicationUsersContacts.Any(uc => uc.ContactId == contactId))
                {
                    user.ApplicationUsersContacts.Add(new ApplicationUserContact()
                    {
                        ApplicationUser = user,
                        Contact = contact
                    });

                    await this.context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Failed to add contact!");
            }

            return RedirectToAction("All", "Contacts");
        }

        public async Task<IActionResult> RemoveFromTeam(int contactId)
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await this.context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.ApplicationUsersContacts)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user id!");
            }

            var contact = user.ApplicationUsersContacts.FirstOrDefault(c => c.ContactId == contactId);

            if (contact == null)
            {
                throw new ArgumentException("Invalid contact id!");
            }

            user.ApplicationUsersContacts.Remove(contact);
            await this.context.SaveChangesAsync();

            return RedirectToAction("Team", "Contacts");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            Contact? contact = await this.context.Contacts.FindAsync(id);

            if (contact == null) 
            {
                throw new ArgumentException("Invalid contact id!");
            }

            AddContactViewModel contactToEdit = new AddContactViewModel()
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber,
                Address = contact.Address,
                Website = contact.Website
            };

            return View(contactToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AddContactViewModel editedContactModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editedContactModel);
            }

            Contact? contactToEdit = await this.context.Contacts.FindAsync(id);

            if (contactToEdit != null)
            {
                contactToEdit.FirstName = editedContactModel.FirstName;
                contactToEdit.LastName = editedContactModel.LastName;
                contactToEdit.Email = editedContactModel.Email;
                contactToEdit.PhoneNumber = editedContactModel.PhoneNumber;
                contactToEdit.Address = editedContactModel.Address;
                contactToEdit.Website = editedContactModel.Website;
            }

            await this.context.SaveChangesAsync();

            return RedirectToAction("All", "Contacts");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Contact? contactToDelete = await this.context.Contacts.FindAsync(id);

            if (contactToDelete != null)
            {
                this.context.Contacts.Remove(contactToDelete);
                await this.context.SaveChangesAsync();
            }

            return RedirectToAction("All", "Contacts");
        }
    }
}

//using Microsoft.AspNet.OData;

using GitInitTest.Entities.Dtos;
using GitInitTest.Entities.Models;
using GitInitTest.Site.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitInitTest.Site.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInformationsApiController : ControllerBase
    {
        //private readonly MtuSecurityContext _context;
        private readonly IUserInformationAdminService _userInformationAdminService;

        public UserInformationsApiController(IUserInformationAdminService userInformationAdminService)
        {
            //_context = context;
            _userInformationAdminService = userInformationAdminService;
        }

        //GET: api/UserInformations
        [HttpGet]
        public async Task<ActionResult<List<ApplicationUser>>> GetUserInformations()
        {
            var allUsers = await _userInformationAdminService.GetAllAsync();
            return allUsers.ToList();
        }

        // GET: api/UserInformations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUserDto>> GetUserInformation(Guid id)
        {
            var userInformation = await _userInformationAdminService.GetByIdAsync(id);

            if (userInformation == null)
            {
                return NotFound();
            }

            return userInformation;
        }

        // GET: api/UserInformations/GetByLanId?lanId=[LanId]
        //[Route("GetByLanId")]
        //[HttpGet]
        //public async Task<ActionResult<AdInformation>> GetUserInformation([FromQuery]string lanId)
        //{
        //    //var userInformation = _userInformationAdminService.GetLookupJson(lanId);

        //    var userInformation = _userInformationAdminService.GetAdInformationByLanId(lanId);

        //    if (userInformation == null)
        //    {
        //        return NotFound();
        //    }

        //    return userInformation;
        //}

        // GET: api/UserInformations/GetByLastName?lastName=[LastName]
        //[Route("GetByLastName")]
        //[EnableQuery()]
        //[HttpGet]
        //public async Task<ActionResult<List<AdInformation>>> GetUserInformationByLastName([FromQuery]string lastName)
        //{
        //    var userInformation = _userInformationAdminService.GetAdInformationByLastName(lastName);

        //    if (userInformation == null)
        //    {
        //        return NotFound();
        //    }

        //    return userInformation;
        //}

        // PUT: api/UserInformations/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUserInformation(int id, UserInformation userInformation)
        //{
        //    if (id != userInformation.UserInformationId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(userInformation).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserInformationExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/UserInformations
        //[HttpPost]
        //public async Task<ActionResult<UserInformation>> PostUserInformation(UserInformation userInformation)
        //{
        //    _context.UserInformations.Add(userInformation);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetUserInformation", new { id = userInformation.UserInformationId }, userInformation);
        //}

        // DELETE: api/UserInformations/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<UserInformation>> DeleteUserInformation(int id)
        //{
        //    var userInformation = await _context.UserInformations.FindAsync(id);
        //    if (userInformation == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.UserInformations.Remove(userInformation);
        //    await _context.SaveChangesAsync();

        //    return userInformation;
        //}

        //private bool UserInformationExists(int id)
        //{
        //    return _context.UserInformations.Any(e => e.UserInformationId == id);
        //}
    }
}
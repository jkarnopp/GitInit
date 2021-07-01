using GitInitTest.Entities.Models;
using GitInitTest.Site.Services;
using GitInitTest.Site.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GitInitTest.Site.Controllers
{
    public class SysConfigsController : Controller
    {
        private readonly ISysConfigService _sysConfigService;

        public SysConfigsController(ISysConfigService sysConfigService)
        {
            _sysConfigService = sysConfigService;
        }

        // GET: SysConfigs

        // GET: SysConfigs/Edit/5
        public async Task<IActionResult> Index()
        {
            var pageView = await _sysConfigService.GetIndexViewAsync();
            return View(pageView);
        }

        // POST: SysConfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("SysConfigId,AppName,AppFolder,DeveloperName,DeveloperEmail,BusinessOwnerName,BusinessOwnerEmail,AppFromName,AppFromEmail,SmtpServer,SmtpPort,UserAdministratorName,UserAdministratorEmail,Rebuild,IsDebug")] SysConfig sysConfig)
        {
            if (sysConfig.SysConfigId != 1)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _sysConfigService.UpdateSysConfig(sysConfig);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index), "Admin");
            }

            var pageView = new SysConfigViewModel();
            pageView.SysConfig = sysConfig;
            pageView.Title = "Edit System Configuration";
            return View(pageView);
        }
    }
}
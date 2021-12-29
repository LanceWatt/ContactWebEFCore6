#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactWebModels;
using MyContactManagementData;
using Microsoft.Extensions.Caching.Memory;
using CreateProjectTest.Models;
using MyContactManagerServices;

namespace CreateProjectTest.Controllers
{
    public class StatesController : Controller
    {
        //private readonly MyContactManagerDbContext _context;
        private readonly IStatesService _statesService;
        private IMemoryCache _cache;

        public StatesController(IStatesService statesService, IMemoryCache cache)
        {;
            _statesService = statesService;
            _cache = cache;
        }

        // GET: States
        public async Task<IActionResult> Index()
        {
            var allStates = new List<State>();

            // if we get the value back all states is going to contain the states at this point
            if (!_cache.TryGetValue(ContactCacheConstants.ALL_STATES, out allStates))
            {
                var allStatesData = await _statesService.GetAllAsync() as List<State>;

                _cache.Set(ContactCacheConstants.ALL_STATES, allStatesData, TimeSpan.FromDays(1));

                return View(allStatesData);
            }

            return View(allStates);
        }

        // GET: States/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var state = await _statesService.GetAsync((int)id);

            if (state == null)
            {
                return NotFound();
            }

            return View(state);
        }

        // GET: States/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken] // Ensures coming back to the correct HTTPRequest, ie no fraud
        public async Task<IActionResult> Create([Bind("Id,Name,Abbreviation")] State state) // Bind these details to the state object
        {
            if (ModelState.IsValid)
            {
                await _statesService.AddOrUpdateAsync(state);
                _cache.Remove(ContactCacheConstants.ALL_STATES); // INVALIDATE CACHE
                return RedirectToAction(nameof(Index));
            }
            return View(state);
        }

        // GET: States/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var state = await _statesService.GetAsync((int)id);
            if (state == null)
            {
                return NotFound();
            }
            return View(state);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Abbreviation")] State state)
        {
            if (id != state.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _statesService.AddOrUpdateAsync(state);

                    _cache.Remove(ContactCacheConstants.ALL_STATES); // INVALIDATE CACHE
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StateExists(state.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(state);
        }

        // GET: States/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var state = await _statesService.GetAsync((int)id);

            if (state == null)
            {
                return NotFound();
            }

            return View(state);
        }

        // POST: States/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _statesService.DeleteAsync((int)id);
            _cache.Remove(ContactCacheConstants.ALL_STATES); // INVALIDATE CACHE
            return RedirectToAction(nameof(Index));
        }

        private bool StateExists(int id)
        {
            return Task.Run(() => _statesService.ExistsAsync(id)).Result;
        }
    }
}

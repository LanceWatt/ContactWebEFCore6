using ContactWebModels;
using Microsoft.EntityFrameworkCore;
using MyContactManagementData;

namespace MyContactManagerRepositories
{
    public class StatesRepository : IStatesRepository
    {
        private MyContactManagerDbContext _context;

        public StatesRepository(MyContactManagerDbContext context)
        {
            _context = context;
        }

        public async Task<IList<State>> GetAllAsync()
        {
            return await _context.States
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<State?> GetAsync(int id)
        {
            return await _context.States
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);

        }

        public async Task<int> AddOrUpdateAsync(State state)
        {
            if(state.Id > 0)
            {
                return await Update(state);
            }

            return await Insert(state);
        }


        private async Task<int> Insert(State state)
        {
            await _context.States.AddAsync(state);
            await _context.SaveChangesAsync();
            return state.Id;
        }

        private async Task<int> Update(State state)
        {
            var existingState = await _context.States.FirstOrDefaultAsync(x => x.Id == state.Id); // Get existing state
            if (existingState == null) throw new Exception("State not found");

            existingState.Abbreviation = state.Abbreviation;
            existingState.Name = state.Name;

            await _context.SaveChangesAsync();
            return state.Id;
        }


        public async Task<int> DeleteAsync(State state)
        {
            return await DeleteAsync(state.Id);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var existingState = await _context.States.FirstOrDefaultAsync(x => x.Id == id); // Get existing state
            if (existingState == null) throw new Exception("State not found");

            await Task.Run(() => { _context.States.Remove(existingState); });
            await _context.SaveChangesAsync();
            return id;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.States.AnyAsync(x => x.Id == id);
        }


    }
}

using Microsoft.EntityFrameworkCore;
using ParkyApi.Data;
using ParkyApi.Models;
using ParkyApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyApi.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext _context;

        public TrailRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public bool CreateTrail(Trail trail)
        {
            _context.Trails.Add(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            _context.Trails.Remove(trail);
            return Save();
        }

        public Trail GetTrail(int trailId)
        {
            return _context.Trails
                .Include(x => x.NationalPark)
                .FirstOrDefault(x => x.Id == trailId);
        }

        public ICollection<Trail> GetTrails()
        {
            return _context.Trails
                .Include(x => x.NationalPark)
                .OrderBy(x => x.Name).ToList();
        }

        public bool TrailExists(string name)
        {
            bool value = _context.Trails.Any(x => x.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool TrailExists(int trailId)
        {
            bool value = _context.Trails.Any(x => x.Id == trailId);
            return value;
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateTrail(Trail trail)
        {
            _context.Trails.Update(trail);
            return Save();
        }

        public ICollection<Trail> GetTrailsInNationalPark(int npId)
        {
            return _context.Trails
                .Include(x => x.NationalPark)
                .Where(x => x.NationalParkId == npId)
                .ToList();
        }
    }
}

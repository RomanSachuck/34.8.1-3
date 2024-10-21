using System;
using System.Linq;
using System.Threading.Tasks;
using HomeApi.Data.Models;
using HomeApi.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace HomeApi.Data.Repos
{
    /// <summary>
    /// Репозиторий для операций с объектами типа "Room" в базе
    /// </summary>
    public class RoomRepository : IRoomRepository
    {
        private readonly HomeApiContext _context;
        
        public RoomRepository (HomeApiContext context)
        {
            _context = context;
        }
        
        /// <summary>
        ///  Найти комнату по имени
        /// </summary>
        public async Task<Room> GetRoomByName(string name)
        {
            return await _context.Rooms
                .Where(r => r.Name == name).FirstOrDefaultAsync();
        }
        
        public async Task<Room> GetRoomById(Guid id)
        {
            return await _context.Rooms
                .Where(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Room[]> GetRooms()
        {
            return await _context.Rooms
                .ToArrayAsync();
        }

        /// <summary>
        ///  Добавить новую комнату
        /// </summary>
        public async Task AddRoom(Room room)
        {
            var entry = _context.Entry(room);
            if (entry.State == EntityState.Detached)
                await _context.Rooms.AddAsync(room);
            
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoom(Room room, UpdateRoomQuery query)
        {
            if (!String.IsNullOrEmpty(query.NewName))
                room.Name = query.NewName;
            if (query.NewArea != 0)
                room.Area = query.NewArea;
            if (query.NewGasConnected != null)
                room.GasConnected = (bool)query.NewGasConnected;
            if (query.NewVoltage != 0)
                room.Voltage = query.NewVoltage;

            var entry = _context.Entry(room);
            if (entry.State == EntityState.Detached)
                _context.Rooms.Update(room);
            
            await _context.SaveChangesAsync();
        }
    }
}
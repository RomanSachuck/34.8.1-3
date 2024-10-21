using System;
using System.Threading.Tasks;
using AutoMapper;
using HomeApi.Contracts.Models.Rooms;
using HomeApi.Data.Models;
using HomeApi.Data.Queries;
using HomeApi.Data.Repos;
using Microsoft.AspNetCore.Mvc;

namespace HomeApi.Controllers
{
    /// <summary>
    /// Контроллер комнат
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : ControllerBase
    {
        private IRoomRepository _repository;
        private IMapper _mapper;
        
        public RoomsController(IRoomRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        [HttpGet] 
        [Route("")] 
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _repository.GetRooms();

            var resp = new GetRoomsResponse()
            {
                RoomAmount = rooms.Length,
                Rooms = _mapper.Map<Room[], RoomView[]>(rooms)
            };
            
            return StatusCode(200, resp);
        }
        
        /// <summary>
        /// Добавление комнаты
        /// </summary>
        [HttpPost] 
        [Route("")] 
        public async Task<IActionResult> Add([FromBody] AddRoomRequest request)
        {
            var existingRoom = await _repository.GetRoomByName(request.Name);
            if (existingRoom == null)
            {
                var newRoom = _mapper.Map<AddRoomRequest, Room>(request);
                await _repository.AddRoom(newRoom);
                return StatusCode(201, $"Комната {request.Name} добавлена!");
            }
            
            return StatusCode(409, $"Ошибка: Комната {request.Name} уже существует.");
        }
        
        [HttpPut] 
        [Route("{id}")] 
        public async Task<IActionResult> Edit(
            [FromRoute] Guid id,
            [FromBody]  EditRoomRequest request)
        {
            var room = await _repository.GetRoomById(id);
            if(room == null)
                return StatusCode(400, $"Ошибка: Комната с идентификатором {id} не существует.");

            var withSameName = await _repository.GetRoomByName(request.NewName);
            if(withSameName != null)
                return StatusCode(400, $"Ошибка: Комната с именем {request.NewName} уже существует. Выберите другое имя!");

            await _repository.UpdateRoom(
                room,
                new UpdateRoomQuery(request.NewName, request.NewArea, request.NewGasConnected, request.NewVoltage)
            );

            return StatusCode(200, $"Комната обновлена! Имя - {room.Name}, Площадь - {room.Area},  " +
                                   $"Наличие газа - {room.GasConnected}, Напряжение - {room.Voltage}");
        }
    }
}
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using FolhaDePonto.DTO;
using FolhaDePonto.Exceptions.FolhaDePontoExceptions;
using Microsoft.AspNetCore.Mvc;

namespace FolhaDePonto.Controllers
{
    [ApiController]
    [Route("v1")]
    public class FolhaDePontoController : ControllerBase
    {

        private readonly ILogger<FolhaDePontoController> _logger;
        private readonly IFolhaDePonto folhaDePonto;
        private readonly IMapper mapper;

        public FolhaDePontoController(ILogger<FolhaDePontoController> logger, IFolhaDePonto folhaDePonto, IMapper mapper)
        {
            _logger = logger;
            this.folhaDePonto = folhaDePonto;
            this.mapper = mapper;
        }

        [HttpPost("batidas")]
        public ObjectResult Batidas([FromBody] TimeMomentCreateDTO timeMoment)
        {
            try
            {
                GetDateTime(timeMoment.DataHora);
                TimeMoment data = mapper.Map<TimeMomentCreateDTO, TimeMoment>(timeMoment);
                data.User = new User { Name = "teste" };
                IEnumerable<TimeMoment> moments = folhaDePonto.ClockIn(data);
                RegisterDTO register = mapper.Map<IEnumerable<TimeMoment>, RegisterDTO>(moments);
                return new OkObjectResult(register) { StatusCode = 201 };
            }
            catch (WeekendExceptions e)
            {
                return ReturnError(e, 403);
            }
            catch (LunchTimeLimitExceptions e)
            {
                return ReturnError(e, 403);
            }
            catch (HoursLimitExceptions e)
            {
                return ReturnError(e, 403);
            }
            catch (HourAlreadyExistsException e)
            {
                return ReturnError(e, 409);
            }
            catch (InvalidDataException e)
            {
                return ReturnError(e, 400);
            }
            catch (Exception e)
            {
                return ReturnError(e, 400);
            }
        }

        [HttpPost("alocacoes")]
        public ObjectResult Alocacao([FromBody] AllocationCreateDTO allocation)
        {
            try
            {
                TimeAllocation data = mapper.Map<AllocationCreateDTO, TimeAllocation>(allocation);
                data.User = new User { Name = "teste" };
                TimeAllocation result = folhaDePonto.AllocateHoursInProject(data);
                AllocationCreateDTO resultReturn = mapper.Map<TimeAllocation, AllocationCreateDTO>(result);
                return new OkObjectResult(resultReturn) { StatusCode = 201 };
            }
            catch (WeekendExceptions e)
            {
                return ReturnError(e, 403);
            }
            catch (TimeAllocationLimitException e)
            {
                return ReturnError(e, 400);
            }
            catch (Exception e)
            {
                return ReturnError(e, 400);
            }
        }

        private void GetDateTime(string dateTimeStr)
        {
            if (String.IsNullOrWhiteSpace(dateTimeStr))
            {
                throw new InvalidDataException("Data e hora em formato inválido");
            }
            DateTime dateTime;
            if (!DateTime.TryParse(dateTimeStr, out dateTime))
            {
                throw new InvalidDataException("Data e hora em formato inválido");
            }
        }

        private ObjectResult ReturnError(Exception e, int statusCode)
        {
            _logger.LogError(e.ToString(), e.StackTrace);
            MessageDTO messageDTO = new MessageDTO { Mensagem = e.Message };
            return new ObjectResult(messageDTO) { StatusCode = statusCode };
        }
    }
}
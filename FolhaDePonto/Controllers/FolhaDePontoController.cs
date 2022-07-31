using AutoMapper;
using BusinessRule.Domain;
using BusinessRule.Exceptions.FolhaDePontoExceptions;
using BusinessRule.Interfaces;
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
                TimeMomentBR data = mapper.Map<TimeMomentCreateDTO, TimeMomentBR>(timeMoment);
                data.User = new UserBR { Name = "teste" };
                IEnumerable<TimeMomentBR> moments = folhaDePonto.ClockIn(data);
                RegisterResponseDTO register = mapper.Map<IEnumerable<TimeMomentBR>, RegisterResponseDTO>(moments);
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
                TimeAllocationBR data = mapper.Map<AllocationCreateDTO, TimeAllocationBR>(allocation);
                data.User = new UserBR { Name = "teste" };
                TimeAllocationBR result = folhaDePonto.AllocateHoursInProject(data);
                AllocationCreateDTO resultReturn = mapper.Map<TimeAllocationBR, AllocationCreateDTO>(result);
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


        [HttpPost("folhas-de-ponto/{mes}")]
        public ObjectResult Alocacao([FromQuery] string month)
        {
            
            try
            {
                var monthDateTime = GetDateTime(month);

                
                ReportBR reportGetDTO = new ReportBR
                { 
                    Month = monthDateTime, 
                    User = new UserBR { Name = "teste" } 
                };
                
                ReportDataBR result = folhaDePonto.GetReport(reportGetDTO);
                var reportResult = mapper.Map<ReportDataBR, ReportResponseDTO>(result);
                
                return new OkObjectResult(reportResult) { StatusCode = 201 };
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

        private DateTime GetDateTime(string dateTimeStr)
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
            return dateTime;
        }


        private ObjectResult ReturnError(Exception e, int statusCode)
        {
            _logger.LogError(e.ToString(), e.StackTrace);
            MessageResponseDTO messageDTO = new MessageResponseDTO { Mensagem = e.Message };
            return new ObjectResult(messageDTO) { StatusCode = statusCode };
        }
    }
}
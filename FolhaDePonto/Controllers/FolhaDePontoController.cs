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
        private readonly IAuthentication authentication;

        public FolhaDePontoController(ILogger<FolhaDePontoController> logger,
            IMapper mapper,
            IFolhaDePonto folhaDePonto,
            IAuthentication authentication
            )
        {
            _logger = logger;
            this.folhaDePonto = folhaDePonto;
            this.mapper = mapper;
            this.authentication = authentication;
        }

        [HttpPost("batidas")]
        public ObjectResult Batidas([FromBody] TimeMomentCreateDTO timeMoment)
        {
            try
            {
                GetDateTime(timeMoment.DataHora);
                TimeMomentBR data = mapper.Map<TimeMomentCreateDTO, TimeMomentBR>(timeMoment);
                UserDTO userDTO = GetSignedInUser();
                var user = mapper.Map<UserDTO, UserBR>(userDTO);
                data.User = user;
                data.UserId = user.Id;
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
                UserDTO userDTO = GetSignedInUser();
                var user = mapper.Map<UserDTO, UserBR>(userDTO);
                data.User = user;
                data.UserId = user.Id;
                TimeAllocationBR result = folhaDePonto.AllocateHoursInProject(data);
                AllocationResponseDTO resultReturn = mapper.Map<TimeAllocationBR, AllocationResponseDTO>(result);
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


        [HttpGet("folhas-de-ponto/{mes}")]
        public ObjectResult FolhaDePonto(string mes)
        {
            try
            {
                var monthDateTime = GetDateTime(mes);
                ReportBR reportGetDTO = new ReportBR
                {
                    Month = monthDateTime,
                };
                UserDTO userDTO = GetSignedInUser();
                var user = mapper.Map<UserDTO, UserBR>(userDTO);
                reportGetDTO.User = user;
                //reportGetDTO.UserId = user.Id;

                ReportDataBR? result = folhaDePonto.GetReport(reportGetDTO);
                if (result == null)
                {
                    return ReturnError(404, "Relatório não encontrado");
                }
                var reportResult = mapper.Map<ReportDataBR, ReportResponseDTO>(result);

                return new OkObjectResult(reportResult) { StatusCode = 201 };
            }
            catch (Exception e)
            {
                return ReturnError(e, 400);
            }
        }

        private UserDTO GetSignedInUser()
        {
            // TODO: Implementar forma de autenticação
            string? token = User?.Identity?.Name;
            var user = authentication.GetSignedInUser(token);
            return user;
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
            return StatusCode(statusCode, messageDTO);
        }

        private ObjectResult ReturnError(int statusCode, object? value)
        {
            _logger.LogError(value.ToString());
            MessageResponseDTO messageDTO = new MessageResponseDTO { Mensagem = value.ToString() };
            return StatusCode(statusCode, messageDTO);
        }
    }
}